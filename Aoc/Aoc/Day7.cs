using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day7 : DayBase
    {
        public Day7() : base(7)
        {
        }

        public override void Solve()
        {
            var pos = SplitInts(this.GetInputLines(false).First(), ',').ToList();
            var (min, max) = (pos.Min(), pos.Max());
            var optimum = int.MaxValue;
            for (var i = min; i <= max; ++i)
            {
                var candidate = pos.Select(p => Math.Abs(p - i)).Sum();
                if (candidate < optimum)
                {
                    optimum = candidate;
                }
            }
            Console.WriteLine(optimum);
        }

        public override void SolveMain()
        {
            var pos = SplitInts(this.GetInputLines(false).First(), ',').ToList();
            var (min, max) = (pos.Min(), pos.Max());
            var optimum = int.MaxValue;
            for (var i = min; i <= max; ++i)
            {
                var candidate = pos.Select(p =>
                    {
                        var diff = Math.Abs(p - i);
                        return (diff * (diff + 1)) / 2;
                    }
                ).Sum();
                if (candidate < optimum)
                {
                    optimum = candidate;
                }
            }
            Console.WriteLine(optimum);
        }
    }
}
