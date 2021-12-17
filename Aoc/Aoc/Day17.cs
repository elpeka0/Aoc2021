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

        private IEnumerable<(int X, int Y)> Shoot(int vx, int vy)
        {
            var x = 0;
            var y = 0;
            while (true)
            {
                x += vx;
                y += vy;
                if (vx > 0)
                {
                    vx -= Math.Sign(x);
                }
                --vy;
                yield return (x, y);
            }
        }

        private IEnumerable<(int X, int Y)> AllPairs()
        {
            var x = 0;
            var y = 0;
            var down = false;
            while (true)
            {
                if (x == 0 && !down)
                {
                    ++y;
                    down = true;
                }
                else if (y == 0 && down)
                {
                    ++x;
                    down = false;
                }
                else if (down)
                {
                    ++x;
                    --y;
                }
                else
                {
                    ++y;
                    --x;
                }

                yield return (x, y);
                yield return (x, -y);
            }
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

        private IEnumerable<(int vx, int vy, int localmax)> TexasMode()
        {
            var (xlow, ylow, xhigh, yhigh) = this.GetInput();
            
            foreach (var (vx, vy) in this.AllPairs())
            {
                var localmax = int.MinValue;
                foreach (var p in this.Shoot(vx, vy))
                {
                    if (p.Y > localmax)
                    {
                        localmax = p.Y;
                    }

                    if (p.X >= xlow && p.X <= xhigh && p.Y >= ylow && p.Y <= yhigh)
                    {
                        yield return (vx, vy, localmax);
                        break;
                    }

                    if (p.Y < ylow)
                    {
                        break;
                    }
                }
            }
        }

        public override void Solve()
        {
            var maxy = int.MinValue;
            foreach (var (_, _, localmax) in this.TexasMode())
            {
                if (localmax > maxy)
                {
                    maxy = localmax;
                    Console.WriteLine(maxy);
                }
            }
        }

        public override void SolveMain()
        {
            var points = new HashSet<(int, int)>();
            foreach (var (vx, vy, _) in this.TexasMode())
            {
                points.Add((vx, vy));
                Console.WriteLine(points.Count);
            }
        }
    }
}
