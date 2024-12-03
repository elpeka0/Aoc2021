using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day03 : DayBase
    {
        public Day03() : base(3)
        {
        }

        public override void Solve()
        {
            using var reader = this.OpenInput();
            var all = reader.ReadToEnd();
            var sum = HandleMuls(all);
            Console.WriteLine(sum);
        }

        private static long HandleMuls(string sub)
        {
            var regex = new Regex(@"mul\((\d+),(\d+)\)");
            var matches = regex.Matches(sub);
            var sum = matches.Sum(m => long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value));
            return sum;
        }

        public override void SolveMain()
        {
            using var reader = this.OpenInput();
            var all = reader.ReadToEnd();

            var parts = all.Split("do()");
            var sum = 0L;
            foreach (var p in parts)
            {
                var idx = p.IndexOf("don't()", StringComparison.Ordinal);
                if (idx == -1)
                {
                    sum += HandleMuls(p);
                }
                else
                {
                    sum += HandleMuls(p.Substring(0, idx));
                }
            }
            Console.WriteLine(sum);
        }
    }
}
