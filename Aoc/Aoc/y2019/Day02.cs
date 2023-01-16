using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2019
{
    internal class Day02 : DayBase
    {
        public Day02() : base(2)
        {
        }

        public override void Solve()
        {
            var i = new IntCodeInterpreter(GetInputLines(false).First(), null, null);
            i[1] = 12;
            i[2] = 2;
            i.Run();
            Console.WriteLine(i[0]);
        }

        public override void SolveMain()
        {
            var i = new IntCodeInterpreter(GetInputLines(false).First(), null, null);
            int noun;
            int verb = 0;
            for (noun = 0; noun <= 99; ++noun)
            {
                for (verb = 0; verb <= 99; ++verb)
                {
                    i.Reset();
                    i[1] = noun;
                    i[2] = verb;
                    i.Run();
                    if (i[0] == 19690720)
                    {
                        Console.WriteLine(100*noun + verb);
                        return;
                    }
                }
            }
        }
    }
}
