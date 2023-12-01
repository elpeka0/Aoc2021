using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day01 : DayBase
    {
        private readonly Dictionary<string, int> spelled = new()
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9
        };

        public Day01() : base(1)
        {
        }

        public override void Solve()
        {
            var sum = 0;
            foreach (var line in GetInputLines(false))
            {
                var f = line.First(char.IsDigit);
                var l = line.Last(char.IsDigit);
                sum += (f - '0') * 10 + (l - '0');
            }
            Console.WriteLine(sum);
        }

        private IEnumerable<int> Indices(string haystack, string needle)
        {
            var idx = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            while (idx >= 0)
            {
                yield return idx;
                idx = haystack.IndexOf(needle, idx + needle.Length, StringComparison.OrdinalIgnoreCase);
            }
        }

        private IEnumerable<int> Digits(string s)
        {
            var chars = s
                .Select((c, i) => (Char: c, Index: i))
                .Where(t => char.IsDigit(t.Char))
                .Select(t => (Number: t.Char - '0', t.Index));
            var full = this.spelled.SelectMany(kv => Indices(s, kv.Key).Select(i => (Number: kv.Value, Index: i)));
            return chars.Concat(full).OrderBy(t => t.Index).Select(t => t.Number);
        }

        public override void SolveMain()
        {
            var sum = 0;
            foreach (var line in GetInputLines(false))
            {
                var nrs = Digits(line).ToList();
                var f = nrs.First();
                var l = nrs.Last();
                sum += 10 * f + l;
            }
            Console.WriteLine(sum);
        }
    }
}
