using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2025
{
    internal class Day01 : DayBase
    {
        public Day01() : base(1)
        {
        }

        public override void Solve()
        {
            var lines = GetInputLines();
            var values = lines.Select(l => int.Parse(l.Substring(1)) * l[0] switch
            {
                'L' => -1,
                'R' => 1
            }).ToList();
            var sum = 50;
            var cnt = 0;
            foreach (var mul in values)
            {
                var next = sum + mul;
                if (next > 99)
                {
                    cnt += next / 100;
                }
                else if(next <= 0)
                {
                    cnt += Math.Abs(next) / 100 + (sum == 0 ? 0 : 1);
                }
                Console.WriteLine($"{sum} + {mul} = {next} => {next % 100}, cnt={cnt}");
                sum = next % 100;
                if (sum < 0)
                {
                    sum += 100;
                }
            }
            Console.WriteLine(cnt);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
