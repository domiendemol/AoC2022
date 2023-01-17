using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2022
{
    public class Day15
    {
        const string INPUTFILE = "day15.txt";
            
        List<Sensor> sensors = new List<Sensor>();
        List<Beacon> beacons = new List<Beacon>();
        
        public void Run()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 1).ToList();
            foreach (string line in lines)
            {
                var matches = Regex.Matches(line, @"x=(?<x>-*\d+), y=(?<y>-*\d+)");
                // Console.WriteLine($"Sensor: {matches[0].Groups["x"].Value}, {matches[0].Groups["y"].Value}");
                // Console.WriteLine($"Beacon: {matches[1].Groups["x"].Value}, {matches[1].Groups["y"].Value}");
                beacons.Add(new Beacon(Int32.Parse(matches[1].Groups["x"].Value), Int32.Parse(matches[1].Groups["y"].Value)));
                sensors.Add(new Sensor(Int32.Parse(matches[0].Groups["x"].Value), Int32.Parse(matches[0].Groups["y"].Value), beacons.Last()));
            }
            beacons = beacons.Distinct().ToList();
            
            // for whole row: find all positions mapped by sensors
            // find left most sensor & right most sensor + their ranges
            // loop all positions
            // for every position: determine if it's within the manhattan distance of a sensor
            // exclude known beacons
            var sorted = sensors.OrderBy(s => s.x);
            Sensor leftSensor = sorted.FirstOrDefault();
            Sensor rightSenor = sorted.Last();

            int counter = 0;
            int row = INPUTFILE.Contains("test") ? 10 : 2000000;
            for (int i = leftSensor.x - leftSensor.GetRange(); i < rightSenor.x + rightSenor.GetRange(); i++)
            {
                foreach (Sensor sensor in sensors)
                {
                    if (!beacons.Where(b => b.x == i).Any(b => b.y == row) && sensor.InRange(i, row))
                    {
                        counter++;
                        break;
                    }
                }
            }
            
            Console.WriteLine($"RESULT PART 1: {counter}");

            if (INPUTFILE.Contains("test"))
            {
                Part2BruteForce();
            }
            else
            {
                Part2Smarter();
            }
        }

        // Since there is only 1 such beacon, it must be right at the edges of others
        // Don't just loop all positions, but only follow the edges of the sensor ranges
        void Part2Smarter()
        {
            int counter = 0;
            foreach (Sensor sensor in sensors)
            {
                // loop the edges
                var result = sensor.CheckEdgesWithOthers(sensors);
                Console.WriteLine("Done checking boundaries of sensor: " + counter++);
                if (result.Item1)
                {
                    Console.WriteLine(result);
                    Console.WriteLine($"RESULT PART 2: {(result.Item2*4000000L) + result.Item3}");
                    return;
                }
            }
            
            Console.WriteLine("PART 2: NO RESULT :(");
        }
        
        // WARNING: execute on your own risk
        void Part2BruteForce()
        {
            int counter = 0;
            for (int x = 0; x < (INPUTFILE.Contains("test") ? 20 : 4000000); x++)
            {
                for (int y = 0; y < (INPUTFILE.Contains("test") ? 20 : 2000000); y++)
                {
                    bool free = true;
                    foreach (Sensor sensor in sensors)
                    {
                        if (sensor.InRange(x, y))
                        {
                            free = false;
                            break;
                        }
                    }

                    if (free)
                    {
                        Console.WriteLine($"RESULT PART 2: {x*4000000+y}");
                        return;
                    }

                    if (++counter % 100000 == 0) Console.WriteLine($"{counter} / {16000000000000} => {counter / 16000000000000f}%");
                }
            }
        }
    }

    class Sensor
    {
        public int x;
        public int y;
        public Beacon beacon;

        public Sensor(int x, int y, Beacon beacon)
        {
            this.x = x;
            this.y = y;
            this.beacon = beacon;
        }

        public int GetRange() // dist to its beacon
        {
            return Math.Abs(beacon.x - x) + Math.Abs(beacon.y - y);
        }

        public bool InRange(int x, int y)
        {
            return Math.Abs(this.x - x) + Math.Abs(this.y - y) <= GetRange();
        }

        public Tuple<bool, int, int> CheckEdgesWithOthers(List<Sensor> sensors)
        {
            int range = GetRange() + 1;

            Tuple<bool, int, int> t1 = CheckLineWithOthers(x-range, y, x, y+range, sensors);
            Tuple<bool, int, int> t2 = CheckLineWithOthers(x+range, y, x, y+range, sensors);
            Tuple<bool, int, int> t3 = CheckLineWithOthers(x+range, y, x, y-range, sensors);
            Tuple<bool, int, int> t4 = CheckLineWithOthers(x-range, y, x, y-range, sensors);

            if (t1.Item1) return t1;
            else if (t2.Item1) return t2;
            else if (t3.Item1) return t3;
            else if (t4.Item1) return t4;
            else return new Tuple<bool, int, int>(false, 0, 0);
        }

        // return true with free point!
        Tuple<bool, int, int> CheckLineWithOthers(int x1, int y1, int x2, int y2, List<Sensor> sensors)
        {
            // assuming x1 smaller than x2 etc
            int xDir = x1 < x2 ? 1 : -1;
            int yDir = y1 < y2 ? 1 : -1;

            int y = y1;
            for (int x = x1; x <= x2; x += xDir)
            {
                if (x < 0 || x > 4000000) continue;
                // for (int y = y1; y <= y2; y += yDir)
                // {
                    if (y < 0 || y > 4000000) continue;

                    bool free = true;
                    foreach (Sensor sensor in sensors)
                    {
                        if (sensor.InRange(x, y))
                        {
                            free = false;
                            break;
                        }
                    }
                    if (free) return new Tuple<bool, int, int>(true, x, y);
                // } 
                y += yDir;
            }
            return new Tuple<bool, int, int>(false, 0, 0);
        }

    }

    class Beacon
    {
        public int x;
        public int y;

        public Beacon(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}