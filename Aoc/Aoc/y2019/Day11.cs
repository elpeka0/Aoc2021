using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2019
{
    internal class Day11 : DayBase
    {
        public Day11() : base(11)
        {
        }

        public override void Solve()
        {
            var field = new Dictionary<Vector, long>();
            this.RunRobot(field);
            Console.WriteLine(field.Count);
        }

        private void RunRobot(Dictionary<Vector, long> field)
        {
            var pos = new Vector();
            var v = new Vector(0, 1);

            var a = new Channel();
            var b = new Channel();

            var i = new IntCodeInterpreter(this.GetInputLines(false).First(), a.Receive, b.Send);

            var t = Task.Run(i.Run);

            while (!t.IsCompleted)
            {
                a.Send(field.TryGetValue(pos, out var p) ? p : 0);

                var l = b.ReceiveUnlessCompleted(t);
                if (l == null)
                {
                    break;
                }

                field[pos] = l.Value;

                if (b.Receive() == 0)
                {
                    v = TransformMatrix.Rotate(new Vector(), new Vector(0, 0, 1), 1) * v;
                }
                else
                {
                    v = TransformMatrix.Rotate(new Vector(), new Vector(0, 0, 1), 3) * v;
                }

                pos += v;
            }
        }

        public override void SolveMain()
        {
            var field = new Dictionary<Vector, long>();
            field[new Vector(0, 0)] = 1;
            this.RunRobot(field);

            var minx = field.Keys.Min(v => v.X);
            var miny = field.Keys.Min(v => v.Y);
            var maxx = field.Keys.Max(v => v.X);
            var maxy = field.Keys.Max(v => v.Y);

            for (var y = maxy; y >= miny; y--)
            {
                for (var x = minx; x <= maxx; x++)
                {
                    Console.Write(field.TryGetValue(new Vector(x, y), out var v) && v != 0 ? '#' : '.');
                }
                Console.WriteLine();
            }
        }
    }
}
