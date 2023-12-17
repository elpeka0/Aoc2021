using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day17 : DayBase
    {
        public Day17() : base(17)
        {
        }

        public override void Solve()
        {
            var grid = Grid<int>.FromLines(GetInputLines(false).ToList(), c => c - '0');
            grid[0, 0] = 0;
            var dict = Utils.Dijkstra(
                (new Vector(0, 0), new Vector(0, 0)),
                Step);
            var res = dict
                .Where(kv => kv.Key.Item1 == new Vector(grid.Width - 1, grid.Height - 1))
                .Select(kv => kv.Value)
                .Min();
            Console.WriteLine(res);

            IEnumerable<((Vector, Vector), long)> Step((Vector Pos, Vector Into) arg)
            {
                var (p, into) = arg;
                foreach (var n in grid.Neighbors(p, false))
                {
                    var dir = n - p;
                    if (dir != into && dir != -into)
                    {
                        var v = p;
                        var cost = 0L;
                        for (var i = 1; i <= 3; ++i)
                        {
                            v += dir;
                            if (grid.IsInBounds(v))
                            {
                                cost += grid[v];
                                yield return ((v, dir), cost);
                            }
                        }
                    }
                }
            }
        }

        public override void SolveMain()
        {
            var grid = Grid<int>.FromLines(GetInputLines(false).ToList(), c => c - '0');
            grid[0, 0] = 0;
            var dict = Utils.Dijkstra(
                (new Vector(0, 0), new Vector(0, 0)),
                Step);
            var res = dict
                .Where(kv => kv.Key.Item1 == new Vector(grid.Width - 1, grid.Height - 1))
                .Select(kv => kv.Value)
                .Min();
            Console.WriteLine(res);

            IEnumerable<((Vector, Vector), long)> Step((Vector Pos, Vector Into) arg)
            {
                var (p, into) = arg;
                foreach (var n in grid.Neighbors(p, false))
                {
                    var dir = n - p;
                    if (dir != into && dir != -into)
                    {
                        var v = p;
                        var cost = 0L;
                        for (var i = 1; i <= 10; ++i)
                        {
                            v += dir;
                            if (grid.IsInBounds(v))
                            {
                                cost += grid[v];
                                if (i >= 4)
                                {
                                    yield return ((v, dir), cost);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
