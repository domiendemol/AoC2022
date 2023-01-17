using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day22
    {
        private const string INPUTFILE = "day22.txt";
        private const int SIZE = 50;
        
        private static Vector2Int[] DIRECTIONS = 
            {new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)};
        private static Vector2Int UP = new Vector2Int(0, -1);
        private static Vector2Int DOWN = new Vector2Int(0, 1);
        private static Vector2Int LEFT = new Vector2Int(-1, 0);
        private static Vector2Int RIGHT = new Vector2Int(1, 0);
        
        char[,] _map = new char[SIZE*3,SIZE*4]; // x,y
        char[,] _visitedMap = new char[SIZE*3,SIZE*4]; // x,y
        List<string> _instructions = new List<string>();
        private Vector2Int direction;
        
        // part2
        struct Transition
        {
            public Vector2Int fromDirection;
            public Vector2Int toDirection;
            // faces
            public int fromX;
            public int fromY;
            public int toX;
            public int toY;

            public Transition(Vector2Int fromDirection, Vector2Int direction, int fromX, int fromY, int x, int y)
            {
                this.fromDirection = fromDirection;
                toDirection = direction;
                this.fromX = fromX;
                this.fromY = fromY;
                toX = x;
                toY = y;
            }
        }
        List<Transition> _transitions = new List<Transition>();
        
        public void Run()
        {
            ParseInput();
            CreateTransitions();

            // FIND START
            Vector2Int position = new Vector2Int(0,0);
            bool posFound = false;
            for (int y = 0; y < _map.GetLength(1) && !posFound; y++)
            {
                for (int x = 0; x < _map.GetLength(0) && !posFound; x++)
                {
                    if (_map[x, y] == '.')
                    {
                        position = new Vector2Int(x, y);
                        posFound = true;
                    }
                }
            }

            // PART 1
            FollowDirections(position, false);

            // PART 2
            ParseInput();
            FollowDirections(position, true);

            PrintMap();
        }

        private void PrintMap()
        {
            // DEBUG 
            for (int y = 0; y < _map.GetLength(1) ; y++)
            {
                string line = "";
                for (int x = 0; x < _map.GetLength(0) ; x++)
                {
                    line += _visitedMap[x, y] == '\0' ? ' ' : _visitedMap[x, y];
                }
                Console.WriteLine(line);
            }
        }

        private void FollowDirections(Vector2Int position, bool cube)
        {
            direction = new Vector2Int(1, 0);

            foreach (string instruction in _instructions)
            {
                // Console.WriteLine("->" + instruction);
                int moves;
                if (int.TryParse(instruction, out moves))
                {
                    // move
                    for (int i = 0; i < moves; i++)
                    {
                        position = cube ? CubeMove(position) : Move(position, direction);
                        _visitedMap[position.x, position.y] = '@';
                    }

                    // Console.WriteLine(position.x + ", " + position.y);
                }
                else if (instruction.Equals("R"))
                {
                    direction = DIRECTIONS[(Array.IndexOf(DIRECTIONS, direction) + 1) % DIRECTIONS.Length];
                }
                else if (instruction.Equals("L"))
                {
                    int indx = Array.IndexOf(DIRECTIONS, direction) - 1;
                    if (indx < 0) indx += DIRECTIONS.Length;
                    direction = DIRECTIONS[indx % DIRECTIONS.Length];
                }
            }

            Console.WriteLine($"RESULT PART {(cube ? 2 : 1)}: " + (4 * (position.x+1) + (1000 * (position.y+1)) + Array.IndexOf(DIRECTIONS, direction)));
            Console.WriteLine("Should be 10006 for the test, part 2");
        }

        // 1 pos
        Vector2Int Move(Vector2Int currentPos, Vector2Int direction)
        {
            Vector2Int newPos = currentPos;
            do
            {
                newPos = newPos + direction;
                if (newPos.x < 0) newPos.x = _map.GetLength(0)-1;
                if (newPos.y < 0) newPos.y = _map.GetLength(1)-1;
                if (newPos.x >= _map.GetLength(0)) newPos.x = 0;
                if (newPos.y >= _map.GetLength(1)) newPos.y = 0;
            } while (_map[newPos.x, newPos.y] == ' ' || _map[newPos.x, newPos.y] == '\0');

            if (_map[newPos.x, newPos.y] != '#')
                return newPos;
            else
                return currentPos;
        }

        Vector2Int CubeMove(Vector2Int currentPos)
        {
            Vector2Int newPos = currentPos + direction;

                if (TryGetPos(newPos.x, newPos.y, out char c) && (c == '.' || c == '#'))
                {
                    // normal situation
                    return _map[newPos.x, newPos.y] != '#' ? newPos : currentPos;
                }
                else
                {
                    // find transition
                    Transition transition = 
                        _transitions.Single(t => t.fromDirection.Equals(direction) && t.fromX == currentPos.x / SIZE &&
                                                 t.fromY == currentPos.y / SIZE);
                    // find new position
                    bool revert = (transition.toDirection - transition.fromDirection).x != 0 && (transition.toDirection - transition.fromDirection).y != 0;
                    int currX = currentPos.x % SIZE;
                    int currY = currentPos.y % SIZE;
                    if (transition.toDirection == LEFT)
                    {
                        newPos = new Vector2Int(transition.toX * SIZE + SIZE-1, transition.toY * SIZE + (revert ? currX : SIZE-1 - currY));
                    }
                    else if (transition.toDirection == RIGHT)
                    {
                        newPos = new Vector2Int(transition.toX * SIZE, transition.toY * SIZE + (revert ? currX : SIZE-1 - currY));
                    }
                    else if (transition.toDirection == UP)
                    {
                        newPos = new Vector2Int(transition.toX * SIZE + (revert ? currY : currX), transition.toY * SIZE + SIZE-1);
                    }
                    else if (transition.toDirection == DOWN)
                    {
                        newPos = new Vector2Int(transition.toX * SIZE + (revert ? currY : currX), transition.toY * SIZE);
                    }
                    
                    if (_map[newPos.x, newPos.y] != '#')
                    {
                        if (_map[newPos.x, newPos.y] == ' ' || _map[newPos.x, newPos.y] == '\0') Console.WriteLine("AAAÁAAAALLLLLEEEEEERRRRTTTT");
                        direction = transition.toDirection; // TODO sure we can only set direction if we could move on?
                        return newPos;
                    }
                    else {
                        return currentPos;
                    }
                }
        }

        bool TryGetPos(int x, int y, out char c)
        {
            c = ' ';
            if (x < 0) return false;
            if (y < 0) return false;
            if (x >= _map.GetLength(0)) return false;
            if (y >= _map.GetLength(1)) return false;

            c = _map[x, y];
            return true;
        }

        
        void ParseInput()
        {
            _instructions.Clear();
            
            var lines = File.ReadAllText(INPUTFILE).Split('\n').Where(l => l.Length > 0).ToList();
            for (int l = 0; l < lines.Count; l++)
            {
                string line = lines[l];
                if (!line.Contains("R")) 
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        _map[i,l] = line[i];
                        _visitedMap[i,l] = line[i];
                    }
                }
                else
                {
                    string instruction = "";
                    foreach (char c in line)
                    {
                        if (c >= '0' && c <= '9')
                            instruction += c;
                        else if (c == '-')
                            break;
                        else
                        {
                            if (!string.IsNullOrEmpty(instruction)) _instructions.Add(instruction);
                            _instructions.Add(c.ToString());
                            instruction = "";
                        }
                    }
                    if (int.TryParse(instruction, out int a)) _instructions.Add(instruction); // don't forget last instruction
                }
            }
        }

        void CreateTransitions()
        {
            _transitions.Add(new Transition(UP, RIGHT, 1, 0, 0, 3));
            _transitions.Add(new Transition(LEFT, RIGHT, 1, 0, 0, 2));
         
            _transitions.Add(new Transition(UP, UP, 2, 0, 0, 3));
            _transitions.Add(new Transition(RIGHT, LEFT, 2, 0, 1, 2));
            _transitions.Add(new Transition(DOWN, LEFT, 2, 0, 1, 1));
            
            _transitions.Add(new Transition(LEFT, DOWN, 1, 1, 0, 2));
            _transitions.Add(new Transition(RIGHT, UP, 1, 1, 2, 0));
            
            _transitions.Add(new Transition(UP, RIGHT, 0, 2, 1, 1));
            _transitions.Add(new Transition(LEFT, RIGHT, 0,2, 1, 0));
            
            _transitions.Add(new Transition(RIGHT, LEFT, 1,2, 2, 0));
            _transitions.Add(new Transition(DOWN, LEFT, 1,2, 0, 3));
            
            _transitions.Add(new Transition(LEFT, DOWN, 0,3, 1, 0));
            _transitions.Add(new Transition(RIGHT, UP, 0,3, 1, 2));
            _transitions.Add(new Transition(DOWN, DOWN, 0,3, 2,0));
        }
    }
}