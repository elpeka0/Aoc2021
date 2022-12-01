using System;
using System.Linq;

namespace Aoc.y2021
{
    public class Day07 : DayBase
    {
        public Day07() : base(7)
        {
        }

        public override void Solve()
        {
            var pos = this.SplitInts(this.GetInputLines(false).First(), ',').ToList();
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
            var pos = this.SplitInts(this.GetInputLines(false).First(), ',').ToList();
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
