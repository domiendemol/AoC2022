using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day13
    {
        public void Run()
        {
            var lines = File.ReadAllText("day13.txt").Trim().Split('\n').Where(l => l.Length > 1).ToList();
            List<Packet[]> pairs = new List<Packet[]>();
            Packet[] ps = new Packet[2];
            for (int i = 0; i < lines.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ps = new Packet[2];
                    pairs.Add(ps);
                }
                ps[i % 2] = new Packet(lines[i]); //.Substring(1, lines[i].Length-2));
            }

            int sum = 0;
            for (int i=0; i<pairs.Count; i++)
            {
                // compare
                Packet[] pair = pairs[i];
                int r = pair[0].Compare(pair[1]);
                // Console.WriteLine($"{pair[0]} vs {pair[1]} --> {r}");
                if (r > 0) sum += i + 1;
            }
            
            Console.WriteLine($"RESULT PART 1: {sum}");

            List<Packet> packets = new List<Packet>();
            Packet p2 = new Packet("[[2]]");
            Packet p6 = new Packet("[[6]]");
            int p2Index = 0, p6Index = 0;
            foreach (Packet[] pair in pairs)
            {
                if (pair[0].Compare(p2) > 0) p2Index++;
                if (pair[1].Compare(p2) > 0) p2Index++;
                if (pair[0].Compare(p6) > 0) p6Index++;
                if (pair[1].Compare(p6) > 0) p6Index++;
            }

            Console.WriteLine($"RESULT PART 1: {((p2Index + 1) * (p6Index + 2))}");
            
            // Alternatively: sort all the packets, including the [[2]] and [[6]]
            // packets.Sort(Comparer<Packet>.Default); 
        }
    }

    class Packet
    {
        private readonly Value value;
        public Packet(string data)
        {
            value = new Value(data);
        }

        public int Compare(Packet p2)
        {
            return value.Compare(p2.value);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class Value
    {
        public enum Type { INTEGER, LIST }

        public Type type;
        public int intVal = -1;
        public List<Value> listVal = new List<Value>();

        public Value(string data)
        {
            if (data.Length > 0)
                ParseValues(data);
            else
                type = Type.LIST;
        }

        public override string ToString()
        {
            if (type == Type.INTEGER) return "" + intVal;
            
            string r = "[";
            foreach (var v in listVal) r+=v + " ";
            return r + "]";
        }

        private void ParseValues(string data)
        {
            if (!data.Contains("[") && !data.Contains(","))
            {
                type = Type.INTEGER;
                intVal = Int32.Parse(data);
            }
            else
            {
                type = Type.LIST;
                
                // split on commas on top level, so not in sublists
                foreach (string part in Split(data.Substring(1, data.Length-2)))
                {
                    Console.WriteLine(part);
                    listVal.Add(new Value(part));
                }
            }
        }

        // Split by comma, on lower level
        List<string> Split(string data)
        {
            int level = 0;
            string part = "";
            List<string> parts = new List<string>();
            foreach (char c in data)
            {
                if (c == '[') level++;
                else if (c == ']') level--;
                if (c == ',' && level == 0)
                {
                    parts.Add(part);
                    part = "";
                }
                else part += c;
            }
            parts.Add(part);

            return parts;
        }

        // positive is smaller
        // 0 if we got to keep checking
        public int Compare(Value v2)
        {
            if (type == Type.INTEGER && v2.type == Type.INTEGER)
            {
                return v2.intVal - intVal;
            }
            else if (type == Type.LIST && v2.type == Type.LIST)
            {
                for (int i=0; i<listVal.Count; i++)
                {
                    if (i >= v2.listVal.Count)
                    {
                        Console.WriteLine($"{this} - Right side ran out of items, so inputs are NOT in the right order");
                        return -1;
                    }
                    
                    int r = listVal[i].Compare(v2.listVal[i]);
                    if (r != 0) return r;
                }
                // ran out of left values
                if (listVal.Count < v2.listVal.Count)  Console.WriteLine($"{this} - Left side ran out of items, so inputs are in the right order");

                return (listVal.Count < v2.listVal.Count) ? 1 : 0; 
            }
            else
            {
                if (type == Type.INTEGER)
                {
                    type = Type.LIST;
                    listVal = new List<Value> {new Value("" + intVal)};
                }
                if (v2.type == Type.INTEGER)
                {
                    v2.type = Type.LIST;
                    v2.listVal = new List<Value> {new Value("" + v2.intVal)};
                }

                return Compare(v2); // do again with lists
            }
        }
    }
    

}