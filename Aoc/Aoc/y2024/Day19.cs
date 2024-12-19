using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day19 : DayBase
    {
        public Day19() : base(19)
        {
        }

        public override void Solve()
        {
            var lines = GetInputLines().ToList();
            var patterns = lines[0].Split(", ");
            var re = new Regex("^(" + string.Join("|", patterns) + ")+$");

            var res = lines.Skip(2).Count(re.IsMatch);
            Console.WriteLine(res);
        }

        private long CountMatches(List<string> patterns, string line, Dictionary<string, long> cache)
        {
            if (line.Length == 0)
            {
                return 1;
            }

            if (cache.TryGetValue(line, out var n))
            {
                return n;
            }

            var sum = 0L;
            foreach (var p in patterns)
            {
                if (line.StartsWith(p))
                {
                    sum += CountMatches(patterns, line.Substring(p.Length), cache);
                }
            }
            cache[line] = sum;
            return sum;
        }

        public override void SolveMain()
        {
            var lines = GetInputLines().ToList();
            var patterns = lines[0].Split(", ").ToList();
            lines = lines.Skip(2).ToList();

            var res = 0L;
            var cache = new Dictionary<string, long>();
            foreach (var line in lines)
            {
                var n = CountMatches(patterns, line, cache);
                res += n;
            }
            Console.WriteLine(res);
        }
    }
}
