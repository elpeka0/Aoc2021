using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day08 : DayBase
    {
        
        public Day08() : base(8)
        {
        }

        private Grid<int> GetInput()
        {
            var all = GetInputLines(false).ToList();
            var grid = new Grid<int>(all[0].Length, all.Count);
            var y = 0;
            foreach (var line in all)
            {
                var x = 0;
                foreach (var c in line)
                {
                    grid[x, y] = c - '0';
                    ++x;
                }
                ++y;
            }
            return grid;
        }

        private void Iterate(Grid<int> grid, Action reset, Action<int, int> callback)
        {
            for (var y = 0; y < grid.Height; ++y)
            {
                reset();
                for (var x = 0; x < grid.Width; ++x)
                {
                    callback(x, y);
                }

                reset();
                for (var x = grid.Width - 1; x >= 0; --x)
                {
                    callback(x, y);
                }
            }

            for (var x = 0; x < grid.Width; ++x)
            {
                reset();
                for (var y = 0; y < grid.Height; ++y)
                {
                    callback(x, y);
                }

                reset();
                for (var y = grid.Height - 1; y >= 0; --y)
                {
                    callback(x, y);
                }
            }
        }

        public override void Solve()
        {
            var grid = GetInput();
            var visible = new Grid<bool>(grid.Width, grid.Height);
            int tallest = -1;

            Iterate(grid, () => tallest = -1, (int x, int y) =>
            {
                if (grid[x, y] > tallest)
                {
                    tallest = grid[x, y];
                    visible[x, y] = true;
                }
            });

            Console.WriteLine(visible.Count(b => b));
        }

        public override void SolveMain()
        {
            var grid = GetInput();
            var distance = new Dictionary<int, int>();
            var score = new Grid<int>(grid.Width, grid.Height);
            score.Fill(1);

            Iterate(grid, () =>
            {
                distance.Clear();
                distance[10] = 0;
            }, 
            (int x, int y) =>
            {
                var value = grid[x, y];
                var view = distance.Where(kv => kv.Key >= value).Min(kv => kv.Value);
                score[x, y] *= view;
                distance[value] = 0;
                foreach (var k in distance.Keys)
                {
                    ++distance[k];
                }
            });

            Console.WriteLine(score.Max());
        }
    }
}
