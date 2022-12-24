using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day12 : DayBase
    {
        

        public Day12() : base(12)
        {
        }

        private Grid<int> GetInput() => Grid<int>.FromLines(GetInputLines(false).ToList(), c => c switch
        {
            'S' => -1,
            'E' => 26,
            _ => c -'a'
        });

        private void SolveInternal(bool backwards)
        {
            var grid = GetInput();
            var start = grid.Indexes().Single(p => grid[p.X, p.Y] == -1);
            var end = grid.Indexes().Single(p => grid[p.X, p.Y] == 26);
            grid[start.X, start.Y] = 0;
            grid[end.X, end.Y] = 25;
            var queue = new Queue<(int X, int Y, int Count)>();
            var seen = new HashSet<(int X, int Y)>();
            var initial = backwards ? end : start;
            queue.Enqueue((initial.X, initial.Y, 0));

            while (true)
            {
                var p = queue.Dequeue();

                if ((backwards && grid[p.X, p.Y] == 0) || (!backwards && p.X == end.X && p.Y == end.Y))
                {
                    Console.WriteLine(p.Count);
                    break;
                }

                bool CanStep((int X, int Y) from)
                {
                    if (backwards)
                    {
                        return grid[from.X, from.Y] >= grid[p.X, p.Y] - 1;
                    }
                    else
                    {
                        return grid[from.X, from.Y] <= grid[p.X, p.Y] + 1;
                    }
                }

                foreach (var n in grid.Neighbors(p.X, p.Y, false).Where(n => !seen.Contains(n) && CanStep(n)))
                {
                    queue.Enqueue((n.X, n.Y, p.Count + 1));
                    seen.Add((n.X, n.Y));
                }
            }
        }

        public override void Solve()
        {
            SolveInternal(false);
        }

        public override void SolveMain()
        {
            SolveInternal(true);
        }
    }
}
