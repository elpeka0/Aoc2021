using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2022
{
    public class Day23 : DayBase
    {
        public Day23() : base(23)
        {
        }

        private record Point(int X, int Y);

        private HashSet<Point> GetInput()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            return grid.Indexes().Where(i => grid[i.X, i.Y] == '#').Select(i => new Point(i.X, i.Y)).ToHashSet();
        }

        private IEnumerable<Func<Point, Point[]>> Checks()
        {
            yield return p => new[]
            {
                new Point(p.X - 1, p.Y - 1),
                new Point(p.X, p.Y - 1),
                new Point(p.X + 1, p.Y - 1)
            };
            yield return p => new[]
            {
                new Point(p.X - 1, p.Y + 1),
                new Point(p.X, p.Y + 1),
                new Point(p.X + 1, p.Y + 1)
            };
            yield return p => new[]
            {
                new Point(p.X - 1, p.Y - 1),
                new Point(p.X - 1, p.Y),
                new Point(p.X - 1, p.Y + 1)
            };
            yield return p => new[]
            {
                new Point(p.X + 1, p.Y - 1),
                new Point(p.X + 1, p.Y),
                new Point(p.X + 1, p.Y + 1)
            };
        }

        private Point Move(Point p, HashSet<Point> others, IEnumerable<Func<Point, Point[]>> q)
        {
            if (q.All(c => c(p).All(x => !others.Contains(x))))
            {
                return p;
            }

            foreach (var check in q)
            {
                var o = check(p);
                if (o.All(x => !others.Contains(x)))
                {
                    return o[1];
                }
            }
            return p;
        }

        private void Render(HashSet<Point> points)
        {
            var minx = points.Min(s => s.X);
            var maxx = points.Max(s => s.X);
            var miny = points.Min(s => s.Y);
            var maxy = points.Max(s => s.Y);

            for (int y = miny; y <= maxy; ++y)
            {
                for (int x = minx; x <= maxx; ++x)
                {
                    Console.Write(points.Contains(new Point(x, y)) ? '#' : '.');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private void Cycle<T>(Queue<T> q)
        {
            q.Enqueue(q.Dequeue());
        }

        private (HashSet<Point> State, int Count) Run(int maxRoundNumber)
        {
            var state = GetInput();
            var q = new Queue<Func<Point, Point[]>>(Checks());

            int i;
            for (i = 0; i < maxRoundNumber; i++)
            {
                var newState = state
                    .GroupBy(p => Move(p, state, q))
                    .SelectMany(g => g.Count() == 1 ? new[] { g.Key } : (IEnumerable<Point>)g)
                    .ToHashSet();
                if (newState.All(x => state.Contains(x)))
                {
                    break;
                }
                state = newState;
                Cycle(q);
            }
            return (state, i);
        }

        public override void Solve()
        {
            var (state, _) = Run(10);
            var minx = state.Min(s => s.X);
            var maxx = state.Max(s => s.X);
            var miny = state.Min(s => s.Y);
            var maxy = state.Max(s => s.Y);
            Console.WriteLine((maxx - minx + 1) * (maxy - miny + 1) - state.Count);
        }

        public override void SolveMain()
        {
            var (_, nr) = Run(int.MaxValue);
            Console.WriteLine(nr + 1);
        }
    }
}
