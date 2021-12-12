using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day01 : DayBase
    {
        public Day01() : base(1)
        {
        }

        public override void Solve()
        {
            var input = GetInputLines(false).Select(l => int.Parse(l)).ToList();
            var res = input.Skip(1).Zip(input).Count(t => t.Second < t.First);
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var input = GetInputLines(false).Select(l => int.Parse(l)).ToList();
            var sums = input.Zip(input.Skip(1)).Zip(input.Skip(2)).Select(t => t.First.First + t.First.Second + t.Second).ToList();
            var res = sums.Skip(1).Zip(sums).Count(t => t.Second < t.First);
            Console.WriteLine(res);
        }
    }
}
