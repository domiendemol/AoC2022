using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC2022
{
    public class Day14
    {            
        bool[,] map = new bool[1000, 1000];
        private int abyss = 180;

        public void Run()
        {
            ReadInput("day14.txt");
            // VisualizeMap();
            
            // simulate sand
            int counter = 0;
            while (true) // true
            {
                counter++;
                if (DropSand()) break;
            }
            //VisualizeMap();
            Console.WriteLine($"RESULT PART 1: {counter-1}");

            // PART 2: reset/read input again but add floor
            int maxY = ReadInput("day14.txt");
            int floor = maxY + 2;
            DrawLine(0, floor, map.GetLength(1)-1, floor);
            // simulate sand
            counter = 0;
            while (true) // true
            {
                counter++;
                if (DropSand()) break;
            }
            VisualizeMap();
            
            Console.WriteLine($"RESULT PART 2: {counter}");
        }

        int ReadInput(string filename)
        {
            map = new bool[1000, 1000];
            var lines = File.ReadAllText(filename).Trim().Split('\n').Where(l => l.Length > 1).ToList();
            
            // 'draw' level
            int maxY = 0;
            foreach (string line in lines)
            {
                string[] poss = line.Split(new string[]{" -> "}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < poss.Length-1; i++)
                {
                    // Console.WriteLine(poss[i] + " - " + poss[i+1]);
                    string[] pos1 = poss[i].Split(',');
                    string[] pos2 = poss[i+1].Split(',');
                    DrawLine(Convert.ToInt32(pos1[0]), Convert.ToInt32(pos1[1]), Convert.ToInt32(pos2[0]), Convert.ToInt32(pos2[1]));

                    if (Convert.ToInt32(pos1[1]) > maxY) maxY = Convert.ToInt32(pos1[1]);
                    if (Convert.ToInt32(pos2[1]) > maxY) maxY = Convert.ToInt32(pos2[1]);
                }
            }

            return maxY;
        }

        // returns true if in the abyss or starting pos is taken
        bool DropSand()
        {
            bool still = false;
            var startPos = (0, 500);
            var pos = startPos;
            while (!still && pos.Item1 < abyss && !map[startPos.Item1, startPos.Item2])
            {
                // try move down
                if (!map[pos.Item1 + 1, pos.Item2])
                    pos = (pos.Item1 + 1, pos.Item2);
                // try down left
                else if (!map[pos.Item1 + 1, pos.Item2-1])
                    pos = (pos.Item1 + 1, pos.Item2-1);
                // try down right
                else if (!map[pos.Item1 + 1, pos.Item2+1])
                    pos = (pos.Item1 + 1, pos.Item2+1);
                // if not possible: still = true
                else
                {
                    still = true;
                    map[pos.Item1, pos.Item2] = true;
                }
            }

            return pos.Item1 >= abyss || map[startPos.Item1, startPos.Item2];
        }

        // Coordinates in 'normal' space: X,Y
        void DrawLine(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int a = x1;
                x1 = x2;
                x2 = a;
            }
            if (y2 < y1)
            {
                int a = y1;
                y1 = y2;
                y2 = a;
            }
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    map[j, i] = true;
                } 
            }
        }

        void VisualizeMap()
        {
            // only the top part at the moment
            for (int i = 0; i <= 20; i++) // map.GetLength(0)
            {
                StringBuilder b = new StringBuilder();
                for (int j = 496; j <= 504; j++)
                {
                    b.Append(map[i, j] ? "#" : ".");
                } 
                Console.WriteLine(b.ToString());
                b.Clear();
            }
        }
    }
}