using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day16 : DayBase
    {
        public Day16() : base(16)
        {
        }

        private record Beam(Vector Position, Vector Direction);

        private int CountTiles(Grid<char> grid, Beam initial)
        {
            var queue = new Queue<Beam>();
            var seen = new HashSet<Beam>();
            queue.Enqueue(initial);

            while (queue.Any())
            {
                var next = queue.Dequeue();
                next = next with { Position = next.Position + next.Direction };
                if (!seen.Contains(next) && grid.IsInBounds(next.Position))
                {
                    seen.Add(next);
                    switch (grid[next.Position])
                    {
                        case '\\':
                            queue.Enqueue(next with { Direction = new Vector(next.Direction.Y, next.Direction.X) });
                            break;
                        case '/':
                            queue.Enqueue(next with { Direction = new Vector(-next.Direction.Y, -next.Direction.X) });
                            break;
                        case '-' when next.Direction.Y != 0:
                            queue.Enqueue(next with { Direction = new Vector(next.Direction.Y, 0) });
                            queue.Enqueue(next with { Direction = new Vector(-next.Direction.Y, 0) });
                            break;
                        case '|' when next.Direction.X != 0:
                            queue.Enqueue(next with { Direction = new Vector(0, next.Direction.X) });
                            queue.Enqueue(next with { Direction = new Vector(0, -next.Direction.X) });
                            break;
                        default:
                            queue.Enqueue(next);
                            break;
                    }
                }
            }

            return seen.Select(t => t.Position).Distinct().Count();
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            Console.WriteLine(CountTiles(grid, new Beam(new Vector(-1, 0), new Vector(1, 0))));
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var max = 0;
            for (var x = 0; x < grid.Width; x++)
            {
                var n = CountTiles(grid, new Beam(new Vector(x, -1), new Vector(0, 1)));
                if (n > max)
                {
                    max = n;
                }

                n = CountTiles(grid, new Beam(new Vector(x, grid.Height), new Vector(0, -1)));
                if (n > max)
                {
                    max = n;
                }
            }

            for (var y = 0; y < grid.Height; y++)
            {
                var n = CountTiles(grid, new Beam(new Vector(-1, y), new Vector(1, 0)));
                if (n > max)
                {
                    max = n;
                }

                n = CountTiles(grid, new Beam(new Vector(grid.Width, y), new Vector(-1, 0)));
                if (n > max)
                {
                    max = n;
                }
            }

            Console.WriteLine(max);
        }
    }
}
