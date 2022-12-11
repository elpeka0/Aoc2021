using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day10 : DayBase
    {
        
        public Day10() : base(10)
        {
        }

        private IEnumerable<int> GetInput()
        {
            foreach (var line in GetInputLines(false))
            {
                if (line.StartsWith("addx"))
                {
                    yield return 0;
                    yield return int.Parse(line.Split()[1]);
                }
                else
                {
                    yield return 0;
                }
            }
        }

        public override void Solve()
        {
            var sum = 0;
            var x = 1;
            var c = 0;

            foreach (var a in GetInput())
            {
                if ((c + 1 - 20) % 40 == 0)
                {
                    sum += x * (c + 1);
                }
                x += a;
                ++c;
            }
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var x = 1;
            var c = 0;

            var buffer = new StringBuilder();

            foreach (var a in GetInput())
            {
                var px = c % 40;
                if (px == 0)
                {
                    buffer.AppendLine();
                }
                Console.WriteLine($"{c + 1}: {px} vs. {x}: {buffer}");
                if (Math.Abs(x - px) < 2)
                {
                    buffer.Append('#');
                }
                else
                {
                    buffer.Append('.');
                }
                Console.WriteLine($"{c + 1}: {px} vs. {x}: {buffer}");
                x += a;
                ++c;
            }
        }
    }
}
