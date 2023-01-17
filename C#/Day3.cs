using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day3
    {
        public void Run()
        {
            string[] lines = File.ReadAllText("day3.txt").Trim().Split('\n');

            List<int> commons = new List<int>();
            foreach (string line in lines)
            {
                string pack1 = line.Substring(0, line.Length / 2);
                string pack2 = line.Substring(line.Length / 2);
                char common = FindCommon(new []{pack1, pack2});
                commons.Add(common < 'a' ? common - 38 : common - 96);
            }

            Console.WriteLine($"RESULT PART 1: {commons.Sum()}");
            
            commons.Clear();
            // Part 2 - find groups of 3
            for (int i = 0; i < lines.Length; i+=3)
            {
                char common = FindCommon(new []{lines[i], lines[i+1], lines[i+2]});
                commons.Add(common < 'a' ? common - 38 : common - 96);
            }
            
            Console.WriteLine($"RESULT PART 2: {commons.Sum()}");
        }

        char FindCommon(string[] packs)
        {
            foreach (char c in packs[0])
            {
                int count = 0;
                for (int i = 0; i < packs.Length; i++)
                {
                    if (packs[i].Contains(c)) count++;
                }
                if (count == packs.Length) return c;
            }
            return ' ';
        }
    }
}