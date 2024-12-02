using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day02 : DayBase
    {
        public Day02() : base(2)
        {
        }

        private List<List<int>> Read()
        {
            return GetInputLines(false).Select(l => SplitInts(l, ' ').ToList()).ToList();
        }

        private static bool IsSafe(List<int> d)
        {
            var r = d.Zip(d.Skip(1), (a, b) => a - b).ToList();
            return r.All(x => Math.Sign(x) == Math.Sign(r[0]) && Math.Abs(x) is >= 1 and <= 3);
        }

        public override void Solve()
        {
            var c = this.Read().Count(IsSafe);
            Console.WriteLine(c);
        }

        public override void SolveMain()
        {
            var c = this.Read().Count(d => IsSafe(d)
                    || Enumerable.Range(0, d.Count).Any(n => IsSafe(d.Take(n).Concat(d.Skip(n + 1)).ToList())));
            Console.WriteLine(c);
        }
    }
}
