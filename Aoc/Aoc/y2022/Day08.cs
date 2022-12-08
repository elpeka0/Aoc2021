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

        private Grid<int> GetInput() => Grid<int>.FromLines(GetInputLines(false).ToList(), c => c - '0');

        private void Iterate(Grid<int> grid, Action reset, Action<int, int> callback)
        {
            foreach(var slice in 
                grid.Rows()
                .Concat(grid.Columns())
                .Concat(grid.Rows().Select(r => r.Invert()))
                .Concat(grid.Columns().Select(c => c.Invert())))
            {
                reset();
                foreach (var (x, y) in slice.Indexes)
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
