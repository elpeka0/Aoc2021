using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day17 : DayBase
    {
        public Day17() : base(17)
        {
        }

        private (int XLow, int YLow, int XHigh, int YHigh) GetInput()
        {
            var l = this.GetInputLines(false).First();
            l = l.Replace("target area: x=", string.Empty).Replace(" y=", string.Empty);
            var parts = l.Split(',');
            var xpart = parts[0].Split("..");
            var ypart = parts[1].Split("..");
            return (int.Parse(xpart[0]), int.Parse(ypart[0]), int.Parse(xpart[1]), int.Parse(ypart[1]));
        }

        public override void Solve()
        {
            //var maxy = int.MinValue;
            //foreach (var (_, _, localmax) in this.TexasMode())
            //{
            //    if (localmax > maxy)
            //    {
            //        maxy = localmax;
            //        Console.WriteLine(maxy);
            //    }
            //}
        }

        private int Triangle(int n) => n * (n + 1) / 2;

        private int SeedX(int t, int x) => t + (x - this.Triangle(t)) / t;

        private int SeedY(int t, int y) => (y + this.Triangle(t - 1)) / t;

        private bool CanHitX(int t, int x) => (x - this.Triangle(t)) % t == 0;

        private bool CanHitY(int t, int y) => (y - this.Triangle(t - 1)) % t == 0;

        public override void SolveMain()
        {
            var (xlow, ylow, xhigh, yhigh) = this.GetInput();

            var seen = new HashSet<(int, int)>();
            for (int x = xlow; x <= xhigh; ++x)
            {
                for (int t = 1;this.Triangle(t) <= x; ++t)
                {
                    if (this.CanHitX(t, x))
                    {
                        var vx = this.SeedX(t, x);
                        for (int y = ylow; y <= yhigh; ++y)
                        {
                            if (this.CanHitY(t, y))
                            {
                                var vy = this.SeedY(t, y);
                                seen.Add((vx, vy));
                            }
                            if (x == this.Triangle(t))
                            {
                                // Stop on x
                                for (int s = t + 1;; ++s)
                                {
                                    if (this.CanHitY(s, y))
                                    {
                                        var vy = this.SeedY(s, y);
                                        seen.Add((vx, vy));
                                    }
                                    if (this.SeedY(s, y) > Math.Abs(y))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(seen.Count);
        }
    }
}
