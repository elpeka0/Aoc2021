using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day15 : DayBase
    {
        public Day15() : base(15)
        {
        }

        private Grid<int> GetInput()
        {
            var all = this.GetInputLines(false).ToList();
            var size = all.Count*5;
            var grid = new Grid<int>(size, size);
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

            for (var mx = 0; mx < 5; ++mx)
            {
                for (var my = 0; my < 5; ++my)
                {
                    if (mx != 0 || my != 0)
                    {
                        for (var x = 0; x < all.Count; ++x)
                        {
                            for (var cy = 0; cy < all.Count; ++cy)
                            {
                                grid[mx * all.Count + x, my * all.Count + cy] = (grid[x, cy] + mx + my - 1) % 9 + 1;
                            }
                        }
                    }
                }
            }

            return grid;
        }

        private Grid<int> ComputeCosts(Grid<int> grid)
        {
            var cost = new Grid<int>(grid.Width, grid.Height);
            cost.Fill(int.MaxValue);

            cost[0, 0] = 0;
            var any = true;
            while (any)
            {
                any = false;
                var queue = new Queue<Vector>();
                var seen = new HashSet<Vector>();
                var current = new Vector();
                seen.Add(current);
                do
                {
                    foreach (var point in grid.Neighbors(current.X, current.Y, false))
                    {
                        if (!seen.Contains(point))
                        {
                            queue.Enqueue(point);
                            seen.Add(point);
                        }
                    }

                    current = queue.Dequeue();

                    var newCost = cost.Neighbors(current.X, current.Y, false).Select(p => cost[p.X, p.Y]).Min() +
                        grid[current.X, current.Y];
                    if (newCost < cost[current.X, current.Y])
                    {
                        cost[current.X, current.Y] = newCost;
                        any = true;
                    }

                } while (queue.Any());
            }

            return cost;
        }

        public override void Solve()
        {
            var grid = this.GetInput();
            var cost = this.ComputeCosts(grid);
            // Console.WriteLine(grid);
            Console.WriteLine(cost[grid.Width - 1, grid.Height - 1]);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
