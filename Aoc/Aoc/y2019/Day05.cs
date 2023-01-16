using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2019
{
    internal class Day05 : DayBase
    {
        public Day05() : base(5)
        {
        }

        public override void Solve()
        {
            var i = new IntCodeInterpreter(GetInputLines(false).First(), () => 1, v => Console.WriteLine(v));
            i.Run();
        }

        public override void SolveMain()
        {
            var i = new IntCodeInterpreter(GetInputLines(false).First(), () => 5, v => Console.WriteLine(v));
            i.Run();
        }
    }
}
