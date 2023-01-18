using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2019
{
    internal class Day09 : DayBase
    {
        public Day09() : base(9)
        {
        }

        public override void Solve()
        {
            var i = new IntCodeInterpreter(GetInputLines(false).First(), () => 1, n => Console.WriteLine(n));
            i.Run();
        }

        public override void SolveMain()
        {
            var i = new IntCodeInterpreter(GetInputLines(false).First(), () => 2, n => Console.WriteLine(n));
            i.Run();
        }
    }
}
