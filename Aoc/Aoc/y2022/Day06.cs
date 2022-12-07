using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day06 : DayBase
    {

        public Day06() : base(6)
        {
        }

        public override void Solve()
        {
            const int Window = 14;
            var line = GetInputLines(false).First();
            Console.WriteLine(Enumerable.Range(0, line.Length).First(i => line.Substring(i, Window).Distinct().Count() == Window) + Window);
        }

        public override void SolveMain()
        {
        }
    }
}
