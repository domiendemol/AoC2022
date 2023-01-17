using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day24
    {
        private const string INPUTFILE = "day24.txt";
        private static Vector2Int[] DIRECTIONS = 
            {new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0,0) };

        private List<Blizzard> _blizzards = new List<Blizzard>();
        private Dictionary<int, List<Blizzard>> _verBlizzards = new Dictionary<int, List<Blizzard>>();
        private Dictionary<int, List<Blizzard>> _horBlizzards = new Dictionary<int, List<Blizzard>>();
        private Vector2Int _startPos;
        private Vector2Int _endPos;
        private Vector2Int _mapSize;
        
        public void Run()
        {
            ParseInput();

            int run1 = BFS(0);
            Console.WriteLine($"RESULT PART 1: {run1}");

            _startPos = _endPos;
            _endPos = new Vector2Int(1, 0);
            int run2 = BFS(run1);
            
            _endPos = _startPos;
            _startPos = new Vector2Int(1, 0);
            int run3 = BFS(run2);

            Console.WriteLine($"RESULT PART 2: {run3}");
        }

        // BFS (if order of choices is correct) returns shortest path as first
        int BFS(int startMinute)
        {
            int blizzPeriod = 600; // (_mapSize.x - 2) * (_mapSize.y - 2);
            Queue<Vector3Int> q = new Queue<Vector3Int>();
            
            // You're only revisiting a node if you visit it at the same exact part of the blizzard cycle
            Dictionary<Vector3Int, bool> visited = new Dictionary<Vector3Int, bool>();
            Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
            Dictionary<Vector3Int, Vector3Int> prev = new Dictionary<Vector3Int, Vector3Int>(); // Path
                
            // Start
            Vector3Int st = new Vector3Int(_startPos.x, _startPos.y, startMinute);
            visited[st] = true;
            distance[st] = startMinute;
            q.Enqueue(st);
            
            while (q.Count > 0) 
            {
                Vector3Int s = q.Dequeue();
                
                // process node s
                foreach (Vector2Int dir in DIRECTIONS)
                {
                    Vector2Int nextPos = s.ToVector2Int() + dir;
                    Vector3Int nextPos3 = new Vector3Int(nextPos.x, nextPos.y, s.z+1);

                    // END !!
                    if (nextPos.Equals(_endPos)) 
                    {
                        // Print path, not necessary anymore
                        /*
                        Vector3Int p = s;
                        while (true)
                        {
                            if (prev.TryGetValue(p, out Vector3Int prevN))
                            {
                                Console.WriteLine(prevN);
                                p = prevN;
                            }
                            else break;

                        }
                        */
                        return nextPos3.z;
                    }
                    // ALREADY VISITED
                    if (visited.TryGetValue(new Vector3Int(nextPos.x, nextPos.y, (nextPos3.z) % blizzPeriod), out bool vis) && vis) continue;
                    // WALL
                    if (!nextPos.Equals(_startPos) && (nextPos.x <= 0 || nextPos.y <= 0 || nextPos.x >= _mapSize.x - 1 || nextPos.y >= _mapSize.y - 1)) continue;
                    // BLIZZARD 
                    if (HasBlizzard(nextPos3.z, nextPos.x, nextPos.y)) continue;
                    
                    visited[new Vector3Int(nextPos.x, nextPos.y, (nextPos3.z) % blizzPeriod)] = true;

                    distance[nextPos3] = nextPos3.z;
                    prev[nextPos3] = s;
                    q.Enqueue(nextPos3);
                }
            }

            return 0;
        }

        bool HasBlizzard(int minute, int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (_verBlizzards.TryGetValue(x, out List<Blizzard> vblizzs))
            {
                foreach (Blizzard b in vblizzs)
                {
                    if (b.GetPos(minute, _mapSize).Equals(pos)) return true;
                }
            }
            if (_horBlizzards.TryGetValue(y, out List<Blizzard> hblizzs))
            {
                foreach (Blizzard b in hblizzs)
                {
                    if (b.GetPos(minute, _mapSize).Equals(pos)) return true;
                }
            }

            return false;
        }
        
        void ParseInput()
        {
            var lines = File.ReadAllText(INPUTFILE).Split('\n').Where(l => l.Length > 0).ToList();
            for (int l = 0; l < lines.Count; l++)
            {
                if (!_horBlizzards.ContainsKey(l)) _horBlizzards[l] = new List<Blizzard>();
                string line = lines[l];
                for (int i = 0; i < line.Length; i++)
                {
                    if (!_verBlizzards.ContainsKey(i)) _verBlizzards[i] = new List<Blizzard>();
                    if (line[i] == '<' || line[i] == '>' || line[i] == 'v' || line[i] == '^')
                    {
                        Blizzard blizzard = new Blizzard(new Vector2Int(i, l), line[i]);
                        _blizzards.Add(blizzard);
                        if (line[i] == 'v' || line[i] == '^')
                            _verBlizzards[i].Add(blizzard);
                        else 
                            _horBlizzards[l].Add(blizzard);
                    }
                }
            }
            
            _startPos = new Vector2Int(1,0);
            _endPos = new Vector2Int(lines[0].Length-2,lines.Count-1);
            _mapSize = new Vector2Int(lines[0].Length, lines.Count);
        }
    }

    class Blizzard
    {
        public Vector2Int startPos;
        public Vector2Int direction;

        public Blizzard(Vector2Int startPos, char direction)
        {
            switch (direction)
            {
                case '<':
                    this.direction = new Vector2Int(-1, 0);
                    break;
                case '>':
                    this.direction = new Vector2Int(1, 0);
                    break;
                case 'v':
                    this.direction = new Vector2Int(0, 1);
                    break;
                case '^':
                    this.direction = new Vector2Int(0, -1);
                    break;
            }
            this.startPos = startPos;
        }

        public Vector2Int GetPos(int minute, Vector2Int mapSize)
        {
            Vector2Int pos = startPos;
            // for negative directions we: revert the position, calculate as a positive direction and revert pos again
            if (direction.x < 0) pos.x = mapSize.x - 1 - pos.x;
            if (direction.y < 0) pos.y = mapSize.y - 1 - pos.y;
                
                
            if (direction.x != 0)
                pos = new Vector2Int((pos.x - 1 + (minute * Math.Abs(direction.x))) % (mapSize.x-2) + 1, pos.y);
            else
                pos = new Vector2Int(pos.x, (pos.y - 1 + (minute * Math.Abs(direction.y))) % (mapSize.y-2) + 1);

            if (direction.x < 0) pos.x = mapSize.x - 1 - pos.x;
            if (direction.y < 0) pos.y = mapSize.y - 1 - pos.y;
            return pos;
        }
    }
}