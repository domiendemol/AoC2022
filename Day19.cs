using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AoC2022
{
    public class Day19
    {
        private const string INPUTFILE = "day19.txt";
        
        const int ORE = 0;
        const int CLAY = 1;
        const int OBSIDIAN = 2;
        const int GEODE = 3;
        private static int NR_MINUTES = 24;
        ConcurrentQueue<Blueprint> _blueprints = new ConcurrentQueue<Blueprint>();
        ConcurrentQueue<Blueprint> _blueprints2 = new ConcurrentQueue<Blueprint>();
        private int[] _max = new int[32];
        
        public void Run()
        {
            ParseInput();

            // PART 1
            // --------
            NR_MINUTES = 24;
            Multithreading();

            int total1 = 0;
            for (int i = 0; i < _max.Length; i++)
            {
                total1 += i * _max[i];
            }
            Console.WriteLine($"RESULT PART 1: {total1}"); // valid result = 1395

            
            // PART 2
            // --------
            NR_MINUTES = 32;
            _blueprints = _blueprints2;
            Multithreading();
            
            int total2 = 1;
            for (int i = 1; i <= 3; i++)
            {
                total2 *= _max[i];
            }
            
            Console.WriteLine($"RESULT PART 1: {total1}");
            Console.WriteLine($"RESULT PART 2: {total2}");
        }

        void Multithreading()
        {
            // multithreading!!
            List<Thread> threads = new List<Thread>();
            foreach (Blueprint blueprint in _blueprints)
            {
                Console.WriteLine("In Main: Creating the Child thread");
                Thread childThread = new Thread(CallToChildThread);
                childThread.Start();
                threads.Add(childThread);
            }
            Console.WriteLine("------");
         
            // wait for them to finish
            foreach(var thread in threads)
            {
                thread.Join();
            }
        }

        public void CallToChildThread() 
        {
            if (_blueprints.TryDequeue(out var blueprint))
            {
                Console.WriteLine("Child thread starts: " + blueprint.id);

                 int[] robots = {1, 0, 0, 0};
                 int[] resources = {0, 0, 0, 0};
        
                // Do a DFS similar to day 16, but with good pruning/optimization
                // keeping track of nr of robots/resources 
                int[] result = new int[NR_MINUTES+1];
                DFS(1, blueprint, robots, resources, result);
            
                Console.WriteLine($"MAX: {_max[blueprint.id]}");
            }
            Console.WriteLine("Child thread ends: " + blueprint.id);
        }
        
        void ParseInput()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 1).ToList();
            foreach (string line in lines)
            {
                var matches = Regex.Match(line, @"Blueprint (?<bp>\d+): .* ore robot costs (?<ore>\d+) ore\. Each clay robot costs (?<clay>\d+) ore\. Each obsidian robot costs (?<obsidianOre>\d+) ore and (?<obsidianClay>\d+) clay\. Each geode robot costs (?<geodeOre>\d+) ore and (?<geodeObsidian>\d+) obsidian");
                string name = matches.Groups["bp"].Value;
                Blueprint blueprint = new Blueprint(Convert.ToInt32(name));
                blueprint.costs[ORE, ORE] = Convert.ToInt32(matches.Groups["ore"].Value);
                blueprint.costs[CLAY, ORE] = Convert.ToInt32(matches.Groups["clay"].Value);
                blueprint.costs[OBSIDIAN, ORE] = Convert.ToInt32(matches.Groups["obsidianOre"].Value);
                blueprint.costs[OBSIDIAN, CLAY] = Convert.ToInt32(matches.Groups["obsidianClay"].Value);                         
                blueprint.costs[GEODE, ORE] = Convert.ToInt32(matches.Groups["geodeOre"].Value);
                blueprint.costs[GEODE, OBSIDIAN] = Convert.ToInt32(matches.Groups["geodeObsidian"].Value);
                _blueprints.Enqueue(blueprint);
                if (blueprint.id <= 3) _blueprints2.Enqueue(blueprint);
            } 
        }
        
        void DFS(int minute, Blueprint blueprint, int[] robots, int[] resources, int[] result)
        {
            if (minute == NR_MINUTES)
            {
                resources[GEODE] += robots[GEODE]; // produce the geodes of this minute
                _max[blueprint.id] = Math.Max(_max[blueprint.id], resources[GEODE]);
                return;
            }

            // optimizations 
            // Crude: If we don't have a clay robot by minute x it's gonna be too late
            if (minute >= 12 && robots[CLAY] == 0) return;

            // make robot choice (descending order)
            result[minute] = -1;
            for (int r = 3; r >= 0; r--)
            {
                int[] newResources = new int[4];
                resources.CopyTo(newResources, 0);
                
                int[] newRobots = new int[4];
                robots.CopyTo(newRobots, 0);
                int minutesForRobot = BuildRobot(minute, r, blueprint, ref newResources, ref newRobots);
                if (minutesForRobot < 0) continue;
                
                result[minute] = r;
                // produce goods 
                for (int ro = 0; ro < 4; ro++) {
                    newResources[ro] += robots[ro];
                }

                if (CalculateUpperBound(minute + minutesForRobot, newResources, newRobots) <= _max[blueprint.id]) break;
                
                DFS(minute + minutesForRobot + 1, blueprint, newRobots, newResources, result);
            }

            if (result[minute] == -1)
            {
                // produce goods 
                for (int ro = 0; ro < 4; ro++) {
                    resources[ro] += robots[ro];
                }
                DFS(minute + 1, blueprint, robots, resources, result);
            }
        }

        int CalculateUpperBound(int minute, int[] resources,  int[] robots)
        {
            int[] newResources = new int[4];
            resources.CopyTo(newResources, 0);
            int[] newRobots = new int[4];
            robots.CopyTo(newRobots, 0);
            
            int minutesLeft = NR_MINUTES - minute;
            
            for (int i = 0; i < minutesLeft; i++)
            {
                newResources[GEODE] += newRobots[GEODE];
                // assume we can build geode robot every turn
                newRobots[GEODE]++;
            }
            
            // 1. geodes by current robots
            // 2. by new geode robots
            return newResources[GEODE];
        }
        
        int BuildRobot(int currentMinute, int robot, Blueprint blueprint, ref int[] resources, ref int[] robots)
        {
            int minute = 0;
            bool canBuild = false;
            while (!canBuild)
            {
                canBuild = true;
                // can we?
                for (int i = 0; i < 4; i++)
                {
                    if (blueprint.costs[robot, i] > resources[i])
                        canBuild = false;
                }

                if (!canBuild)
                {
                    if (++minute > blueprint.costs[robot,ORE]) return -1; // not worth it anymore
                    // produce goods
                    for (int ro = 0; ro < 4; ro++)
                    {
                        resources[ro] += robots[ro];
                    }
                    if (currentMinute + minute +1 > NR_MINUTES) return -1;
                }
            }
 
            // do it
            for (int i = 0; i < 4; i++) {
                resources[i] -= blueprint.costs[robot, i];
            }
            robots[robot]++;


            return minute;
        }    
        
        bool TryBuildRobot(int robot, Blueprint blueprint, ref int[] resources)
        {
            // can we?
            for (int i = 0; i < 4; i++)
            {
                if (blueprint.costs[robot, i] > resources[i])
                    return false;
            }
            // do it
            for (int i = 0; i < 4; i++)
            {
                resources[i] -= blueprint.costs[robot, i];
            }

            return true;
        }
    }

    class Blueprint
    {
        public int id;
        public int[,] costs = new int[4,4]; // robot, costs

        public Blueprint(int id)
        {
            this.id = id;
        }
    }
}