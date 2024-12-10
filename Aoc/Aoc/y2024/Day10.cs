using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2024
{
    public class Day10 : DayBase
    {
        public Day10() : base(10)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);

            var sum = 0;
            foreach (var zero in grid.Indexes().Where(i => grid[i] == '0'))
            {
                var reachable = Utils.FloodFill(zero, (i, _) =>
                {
                    return grid.Neighbors(i, false).Where(b => grid[b] == grid[i] + 1);
                });
                sum += reachable.Count(kv => grid[kv.Key] == '9');
            }

            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);

            var sum = 0;
            foreach (var zero in grid.Indexes().Where(i => grid[i] == '0'))
            {
                var q = new Queue<Vector>();
                var rating = new Dictionary<Vector, int>();
                q.Enqueue(zero);
                rating[zero] = 1;

                while (q.Count > 0)
                {
                    var next = q.Dequeue();
                    foreach (var n in grid.Neighbors(next, false).Where(b => grid[b] == grid[next] + 1))
                    {
                        if (rating.TryGetValue(n, out var r))
                        {
                            rating[n] = r + rating[next];
                        }
                        else
                        {
                            rating[n] = rating[next];
                            q.Enqueue(n);
                        }
                    }
                }
                var head = rating.Where(kv => grid[kv.Key] == '9').Sum(kv => kv.Value);
                Console.WriteLine($"{zero} => {head}");
                sum += head;
            }

            Console.WriteLine(sum);
        }
    }
}
