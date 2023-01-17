using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC2022
{
    public class Day25
    {
        private const string INPUTFILE = "day25.txt";
        private char[] TRANSLATION = {'=','-','0','1','2'};
        List<string> snafus = new List<string>();

        public void Run()
        {
            ParseInput();

            long sum = 0;
            foreach (string snafu in snafus)
            {
                sum += ToDecimal(snafu);
            }
            Console.WriteLine($"RESULT PART 1: {sum} -> {ToSnafu(sum)}");
        }

        long ToDecimal(string snafu)
        {
            long result = 0;
            for (int i = snafu.Length - 1; i >= 0; i--)
            {
                result += (Array.IndexOf(TRANSLATION, snafu[i]) - 2) * (long) Math.Pow(5, snafu.Length - 1 - i);
            }

            Console.WriteLine(snafu + " -> " + result);
            Console.WriteLine(ToSnafu(result));
            return result;
        }

        string ToSnafu(long dec)
        {
            List<char> snafu = new List<char>();
            
            int i = 0;
            int add = 0;
            while (dec > 0)
            {
                long rem = dec % (long) Math.Pow(5, i + 1);
                double d = rem / Math.Pow(5, i);
                d += add;
                
                snafu.Add(TRANSLATION[((int) d + 2) % 5]);

                dec -= rem;
                add = (d >= 3) ? 1 : 0; // negative number, add one to the rest
                i++;
            }
            if (add == 1) snafu.Add('1');
  
            snafu.Reverse();
            return string.Join("", snafu);
        }
        
        void ParseInput()
        {
            var lines = File.ReadAllText(INPUTFILE).Split('\n').Where(l => l.Length > 0).ToList();
            foreach (var nr in lines)
            {
                snafus.Add(nr);
            }
        }
    }
}