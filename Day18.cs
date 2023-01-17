using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day18
    {
        private const string INPUTFILE = "day18.txt";
        
        List<Cube> _cubes = new List<Cube>();

        private Cube[,,] _map = new Cube[32,32,32];
        
        public void Run()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 1).ToList();

            foreach (string line in lines)
            {
                var parts = line.Split(',');
                int x = Int32.Parse(parts[0]);
                int y = Int32.Parse(parts[1]); 
                int z = Int32.Parse(parts[2]);
                _cubes.Add(new Cube(x,y,z));
                _map[x, y, z] = _cubes.Last();
            }

            Part1();
            Part2();
        }

        void Part1()
        {
            int free = 0;
            foreach (Cube cube in _cubes)
            {
                // check all directions
                if (_map[cube.x+1, cube.y, cube.z] == null) free++;
                if (cube.x-1 < 0 || _map[cube.x-1, cube.y, cube.z] == null) free++;
                if (_map[cube.x, cube.y+1, cube.z] == null) free++;
                if (cube.y-1 < 0 || _map[cube.x, cube.y-1, cube.z] == null) free++;
                if (_map[cube.x, cube.y, cube.z+1] == null) free++;
                if (cube.z-1 < 0 || _map[cube.x, cube.y, cube.z-1] == null) free++;
            }
            
            Console.WriteLine($"RESULT PART 1: {free}");
        }

        void Part2()
        {
            // Flood fill the whole map
            FloodFill(0, 0, 0);
            
            // Iterate over the cubes, count only the sides that touch a steam cube
            int free = 0;
            foreach (Cube cube in _cubes)
            {
                // check all directions
                if (IsSteamed(cube.x+1, cube.y, cube.z)) free++;
                if (IsSteamed(cube.x-1, cube.y, cube.z)) free++;
                if (IsSteamed(cube.x, cube.y+1, cube.z)) free++;
                if (IsSteamed(cube.x, cube.y-1, cube.z)) free++;
                if (IsSteamed(cube.x, cube.y, cube.z+1)) free++;
                if (IsSteamed(cube.x, cube.y, cube.z-1)) free++;
            }
            
            Console.WriteLine($"RESULT PART 2: {free}");
        }
        
        // FloodFill function
        void FloodFill(int x, int y, int z)
        {
            List<Tuple<int,int, int>> queue = new List<Tuple<int,int,int>>();
  
            // Append the position of starting
            // pixel of the component
            queue.Add(new Tuple<int,int, int>(x, y, z));
  
            _map[x,y, z] = new Cube(x, y, z, true);
  
            // While the queue is not empty i.e. the
            // whole component having prevC color
            // is not colored with newC color
            while(queue.Count > 0)
            {
                // Dequeue the front node
                Tuple<int,int,int> currPixel = queue[queue.Count - 1];
                queue.RemoveAt(queue.Count - 1);
  
                int posX = currPixel.Item1;
                int posY = currPixel.Item2;
                int posZ = currPixel.Item3;
  
                // Check if the adjacent
                // pixels are valid
                if(IsFree(posX + 1, posY, posZ))
                {
                    // if valid and enqueue
                    _map[posX + 1, posY, posZ] = new Cube(posX + 1, posY, posZ, true);
                    queue.Add(new Tuple<int,int,int>(posX + 1, posY, posZ));
                }
                if(IsFree(posX-1, posY, posZ ))
                {
                    _map[posX - 1, posY, posZ] = new Cube(posX - 1, posY, posZ, true);
                    queue.Add(new Tuple<int,int,int>(posX - 1, posY, posZ));
                }
                if(IsFree(posX, posY + 1, posZ ))
                {
                    _map[posX, posY + 1, posZ] = new Cube(posX, posY+ 1, posZ, true);
                    queue.Add(new Tuple<int,int,int>(posX, posY + 1, posZ));
                }
                if(IsFree(posX, posY-1, posZ ))
                {
                    _map[posX, posY - 1, posZ] = new Cube(posX, posY- 1, posZ, true);
                    queue.Add(new Tuple<int,int,int>(posX, posY - 1, posZ));
                }
                if(IsFree(posX, posY,  posZ +1))
                {
                    _map[posX, posY, posZ+1] = new Cube(posX, posY, posZ+1, true);
                    queue.Add(new Tuple<int,int,int>(posX, posY, posZ+1));
                }
                if(IsFree(posX, posY,  posZ -1))
                {
                    _map[posX, posY, posZ-1] = new Cube(posX, posY, posZ-1, true);
                    queue.Add(new Tuple<int,int,int>(posX, posY, posZ-1));
                }
            }
        }
        
        // check for empty voxel
        bool IsFree(int x, int y, int z)
        {
            return ((x >= 0 && y >= 0 & z >= 0) && (x < 32 && y < 32 & z < 32) && _map[x, y, z] == null);
        }
        
        // check for steam voxel
        bool IsSteamed(int x, int y, int z)
        {
            return (x < 0 || y < 0 || z < 0 || _map[x, y, z] != null && _map[x, y, z].steam);
        }
    }

    class Cube
    {
        public int x, y, z;
        public bool steam; // rock if false

        public Cube(int x, int y, int z, bool steam = false)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.steam = steam;
        }
    }
}