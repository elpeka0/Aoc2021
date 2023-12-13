using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2023
{
    public class Day13 : DayBase
    {
        public Day13() : base(13)
        {
        }

        public override void Solve()
        {
            var grids = this.Load();
            var sum = 0;
            for (var i = 0; i < grids.Count; i++)
            {
                var (c, r) = FindAxis(grids[i]);
                Console.WriteLine($"{i}: {c} {r}");
                sum += (c ?? 0) + 100 * (r ?? 0);
            }

            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var grids = this.Load();
            var sum = 0;
            for (var i = 0; i < grids.Count; i++)
            {
                var grid = grids[i];
                var (c, r) = FindAxis(grid);
                (c, r) = grid
                    .Indexes()
                    .Select(p =>
                    {
                        var old = grid[p];
                        grid[p] = old switch
                        {
                            '.' => '#',
                            _ => '.'
                        };
                        var t = FindAxis(grid, c, r);
                        grid[p] = old;
                        return t;
                    })
                    .First(t => t.ColIdx != null || t.RowIdx != null);
                sum += (c ?? 0) + 100 * (r ?? 0);
            }

            Console.WriteLine(sum);
        }

        private List<Grid<char>> Load()
        {
            var lines = this.GetInputLines(false).ToList();
            var grouped = new List<Grid<char>>();
            var l = new List<string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) && l.Any())
                {
                    grouped.Add(Grid<char>.FromLines(l, c => c));
                    l.Clear();
                }
                else
                {
                    l.Add(line);
                }
            }

            if (l.Any())
            {
                grouped.Add(Grid<char>.FromLines(l, c => c));
            }

            return grouped;
        }

        private (int? ColIdx, int? RowIdx) FindAxis(Grid<char> grid, int? excludeCol = null, int? excludeRow = null)
        {
            var columnDict = BuildReflectionDictionary(grid.Columns());
            var rowDict = BuildReflectionDictionary(grid.Rows());

            var colIdx = IndexOfReflection(columnDict, 0)
                .Concat(IndexOfReflection(columnDict, grid.Width - 1))
                .FirstOrDefault(i => i != excludeCol);
            var rowIdx = IndexOfReflection(rowDict, 0)
                .Concat(IndexOfReflection(rowDict, grid.Height - 1))
                .FirstOrDefault(i => i != excludeRow);

            return (colIdx, rowIdx);

            Dictionary<int, List<int>> BuildReflectionDictionary(IEnumerable<GridSlice<char>> slices)
            {
                return slices
                    .Select((c, i) => (Value: new string(c.ToArray()), Index: i))
                    .GroupBy(t => t.Value)
                    .SelectMany(g => g.Select(t => (t.Index, List: g.Select(s => s.Index).ToList())))
                    .ToDictionary(t => t.Index, t => t.List);
            }

            IEnumerable<int?> IndexOfReflection(Dictionary<int, List<int>> d, int start)
            {
                foreach (var other in d[start].Where(x => x != start))
                {
                    var max = Math.Max(start, other);
                    var min = Math.Min(start, other);
                    var res = min + (max - min + 1) / 2;

                    var ok = true;
                    while (min < max && ok)
                    {
                        if (!d[min].Contains(max))
                        {
                            ok = false;
                        }

                        min++;
                        max--;
                    }
                    
                    if (min != max && ok)
                    {
                        yield return res;
                    }
                }
            }
        }
    }
}
