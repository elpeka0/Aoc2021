using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2023
{
    public class Day11 : DayBase
    {
        public Day11() : base(11)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            SolveWithWeight(grid, 2L);
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            SolveWithWeight(grid, 1000000L);
        }

        private static void SolveWithWeight(Grid<char> grid, long weight)
        {
            var galaxies = grid.Indexes().Where(i => grid[i] == '#').ToList();

            var expandedColumns = new HashSet<int>();
            for (var x = 0; x < grid.Width; ++x)
            {
                if (grid.Column(x).All(c => c != '#'))
                {
                    expandedColumns.Add(x);
                }
            }

            var expandedRows = new HashSet<int>();
            for (var y = 0; y < grid.Height; ++y)
            {
                if (grid.Row(y).All(c => c != '#'))
                {
                    expandedRows.Add(y);
                }
            }

            var distances = new Dictionary<(Vector, Vector), long>();

            foreach (var g in galaxies)
            {
                var mapped = Utils.Dijkstra(
                    g,
                    v => grid.Neighbors(v, false).Select(n =>
                    {
                        if (v.X != n.X && expandedColumns.Contains(n.X))
                        {
                            return (n, weight);
                        }

                        if (v.Y != n.Y && expandedRows.Contains(n.Y))
                        {
                            return (n, weight);
                        }

                        return (n, 1);
                    }));
                foreach (var h in galaxies)
                {
                    if (h != g && !distances.ContainsKey((h, g)))
                    {
                        distances[(g, h)] = mapped[h];
                    }
                }
            }

            Console.WriteLine(distances.Values.Sum());
        }
    }
}
