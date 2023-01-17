using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day12
    {
        public void Run()
        {
            string[] lines = File.ReadAllText("day12.txt").Trim().Split('\n');

            int path = FindPath(lines, 'S', 0);
            Console.WriteLine($"RESULT PART 1: {path}");

            List<int> results = new List<int>();
            for (int i = 0; i < 1778; i++) 
            {
                results.Add(FindPath(lines, 'a', i));
            }
            
            Console.WriteLine($"PART 2 RESULT: {results.Min()}");
        }
             
        static int FindPath(string[] lines, char starter, int index)
        {
            var map = new Node[41, 161]; //41, 161
            List<Node> nodeList = new List<Node>();
            List<Node> openNodeList = new List<Node>();

            int starterIndex = 0;
            int lineIndex = 0;
            foreach (var line in lines)
            {
                // build nodes
                int colIndex = 0;
                foreach (char c in line)
                {
                    Node node = new Node(lineIndex, colIndex, c, c == starter && starterIndex == index);
                    if (c == starter && starterIndex++ == index)
                    {
                        openNodeList.Add(node); // start with the 'starter' nodes
                    }
                    nodeList.Add(node);

                    map[lineIndex,colIndex] = node;
                    colIndex++;
                }

                lineIndex++;
            }

            // keep going through all open nodes
            while (openNodeList.Count > 0)
            {
                Node node = openNodeList[0];
                openNodeList.RemoveAt(0);

                var neighbours = new[] {new [] {-1, 0}, new [] {1, 0}, new [] {0, -1}, new [] {0, 1}};
                foreach (int[] neighbour in neighbours) 
                {     
                    // validate not outside of map
                    if (node.x + neighbour[0] < 0 || node.x + neighbour[0] >= map.GetLength(0) ||
                        node.y + neighbour[1] < 0 || node.y + neighbour[1] >= map.GetLength(1))
                        continue;
                    
                    Node n = map[node.x + neighbour[0], node.y + neighbour[1]];
                    if ((n.letter - node.letter <= 1) || (n.letter == 'E' && node.letter == 'z') || node.letter == 'S')
                    {
                        var dist = node.dist + 1;
                        if (n.dist > dist)
                        {
                            n.dist = dist;
                            n.path_from = node;
                            openNodeList.Add(n); // this one is close, add to open node list to process
                        }
                    }
                }
            }

            return nodeList.Where(x => x.letter == 'E').Sum(c => c.dist);
        }
        
        
    }

    class Node
    {
        public int x, y;
        public char letter;
        public int dist = Int32.MaxValue;
        public Node path_from;

        public Node(int x, int y, char letter, bool starter)
        {
            this.x = x;
            this.y = y;
            this.letter = letter;
            if (starter) dist = 0;
        }

        public override string ToString()
        {
            return $"{x},{y} - {letter} - {dist}";
        }
    }
}