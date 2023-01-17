using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day20
    {
        private const string INPUTFILE = "day20.txt";

        public void Run()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 0).ToList();
            List<int> numbers = lines.Select(x => Convert.ToInt32(x)).ToList();
            
            // PART 1
            long total = Mix(1, 1, numbers);
            Console.WriteLine($"RESULT PART 1: {total}");
            
            // PART 2
            long total2 = Mix(10, 811589153, numbers);
            Console.WriteLine($"RESULT PART 2: {total2}");
        }

        long Mix(int amount, long multiplier, List<int> numbers)
        {
            LinkedList<Number> numbersMixed = new LinkedList<Number>();
            int index = 0;
            foreach (var value in numbers)
            {
                // reduce the values:
                // 1. no need to calculate some many, take module numbers.Count
                // 2. add the times we reduced it (x / numbers.Count) to take into account where the number 'passes' itself, we need to add 1 every time
                long newVal = value * multiplier;
                while (Math.Abs(newVal) > numbers.Count)
                    newVal = newVal % numbers.Count + newVal / numbers.Count;
                numbersMixed.AddLast(new Number(index++, newVal, value * multiplier));
            }
            
            Number zeroNumber = null;
            for (int a = 0; a < amount; a++)
            {
                Console.WriteLine("Mix: " + (a+1));
                // loop
                for (index=0; index<numbers.Count; index++)
                {
                    // find the node
                    Number number = numbersMixed.First(x => x.origIndex == index);
                    if (number.value == 0)
                    {
                        zeroNumber = number;
                        continue;
                    }
                    
                    // move!
                    LinkedListNode<Number> n = numbersMixed.FindLast(number);
                    LinkedListNode<Number> next = number.value < 0 ? n.Previous : n.Next;
                    if (next == null) next = number.value < 0 ? numbersMixed.Last : numbersMixed.First;
                    long shifts = Math.Abs(number.value);
                    shifts += (shifts / numbersMixed.Count);
                    for (int i = 0; i < shifts -1; i++)
                    {
                        next = number.value < 0 ? next.Previous : next.Next;
                        if (next == null) next = number.value < 0 ? numbersMixed.Last : numbersMixed.First;
                    }

                    if (n == next) continue;
                    
                    numbersMixed.Remove(number);
                    if (number.value < 0)
                    {
                        if (next == numbersMixed.First)
                            numbersMixed.AddLast(number);
                        else
                            numbersMixed.AddBefore(next, number);
                    }
                    else
                    {
                        if (next == numbersMixed.Last)
                            numbersMixed.AddFirst(number);
                        else
                            numbersMixed.AddAfter(next, number);
                    }
                }
            }
            
            long total = 0;
            LinkedListNode<Number> nxt = numbersMixed.Find(zeroNumber);
            for (int i = 1; i <= 3000; i++)
            {
                nxt = nxt.Next;
                if (nxt == null) nxt = numbersMixed.First;
                if (i % 1000 == 0)
                {
                    total += nxt.Value.origValue;
                    // Console.WriteLine(nxt.Value.origValue);
                }
            }

            return total;
        }
    }

    class Number
    {
        public int origIndex;
        public long value;
        public long origValue;

        public Number(int origIndex, long value, long origValue)
        {
            this.origIndex = origIndex;
            this.origValue = origValue;
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}