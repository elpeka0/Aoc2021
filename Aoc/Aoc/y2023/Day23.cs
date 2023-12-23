using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day23 : DayBase
    {
        public Day23() : base(23)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Row(0).Indexes().First(i => grid[i] == '.');
            var target = grid.Row(grid.Width - 1).Indexes().First(i => grid[i] == '.');

            var res = LongestPath(start, target, grid, CanMove);
            Console.WriteLine(res);

            IEnumerable<Vector> CanMove(Vector p)
            {
                return grid[p] switch
                {
                    '.' => grid.Neighbors(p, false),
                    '>' => new[] { p + new Vector(1, 0) },
                    '<' => new[] { p + new Vector(-1, 0) },
                    '^' => new[] { p + new Vector(0, -1) },
                    'v' => new[] { p + new Vector(0, 1) }
                };
            }
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Row(0).Indexes().First(i => grid[i] == '.');
            var target = grid.Row(grid.Width - 1).Indexes().First(i => grid[i] == '.');

            var res = LongestPath(start, target, grid, p => grid.Neighbors(p, false));
            Console.WriteLine(res);
        }

        private long LongestPath(Vector start, Vector target, Grid<char> grid, Func<Vector, IEnumerable<Vector>> stepFunc)
        {
            return Impl(start, new HashSet<Vector>());

            long Impl(Vector s, HashSet<Vector> visited)
            {
                visited.Add(s);
                if (s == target)
                {
                    return 0;
                }
                var best = -1L;
                foreach (var n in stepFunc(s).Where(i => grid[i] != '#'))
                {
                    if (!visited.Contains(n))
                    {
                        var l = Impl(n, visited.ToHashSet());
                        if (l > best)
                        {
                            best = l;
                        }
                    }
                }
                return best + 1;
            }
        }
    }
}
