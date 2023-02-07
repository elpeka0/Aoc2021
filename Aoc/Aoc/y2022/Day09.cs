using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2022
{
    public class Day09 : DayBase
    {
        
        public Day09() : base(9)
        {
        }

        private IEnumerable<Vector> GetInput()
        {
            foreach (var line in GetInputLines(false))
            {
                var v = int.Parse(line.Substring(2));
                yield return line[0] switch
                {
                    'U' => new Vector(0, -v, 0),
                    'D' => new Vector(0, v, 0),
                    'L' => new Vector(-v, 0, 0),
                    'R' => new Vector(v, 0, 0)
                };
            }
        }

        private Vector MoveTowards(Vector a, Vector b) => new Vector(a.X + Math.Sign(b.X - a.X), a.Y + Math.Sign(b.Y - a.Y), 0);

        private Vector MoveTail(Vector tail, Vector head)
        {
            var diff = head - tail;
            if (Math.Abs(diff.X) > 1 || Math.Abs(diff.Y) > 1)
            {
                return MoveTowards(tail, head);
            }
            return tail;
        }

        public override void Solve()
        {
            var instructions = GetInput().ToList();
            var head = new Vector();
            var tail = new Vector();
            var visited = new HashSet<Vector>();
            visited.Add(tail);

            foreach (var i in instructions)
            {
                var target = head + i;
                while (head != target)
                {
                    head = MoveTowards(head, target);
                    tail = MoveTail(tail, head);
                    visited.Add(tail);
                }
            }
            Console.WriteLine(visited.Count);
        }

        public override void SolveMain()
        {
            var instructions = GetInput().ToList();
            var rope = new Vector[10];
            var visited = new HashSet<Vector>();
            visited.Add(rope[9]);

            foreach (var i in instructions)
            {
                var target = rope[0] + i;
                while (rope[0] != target)
                {
                    rope[0] = MoveTowards(rope[0], target);
                    for (var n = 1; n < rope.Length; ++n)
                    {
                        rope[n] = MoveTail(rope[n], rope[n-1]);
                    }
                    visited.Add(rope[9]);
                }
            }
            Console.WriteLine(visited.Count);
        }
    }
}
