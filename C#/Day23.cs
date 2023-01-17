using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    // SPACE: 0,0 is top left, so higher Y value is going down
    public class Day23
    {
        private const string INPUTFILE = "day23.txt";
        private const int SIZE = 7;

        private List<Elf> _elves = new List<Elf>();
        private List<List<Vector2Int>>_directions = new List<List<Vector2Int>>();
        private int _directionIndex = 0;
        
        public void Run()
        {
            ParseInput();
            
            _directions.Add(new List<Vector2Int>(){new Vector2Int(-1,-1), new Vector2Int(0,-1), new Vector2Int(1,-1)}); // NORTH
            _directions.Add(new List<Vector2Int>(){new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(1,1)}); // SOUTH
            _directions.Add(new List<Vector2Int>(){new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)}); // WEST
            _directions.Add(new List<Vector2Int>(){new Vector2Int(1,-1), new Vector2Int(1,0), new Vector2Int(1,1)}); // EAST

            int round = 0;
            for (round = 0; round < 10; round++)
            {
                ExecuteRound();
            } 
            PrintMap();

            int minX = _elves.Min(e => e.pos.x);
            int minY = _elves.Min(e => e.pos.y);
            int maxX = _elves.Max(e => e.pos.x);
            int maxY = _elves.Max(e => e.pos.y);
            int result = (maxX - minX + 1) * (maxY - minY + 1);
            result -= _elves.Count;
            Console.WriteLine($"RESULT PART 1: {result}");

            bool moving = true;
            while (moving)
            {
                if (round % 100 == 0) Console.WriteLine("... round ..." + round);
                moving = ExecuteRound();
                round++;
            }
            Console.WriteLine($"RESULT PART 2: {round}");
        }

        void ParseInput()
        {
            // _map = new char[7,7];
            var lines = File.ReadAllText(INPUTFILE).Split('\n').Where(l => l.Length > 0).ToList();
            for (int l = 0; l < lines.Count; l++)
            {
                string line = lines[l];
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '#') _elves.Add(new Elf(new Vector2Int(i, l)));
                }
            }
        }

        bool ExecuteRound()
        {
            // First step
            foreach (Elf elf in _elves)
            {
                if (IsElfAlone(elf)) continue;
                // Loop directions
                for (int d = 0; d < 4; d++)
                {
                    var offsets =_directions[(d + _directionIndex) % 4];
                    bool free = true;
                    foreach (Vector2Int offset in offsets)
                    {
                        if (!IsSpotFree(new Vector2Int(elf.pos.x + offset.x, elf.pos.y + offset.y)))
                        {
                            free = false;
                        }
                    }

                    if (free)
                    {
                        elf.target = elf.pos + offsets[1];
                        break; // stop testing other directions
                    }
                }
            }
            
            // Second step
            bool moved = false;
            foreach (Elf elf in _elves)
            {
                if (!elf.pos.Equals(elf.target) && !_elves.Any(x => !x.Equals(elf) && x.target.Equals(elf.target)))
                {
                    // ok, no other elf wants this spot
                    elf.pos = elf.target;
                    moved = true;
                }
            }
            
            // Cleanup
            foreach (Elf elf in _elves) elf.target = elf.pos;

            _directionIndex++;

            return moved;
        }

        bool IsSpotFree(Vector2Int spot)
        {
            return !_elves.Any(x => x.pos.Equals(spot));
        }

        bool IsElfAlone(Elf elf) // int x, int y 
        {
            foreach (Elf e in _elves)
            {
                if (e.Equals(elf)) continue;
                if (Math.Abs(e.pos.x - elf.pos.x) <= 1 && Math.Abs(e.pos.y - elf.pos.y) <= 1) return false;
            }
            return true;
        }

        void PrintMap()
        {
            // DEBUG 
            for (int y = -5; y < SIZE+5; y++)
            {
                string line = "";
                for (int x = -5; x < SIZE+5 ; x++)
                {
                    line += _elves.Any(e => e.pos.Equals(new Vector2Int(x,y))) ? '#' : '.';
                }
                Console.WriteLine(line);
            }
        }

        class Elf
        {
            public Vector2Int pos;
            public Vector2Int target;

            public Elf(Vector2Int pos)
            {
                this.pos = pos;
                target = pos;
            }

            protected bool Equals(Elf other)
            {
                return pos.Equals(other.pos);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Elf) obj);
            }

            public override int GetHashCode()
            {
                return pos.GetHashCode();
            }
        }
    }
}