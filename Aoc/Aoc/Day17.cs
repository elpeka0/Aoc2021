using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day17 : DayBase
    {
        public Day17() : base(17)
        {
        }

        private (int XLow, int YLow, int XHigh, int YHigh) GetInput()
        {
            var l = GetInputLines(false).First();
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

        private int SeedX(int t, int x) => t + (x - Triangle(t)) / t;

        private int SeedY(int t, int y) => (y + Triangle(t - 1)) / t;

        private bool CanHitX(int t, int x) => (x - Triangle(t)) % t == 0;

        private bool CanHitY(int t, int y) => (y - Triangle(t - 1)) % t == 0;

        public override void SolveMain()
        {
            var (xlow, ylow, xhigh, yhigh) = this.GetInput();

            var seen = new HashSet<(int, int)>();
            for (int x = xlow; x <= xhigh; ++x)
            {
                for (int t = 1;Triangle(t) <= x; ++t)
                {
                    if (CanHitX(t, x))
                    {
                        var vx = SeedX(t, x);
                        for (int y = ylow; y <= yhigh; ++y)
                        {
                            if (CanHitY(t, y))
                            {
                                var vy = SeedY(t, y);
                                seen.Add((vx, vy));
                            }
                            if (x == Triangle(t))
                            {
                                // Stop on x
                                for (int s = t + 1;; ++s)
                                {
                                    if (CanHitY(s, y))
                                    {
                                        var vy = SeedY(s, y);
                                        seen.Add((vx, vy));
                                    }
                                    if (SeedY(s, y) > Math.Abs(y))
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
