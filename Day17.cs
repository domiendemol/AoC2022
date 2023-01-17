using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AoC2022
{
    public class Day17
    {
        private const string INPUTFILE = "day17.txt";
        public static Queue<int>[] _minYPos = new Queue<int>[7];
        static List<string> lineCache = new List<string>();

        int _typeIndex = 0;
        int _windIndex = 0;

        public void Run()
        {
            // init positions cache (last 25 for now)
            for (int i = 0; i < _minYPos.Length; i++)
            {
                _minYPos[i] = new Queue<int>();
                _minYPos[i].Enqueue(-1);
            }

            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 1).ToList();
            var windString = lines[0].Where(c => c == '<' || c == '>').ToArray();

            Part1(windString);
            Part2(windString);
        }

        void Part1(char[] windInput)
        {
            Simulate(2022, windInput);

            Console.WriteLine($"RESULT PART 1: {string.Join(",", _minYPos.Select(x => x.Max()))}");
            Console.WriteLine($"RESULT PART 1: {_minYPos.Max(x => x.Max()) + 1}");
        }

        void Part2(char[] windInput)
        {
            // init positions cache (last 25 for now)
            for (int i = 0; i < _minYPos.Length; i++)
            {
                _minYPos[i] = new Queue<int>();
                _minYPos[i].Enqueue(-1);
            }

            _typeIndex = 0;
            _windIndex = 0;

            // period figured out by analyzing output / y positions
            int period = 1720; // 35;
            int periodStart = 1816; // 63;
            long nrPeriods = (1000000000000L - periodStart + 1L) / period;

            // part 1: before the period starts
            Simulate(periodStart - 1, windInput);
            long h1 = _minYPos.Max(x => x.Max()) + 1;
            // part 2: simulate the period
            Simulate(period, windInput);
            long h2 = _minYPos.Max(x => x.Max()) + 1 - h1;
            // part 3: the rest: between last period end and 1 trillion
            long rest = (1000000000000L - periodStart + 1L) % period;
            Simulate((int) rest, windInput);
            long h3 = _minYPos.Max(x => x.Max()) + 1 - h2 - h1;

            long result = h1 + (h2 * nrPeriods) + h3;

            Console.WriteLine($"RESULT PART 2: {string.Join(",", _minYPos.Select(x => x.Max()))}");
            Console.WriteLine($"RESULT PART 2: {result}");
        }

        void Simulate(int nrRocks, char[] windInput, bool debug = false)
        {
            // keep track of lowest visible y-pos of every column
            int rockCounter = 0;
            Rock rock = null;
            bool spawn = true;
            while (true)
            {
                if (spawn)
                {
                    // Each rock appears so that its left edge is two units away from the left wall and its bottom edge is three units above the highest rock in the room (or the floor, if there isn't one).
                    int topY = _minYPos.Max(x => x.Max());
                    rock = new Rock((Rock.RockType) (_typeIndex++ % (int) Rock.RockType.LENGTH), topY + 4);
                    rockCounter++;
                    spawn = false;
                }

                // wind
                char windDir = windInput[_windIndex++ % windInput.Length];
                rock.Move(windDir);

                // drop
                if (!rock.Drop())
                {
                    if (rockCounter == nrRocks) break;
                    spawn = true;

                    int m = _minYPos.Min(x => x.Max());
                    string l = string.Join(",", _minYPos.Select(x => x.Max() - m));
                    if (lineCache.Contains(l) && debug)
                    {
                        Console.WriteLine(
                            $"DUPE:{rockCounter} - {l} - {lineCache.LastIndexOf(l) + 1} - diff:{rockCounter - lineCache.LastIndexOf(l) - 1}");
                    }

                    lineCache.Add(l);
                }
            }
        }

        class Rock
        {
            public enum RockType
            {
                HOR,
                CROSS,
                L,
                VERT,
                BLOCK,
                LENGTH
            }

            public RockType type;
            public List<Vector2Int> positions = new List<Vector2Int>();

            public Rock(RockType type, int lowY)
            {
                this.type = type;
                switch (type)
                {
                    case RockType.HOR:
                        positions.Add(new Vector2Int(2, lowY));
                        positions.Add(new Vector2Int(3, lowY));
                        positions.Add(new Vector2Int(4, lowY));
                        positions.Add(new Vector2Int(5, lowY));
                        break;
                    case RockType.CROSS:
                        positions.Add(new Vector2Int(2, lowY + 1));
                        positions.Add(new Vector2Int(3, lowY + 2));
                        positions.Add(new Vector2Int(3, lowY + 1));
                        positions.Add(new Vector2Int(3, lowY));
                        positions.Add(new Vector2Int(4, lowY + 1));
                        break;
                    case RockType.L:
                        positions.Add(new Vector2Int(2, lowY));
                        positions.Add(new Vector2Int(3, lowY));
                        positions.Add(new Vector2Int(4, lowY));
                        positions.Add(new Vector2Int(4, lowY + 1));
                        positions.Add(new Vector2Int(4, lowY + 2));
                        break;
                    case RockType.VERT:
                        positions.Add(new Vector2Int(2, lowY));
                        positions.Add(new Vector2Int(2, lowY + 1));
                        positions.Add(new Vector2Int(2, lowY + 2));
                        positions.Add(new Vector2Int(2, lowY + 3));
                        break;
                    case RockType.BLOCK:
                        positions.Add(new Vector2Int(2, lowY));
                        positions.Add(new Vector2Int(2, lowY + 1));
                        positions.Add(new Vector2Int(3, lowY + 1));
                        positions.Add(new Vector2Int(3, lowY));
                        break;
                }
            }

            public void Move(char direction)
            {
                Vector2Int[] movedPositions = new Vector2Int[positions.Count];
                positions.CopyTo(movedPositions);
                for (int i = 0; i < movedPositions.Length; i++)
                {
                    Vector2Int element = movedPositions[i];
                    // try to move left/right
                    if (direction == '<')
                    {
                        if (element.x == 0 || _minYPos[element.x - 1].Any(e => e == element.y)) return;
                        element.x -= 1;
                    }
                    else
                    {
                        if (element.x == 6 || _minYPos[element.x + 1].Any(e => e == element.y)) return;
                        element.x += 1;
                    }

                    movedPositions[i] = element;
                }

                positions = movedPositions.ToList();
            }

            public bool Drop()
            {
                Vector2Int[] movedPositions = new Vector2Int[positions.Count];
                positions.CopyTo(movedPositions);
                for (int i = 0; i < movedPositions.Length; i++)
                {
                    // try to drop
                    Vector2Int element = movedPositions[i];
                    element.y -= 1;
                    if (_minYPos[element.x].Any(e => e == element.y))
                    {
                        UpdateMinY();
                        return false;
                    }

                    movedPositions[i] = element;
                }

                positions = movedPositions.ToList();
                return true;
            }

            void UpdateMinY()
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    _minYPos[positions[i].x].Enqueue(positions[i].y);
                    if (_minYPos[positions[i].x].Count > 75)
                        _minYPos[positions[i].x].Dequeue();
                }
            }
        }
    }
}