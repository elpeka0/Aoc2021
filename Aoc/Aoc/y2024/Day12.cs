using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2024
{
    public class Day12 : DayBase
    {
        public Day12() : base(12)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
            var seen = new HashSet<Vector>();

            var total = 0;

            foreach (var index in grid.Indexes())
            {
                if (seen.Contains(index))
                {
                    continue;
                }

                var area = 0;
                var perimeter = 0;

                Utils.FloodFill(index, (n, _) =>
                {
                    if (!seen.Add(n))
                    {
                        return Enumerable.Empty<Vector>();
                    }

                    area++;
                    perimeter += n.Neighbors(false).Count(x => !grid.IsInBounds(x) || grid[x] != grid[n]);
                    return grid.Neighbors(n, false).Where(x => grid[x] == grid[n]);
                });

                total += area * perimeter;
            }

            Console.WriteLine(total);
        }

        public override void SolveMain()
        {
            var original = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
            var grid = Grid<char>.WithSize(original.Width * 3, original.Height * 3);
            foreach (var index in original.Indexes())
            {
                for (var x = 0; x < 3; x++)
                {
                    for (var y = 0; y < 3; y++)
                    {
                        grid[index.X * 3 + x, index.Y * 3 + y] = original[index];
                    }
                }
            }

            var seen = new HashSet<Vector>();

            var total = 0;

            foreach (var index in grid.Indexes())
            {
                if (seen.Contains(index))
                {
                    continue;
                }

                var plot = new HashSet<Vector>();
                Utils.FloodFill(index, (n, _) =>
                {
                    if (!seen.Add(n))
                    {
                        return Enumerable.Empty<Vector>();
                    }

                    plot.Add(n);
                    return grid.Neighbors(n, false).Where(x => grid[x] == grid[n]);
                });

                var sides = plot.Count(x =>
                {
                    var connected = x.Neighbors(false).Count(plot.Contains);
                    var empty = x.Neighbors(true).Count(y => !plot.Contains(y));
                    return connected == 2 || (connected == 4 && empty == 1);
                });

                total += (plot.Count / 9) * sides;
            }

            Console.WriteLine(total);
        }
    }
}
