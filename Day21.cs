using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2022
{
    public class Day21
    {
        private const string INPUTFILE = "day21.txt";

        List<Monkey> _monkeys = new List<Monkey>();
        private long _humnStart = -1;
        
        public void Run()
        {
            var lines = File.ReadAllText(INPUTFILE).Trim().Split('\n').Where(l => l.Length > 0).ToList();

            ParseInput(lines);

            Monkey root = _monkeys.Find(x => x.name.Equals("root"));
            foreach (Monkey monkey in _monkeys.Where(x => x.val > 0))
            {
                // find monkeys having this in the "calc" part
                PropagateValue(monkey);
            }
            
            Console.WriteLine($"RESULT PART 1: {root.val}");
            
            // PART 2:
            // either brute force it: trying solution of part 1 with a range of inputs for "humn"
            // or try to be smart and write out the whole expression
            // or invert the whole tree so 'humn' is at the route

            // BINARY SEARCHING because the function turns out to be "monotonic"
            long minNum = 1;
            long maxNum = 33252275935370;
            while (minNum <=maxNum) {
                long mid = (minNum + maxNum) / 2;
                Console.WriteLine($"{mid}");

                ParseInput(lines);
                root = _monkeys.Find(x => x.name.Equals("root"));
                root.calc = root.calc.Replace("+", "=");
                Monkey humn = _monkeys.Find(x => x.name.Equals("humn"));
                humn.val = mid;
                foreach (Monkey monkey in _monkeys.Where(x => x.calced))
                {
                    // find monkeys having this in the "calc" part
                    PropagateValue(monkey);
                }
                
                if (_humnStart == 0) {
                    Console.WriteLine($"{mid} => {_humnStart}");
                    return;
                } else if (_humnStart<0)
                {
                    maxNum = mid - 1; // revert these two for the test inputs
                }else
                {
                    minNum = mid + 1;
                }
            }
        }

        private void PropagateValue(Monkey monkey)
        {
            // Console.WriteLine(monkey.name);
            foreach (Monkey tocalcMonkey in _monkeys.Where(x => !string.IsNullOrEmpty(x.calc) && x.calc.Contains(monkey.name)))
            {
                tocalcMonkey.calc = tocalcMonkey.calc.Replace(monkey.name, ""+monkey.val);
                if (!Regex.Match(tocalcMonkey.calc, @"[a-z][a-z][a-z][a-z]").Success)
                {
                    tocalcMonkey.val = Eval(tocalcMonkey.calc);
                    tocalcMonkey.calced = true;
                    PropagateValue(tocalcMonkey);
                }
            }
        }

        private void ParseInput(List<string> lines)
        {
            _monkeys.Clear();
            foreach (string line in lines)
            {
                Match match = Regex.Match(line, @"(?<name>\w\w\w\w): (?<val>\d+)");
                if (match.Success)
                {
                    _monkeys.Add(new Monkey(match.Groups["name"].Value, Convert.ToInt64(match.Groups["val"].Value), null));
                }
                else
                {
                    match = Regex.Match(line, @"(?<name>\w\w\w\w): (?<calc>.+)");
                    _monkeys.Add(new Monkey(match.Groups["name"].Value, 0, match.Groups["calc"].Value));
                }
            }
        }
        
        long Eval(String expression)
        {
            Match match = Regex.Match(expression, @"(?<p1>-*\d+) (?<op>[=+\-/*]) (?<p2>-*\d+)");
            long p1 = Int64.Parse(match.Groups["p1"].Value);
            long p2 = Int64.Parse(match.Groups["p2"].Value);
            char op = match.Groups["op"].Value[0];
            switch (op)
            {
                case '*':
                    return p1 * p2;
                case '+':
                    return p1 + p2;
                case '-':
                    return p1 - p2;
                case '/':
                    return p1 / p2;
                case '=':
                    _humnStart = p1 - p2;
                    break;
            }
            // System.Data.DataTable table = new System.Data.DataTable();
            // return Convert.ToDouble(table.Compute(expression, String.Empty));
            
            return 0;
        }
    }

    class Monkey
    {
        public long val;
        public string calc;
        public string name;
        public bool calced = false;

        public Monkey(string name, long val, string calc)
        {
            this.val = val;
            this.calc = calc;
            this.name = name;
            if (this.val > 0) calced = true;
        }
    }
}