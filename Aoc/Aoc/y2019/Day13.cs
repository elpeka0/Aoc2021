using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoc.y2019
{
    internal class Day13 : DayBase
    {
        public Day13() : base(13)
        {
        }

        public override void Solve()
        {
            var a = new Channel();
            var b = new Channel();

            var i = new IntCodeInterpreter(this.GetInputLines(false).First(), a.Receive, b.Send);
            var t = Task.Run(i.Run);

            var field = new Dictionary<Vector, long>();
            while (!t.IsCompleted)
            {
                var x = b.ReceiveUnlessCompleted(t);
                if (x == null)
                {
                    break;
                }

                var y = b.Receive();
                var tile = b.Receive();
                field[new((int)x.Value, (int)y)] = tile;
            }

            Console.WriteLine(field.Values.Count(v => v == 2));
        }

        public override void SolveMain()
        {   
            var score = 0L;
            var field = new Dictionary<Vector, long>();
            var i = new IntCodeInterpreter(this.GetInputLines(false).First(), () => 
            {
                Vector? paddlePos = null;
                Vector? ballPos = null;
                
                foreach (var kv in field)
                {
                    if (kv.Value == 3)
                    {
                        paddlePos = kv.Key;
                    }

                    if (kv.Value == 4)
                    {
                        ballPos = kv.Key;
                    }
                }
                
                if (paddlePos != null && ballPos != null)
                {
                    return Math.Sign(ballPos.Value.X - paddlePos.Value.X);
                }

                return 0L;
            }, IntCodeInterpreter.MakeBatch(3, l =>
            {
                if (l[0] < 0)
                {
                    score = l[2];
                }
                else
                {
                    field[new((int)l[0], (int)l[1])] = l[2];
                    // Visualize(field, score);
                }
            }));
            i[0] = 2;
            i.Run();
            Console.WriteLine(score);
        }

        private void Visualize(Dictionary<Vector, long> field, long score)
        {
            Console.SetCursorPosition(0, 0);
            var minx = field.Keys.Min(v => v.X);
            var miny = field.Keys.Min(v => v.Y);
            var maxx = field.Keys.Max(v => v.X);
            var maxy = field.Keys.Max(v => v.Y);

            Console.WriteLine($"Score: {score}");
            for (var y = miny; y <= maxy; y++)
            {
                for (var x = minx; x <= maxx; x++)
                {
                    field.TryGetValue(new Vector(x, y), out var v);
                    Console.Write(v switch
                    {
                        1 => '#',
                        2 => 'X',
                        3 => '_',
                        4 => 'O',
                        _ => ' '
                    });
                }
                Console.WriteLine();
            }
        }
    }
}
