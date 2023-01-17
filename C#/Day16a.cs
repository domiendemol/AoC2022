using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2022_broken
{
    public class Day16
    {
        private const string INPUTFILE = "day16-test.txt";
        private const int MINUTES = 24;
        
        List<Valve> _valves = new List<Valve>();
        private int _maxPressure = 0;
        Stack<string> _stack = new Stack<string>();
        
        public void Run()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 1).ToList();
            
            // 1. Build valve list without neighbours
            foreach (string line in lines)
            {
                // Create Valve
                var matches = Regex.Match(line, @"Valve (?<valve>\w+) has flow rate=(?<flow>\d+)");
                _valves.Add(new Valve(matches.Groups["valve"].Value, Int32.Parse(matches.Groups["flow"].Value)));
            }
            // 2. Second pass: link valves
            foreach (string line in lines)
            {
                // Link valves
                var match = Regex.Match(line, @"Valve (?<valve>\w+).+valves* (?<valves>.+)");
                Valve v = _valves.First(x => x.name.Equals(match.Groups["valve"].Value));
                foreach (string neighbour in match.Groups["valves"].Value.Split(','))
                {
                    // Find valve and add as neighbour
                    Valve toLink = _valves.First(x => x.name.Equals(neighbour.Trim()));
                    v.neighbours.Add(toLink);
                    v.costs.Add(toLink, 1);
                }
            }
            // 3. Third pass: remove all valves with flow rate 0
            OptimizeGraph();

            Valve aa = _valves.First(x => x.name.Equals("AA"));
            VisitValve(aa, 1, 0);
            
            Console.WriteLine("RESULT PART 1: " + _maxPressure);
        }

        void VisitValve(Valve valve, int minute, int pressure)
        {
            //minute++; // this is the minute we are in
            if (minute > MINUTES) return;
            
            //Console.WriteLine(String.Join("-", _stack.ToArray()));
            int press = CurrentPressure();    
            if (pressure + press > _maxPressure)
            {
                _maxPressure = pressure + press;
                Console.WriteLine("NEW MAX: " + (pressure + press) + " - " + String.Join("-", _stack.ToArray()) );
            }

            if (minute == MINUTES) return;
            
            _stack.Push(valve.name + "(" + pressure + "/ " + minute + ")");

            // option 1: open valve, adding a minute to the time (pressure goes up)
            if (!valve.open)
            {
                OpenValve(valve, minute, pressure);
            }
            
            // option 2: visiting the neighbours without opening this valve
            foreach (Valve nb in valve.neighbours) //.Where(x => x.flowRate > 0))
            {
                int cost = valve.costs[nb];
                VisitValve(nb, minute+cost, pressure + cost * press);
            }

            _stack.Pop();
        }
        
        void OpenValve(Valve valve, int minute, int pressure)
        {
            int press = CurrentPressure();
            
            valve.open = true;
            _stack.Push("open(" + pressure + ")");

            if (_valves.All(x => x.open))
            {
                for (int i = 0; i < MINUTES - minute; i++)
                    pressure += CurrentPressure();
                if (pressure > _maxPressure)
                {
                    _maxPressure = pressure;
                    Console.WriteLine($"ALL VALVES OPEN, NEW MAX: {pressure} - min:{minute} - valve:{valve.name} - {String.Join("-", _stack.ToArray())}");
                }
                _stack.Pop();
                valve.open = false; 
                return; // ALL VALVES OPEN
            }

            foreach (Valve nb in valve.neighbours) //.Where(x => x.flowRate > 0))
            {
                int cost = valve.costs[nb];
                VisitValve(nb, minute+1+cost, pressure + press + ((cost -1)* CurrentPressure()));
            }
            valve.open = false; // revert
            _stack.Pop(); // remove the 'open' from the stack
        }

        int CurrentPressure()
        {
            return _valves.Sum(x => x.Pressure);
        }

        void OptimizeGraph()
        {
            // Remove 0 flow valves
            foreach (Valve valve in _valves)
            {
                if (valve.flowRate != 0)
                {
                    Valve[] neighbours = new Valve[valve.neighbours.Count];
                    valve.neighbours.CopyTo(neighbours);
                    foreach (Valve nb in neighbours)
                    {
                        if (nb.flowRate == 0)
                        {
                            // add all neighbours 
                            foreach (Valve zeroNb in nb.neighbours.Where(x => !x.Equals(valve)))
                            {
                                valve.AddNeighbour(zeroNb, nb.costs[zeroNb]+1);
                                zeroNb.AddNeighbour(valve, nb.costs[zeroNb]+1);
                                Console.WriteLine($"Adding {zeroNb.name} to {valve.name}");
                            }
                        }
                    }
                }
            }

            foreach (Valve valve in _valves)
            {
                Valve[] neighbours = new Valve[valve.neighbours.Count];
                valve.neighbours.CopyTo(neighbours);
                foreach (Valve nb in neighbours)
                {
                    if (nb.flowRate == 0 && nb.name != "AA") valve.RemoveNeighbour(nb);
                }
            }
        }
    }

    class Valve
    {
        public string name;
        public bool open = false;
        public int flowRate;

        public int Pressure { get => open ? flowRate : 0; }

        public List<Valve> neighbours = new List<Valve>();
        public Dictionary<Valve, int> costs = new Dictionary<Valve,int>();

        public Valve(string name, int flowRate)
        {
            this.name = name;
            this.flowRate = flowRate;
            if (flowRate == 0) open = true; // mark it as open 
        }

        public void AddNeighbour(Valve neighbour, int cost)
        {
            if (!neighbours.Contains(neighbour))
            {
                neighbours.Add(neighbour);           
                costs[neighbour] = cost;
            }
        }

        public void RemoveNeighbour(Valve neighbour)
        {
            neighbours.Remove(neighbour);
            costs.Remove(neighbour);
        }
    }
}