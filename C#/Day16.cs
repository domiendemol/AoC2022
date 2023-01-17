using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2022
{
    public class Day16
    {
        private const string INPUTFILE = "day16.txt";
        private int _minutes = 30;
        
        List<Valve> _valves = new List<Valve>();
        private int _maxPressure = 0;
        Stack<string> _stack = new Stack<string>();
        List<Tuple<string, string>> _connections = new List<Tuple<string, string>>();
        List<List<string>> _combos = new List<List<string>>();
        List<int> _comboPressures = new List<int>();
        
        public void Run()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 1).ToList();
            
            // 1. Build valve list without neighbours
            foreach (string line in lines)
            {
                // Create Valve
                var matches = Regex.Match(line, @"Valve (?<valve>\w+) has flow rate=(?<flow>\d+)");
                _valves.Add(new Valve(matches.Groups["valve"].Value, Int32.Parse(matches.Groups["flow"].Value)));
                // Build connections list
                var match = Regex.Match(line, @"Valve (?<valve>\w+).+valves* (?<valves>.+)");
                foreach (string neighbour in match.Groups["valves"].Value.Split(','))
                {
                    _connections.Add(new Tuple<string, string>(matches.Groups["valve"].Value, neighbour.Trim()));
                }
            }

            Valve aa = _valves.First(x => x.name.Equals("AA"));
            // Link 'm together but only the ones with actual flow rates
            foreach (Valve v in _valves) v.paths = GetShortestPaths(v);
            _valves = _valves.Where(x => x.flowRate > 0).ToList();

            VisitValve(aa, 1, 0, false);
            
            Console.WriteLine("RESULT PART 1: " + _maxPressure);
            
            // PART 2
            _minutes = 26;
            foreach (Valve valve in _valves) valve.open = false;

            // store all combinations up to minute 26 for at least 7 valves are opened
            VisitValve(aa, 1, 0, true);

            // find the best combination of 2 paths which have no common opened valves
            int max = 0;
            for (int i=0; i<_combos.Count; i++)
            {
                List<string> combo = _combos[i];
                
                for (int j = 0; j < _combos.Count; j++)
                {
                    List<string> otherCombo = _combos[j];

                    if (!combo.Any(x => otherCombo.Contains(x)))
                    {
                        // no matches
                        max = Math.Max(max, _comboPressures[i] + _comboPressures[j]);
                    }
                }
            }
            Console.WriteLine("RESULT PART 2: " + max);
        }

        void VisitValve(Valve valve, int minute, int pressure, bool save)
        {
            if (minute > _minutes)
            {
                return;
            }
            
            int addedPressure = CurrentPressure();    
            if (pressure + addedPressure > _maxPressure)
            {
                _maxPressure = pressure + addedPressure;
                Console.WriteLine("NEW MAX: " + (pressure + addedPressure) + " - " + String.Join("-", _stack.ToArray()) );
            }

            if (save && _valves.Count(x => x.open) >= 5)
            {
                _combos.Add(_valves.Where(x => x.open).Select(x => x.name).ToList());
                _comboPressures.Add(pressure + (1 + _minutes - minute) * addedPressure);
            } 
            
            if (minute == _minutes)
            {
                return;
            }
            
            _stack.Push(valve.name + "(" + pressure + "/ " + minute + ")");

            // option 1: open valve, adding a minute to the time (pressure goes up)
            if (!valve.open) 
            {
                OpenValve(valve, minute, pressure, save);
            }
            // option 2: visiting the neighbours without opening this valve
            else 
            {
                foreach (Valve nb in valve.paths.Keys.Where(x => !x.open).Where(y => !y.Equals(valve)))
                {
                    int cost = valve.paths[nb];
                    VisitValve(nb, minute + cost, pressure + cost * addedPressure, save);
                }
            }

            _stack.Pop();
        }
        
        void OpenValve(Valve valve, int minute, int pressure, bool save)
        {
            int addedPressure = CurrentPressure();
            
            valve.open = true;
            _stack.Push("open(" + pressure + ")");

            if (_valves.All(x => x.open))
            {
                pressure += addedPressure;
                for (int i = 0; i < _minutes - minute; i++)
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
            
            // Visit other valves
            foreach (Valve nb in valve.paths.Keys.Where(x => !x.open).Where(y => !y.Equals(valve)))
            {
                int cost = valve.paths[nb];
                if (minute + 1 + cost <= _minutes)
                {
                    VisitValve(nb, minute+1+cost, pressure + addedPressure + ((cost)* CurrentPressure()), save);
                }
                else
                {
                    if (save && _valves.Count(x => x.open) >= 5)
                    {
                        _combos.Add(_valves.Where(x => x.open).Select(x => x.name).ToList());
                        _comboPressures.Add(pressure + addedPressure + (_minutes - minute) * CurrentPressure());
                    } 
                    
                    pressure += addedPressure;
                    for (int i = 0; i < _minutes - minute; i++)
                        pressure += CurrentPressure();
                    if (pressure > _maxPressure)
                    {
                        _maxPressure = pressure;
                        Console.WriteLine($"NEW MAX: {pressure} - min:{minute} - valve:{valve.name} - {String.Join("-", _stack.ToArray())}");
                    }
                    _stack.Pop();
                    valve.open = false; 
                    return; 
                }
            }
            valve.open = false; // revert
            _stack.Pop(); // remove the 'open' from the stack
        }

        int CurrentPressure()
        {
            return _valves.Sum(x => x.Pressure);
        }

        // quick dijkstra
        private Dictionary<Valve, int> GetShortestPaths(Valve valve) 
        {
            var pathWeights = new Dictionary<Valve, int> {{valve, 0}};
            var open = new List<Valve> {valve};
            var closed = new List<Valve>();
            while (open.Count > 0 && pathWeights.Count < _connections.Count)  
            {
                foreach (var connection in _connections.Where(connection => connection.Item1.Equals(open[0].name)))
                {
                    Valve target = _valves.Find(x => x.name.Equals(connection.Item2));
                    if (!open.Contains(target) && !closed.Contains(target))
                    {
                        pathWeights.Add(target, pathWeights[open[0]] + 1);
                        open.Add(target);
                    }
                }

                closed.Add(open[0]);
                open.RemoveAt(0);
            }

            return pathWeights;
        }
    }

    class Valve
    {
        public string name;
        public bool open = false;
        public int flowRate;
        public Dictionary<Valve, int> paths;

        public int Pressure { get => open ? flowRate : 0; }
        
        public Valve(string name, int flowRate)
        {
            this.name = name;
            this.flowRate = flowRate;
            if (flowRate == 0) open = true; // mark it as open 
        }
    }
}