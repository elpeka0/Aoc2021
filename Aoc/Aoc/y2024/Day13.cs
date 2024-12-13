using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day13 : DayBase
    {
        public Day13() : base(13)
        {
        }

        public override void Solve()
        {
            this.SolveImpl(false);
        }

        private void SolveImpl(bool isMain)
        {
            var l = this.GetInputLines().ToList();

            var re = new Regex(@"(\d+),[^\d]*(\d+)");
            var total = 0L;
            for (var i = 0; i < l.Count; i += 4)
            {
                var (ax, ay) = Parse(l[i]);
                var (bx, by) = Parse(l[i + 1]);
                var (tx, ty) = Parse(l[i + 2]);

                if (isMain)
                {
                    tx += 10000000000000L;
                    ty += 10000000000000L;
                }

                // tx = a*ax + b*bx
                // ty = a*ay + b*by
                // a = (tx - b*bx)/ax
                // ty = ay*(tx - b*bx)/ax + b*by
                // ty*ax = ay*tx - b*bx*ay + b*by*ax
                // ty*ax - ay*tx = b*(by*ax - bx*ay)
                // b = (ty*ax - ay*tx)/(by*ax - bx*ay)

                var num = ty * ax - ay * tx;
                var denom = by * ax - bx * ay;
                if (num % denom != 0)
                {
                    continue;
                }

                var b = num / denom;
                num = tx - b * bx;
                if (num % ax != 0)
                {
                    continue;
                }
                var a = num / ax;
                if (!isMain && (a > 100 || b > 100))
                {
                    continue;
                }
                total += 3 * a + b;
            }

            Console.WriteLine(total);

            (long, long) Parse(string line)
            {
                var match = re.Match(line);
                return (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
            }
        }

        public override void SolveMain()
        {
            SolveImpl(true);
        }
    }
}
