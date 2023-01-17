using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2022
{
    public class Day4
    {
        private const string INPUTFILE = "day4.txt";

        List<Tuple<Vector2Int, Vector2Int>> pairs = new List<Tuple<Vector2Int, Vector2Int>>();
        
        public void Run()
        {
            ParseInput();

            int totalOverlaps = 0;
            int overlaps = 0;
            foreach (Tuple<Vector2Int,Vector2Int> pair in pairs)
            {
                if (pair.Item1.x >= pair.Item2.x && pair.Item1.x <= pair.Item2.y) overlaps++;
                else if (pair.Item2.x >= pair.Item1.x && pair.Item2.x <= pair.Item1.y) overlaps++;
                
                if (pair.Item1.x >= pair.Item2.x && pair.Item1.y <= pair.Item2.y) totalOverlaps++;
                else if (pair.Item2.x >= pair.Item1.x && pair.Item2.y <= pair.Item1.y) totalOverlaps++;
            }
            
            Console.WriteLine($"RESULT PART 1: {totalOverlaps}");
            Console.WriteLine($"RESULT PART 2: {overlaps}");
        }
        
        void ParseInput()
        {
            var lines = File.ReadAllText(INPUTFILE).Split('\n').Where(l => l.Length > 0).ToList();
            foreach (var line in lines)
            {
                Match m = Regex.Match(line, @"(\d+)?-(\d+)?,(\d+)?-(\d+)?");
                pairs.Add(new Tuple<Vector2Int, Vector2Int>(new Vector2Int(Convert.ToInt32(m.Groups[1].Value), Convert.ToInt32(m.Groups[2].Value)), 
                    new Vector2Int(Convert.ToInt32(m.Groups[3].Value), Convert.ToInt32(m.Groups[4].Value))));
            }
        }
    }
}