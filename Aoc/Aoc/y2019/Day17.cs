using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2019
{
    internal class Day17 : DayBase
    {
        public Day17() : base(17)
        {
        }

        public override void Solve()
        {
            var field = new Dictionary<Vector, char>();
            var x = 0;
            var y = 0;
            var interpreter = new IntCodeInterpreter(this.GetInputLines(false).First(), null, l => 
            {
                if (l == 10)
                {
                    (x, y) = (0, y + 1);
                }
                else
                {
                    field[new Vector(x, y)] = (char)l;
                    ++x;
                }
            });
            interpreter.Run();
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
