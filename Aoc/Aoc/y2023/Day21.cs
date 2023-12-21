using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Aoc.Geometry;

namespace Aoc.y2023
{
    public class Day21 : DayBase
    {
        public Day21() : base(21)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Indexes().First(v => grid[v] == 'S');
            var res = Utils.FloodFill(start, (p, n) => grid
                .Neighbors(p, false)
                .Where(v => grid[v] != '#' && n < 64));
            Console.WriteLine(res.Count(p => p.Value % 2 == 0));
        }

        public override void SolveMain()
        {
            var target = 26501365L;
            //var target = 131*5L + 65;
            //var target = 1000;

            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var same = Utils.FloodFill(new Vector(0, 0), (v, _) => grid.Neighbors(v, false)
                    .Where(n => grid[n] != '#'))
                    .Where(kv => kv.Value % 2 == target % 2)
                    .ToList();
            var off = Utils.FloodFill(new Vector(0, 0), (v, _) => grid.Neighbors(v, false)
                    .Where(n => grid[n] != '#'))
                .Where(kv => kv.Value % 2 != target % 2)
                .ToList();

            var distance = target / grid.Width - 1;
            var extra = target % grid.Width;

            var total = 0L;
            foreach (var corner in new[]
                     {
                         new Vector(0, 0),
                         new Vector(grid.Width-1, 0),
                         new Vector(0, grid.Height-1),
                         new Vector(grid.Width-1, grid.Height-1),
                     })
            {
                long bigHalfSame = same.Count(kv => kv.Key.X - corner.X + kv.Key.Y - corner.Y <= grid.Width);
                long smallHalfSame = same.Count(kv => kv.Key.X - corner.X + kv.Key.Y - corner.Y > grid.Width);
                long bigHalfOff = off.Count(kv => kv.Key.X - corner.X + kv.Key.Y - corner.Y <= grid.Width);
                long smallHalfOff = off.Count(kv => kv.Key.X - corner.X + kv.Key.Y - corner.Y > grid.Width);

                var quarter = distance/2 * (distance/2) * bigHalfSame;
                if (distance > 0)
                {
                    quarter += (distance - 1) / 2 * ((distance - 1) / 2 + 1) * bigHalfOff;
                    quarter += (distance - 1) / 2 * ((distance - 1) / 2) * smallHalfSame;
                }

                if (distance > 1)
                {
                    quarter += (distance - 2) / 2 * (distance / 2) * smallHalfOff;
                }

                long remainderBig = Utils.FloodFill(corner, (v, c) => grid.Neighbors(v, false)
                    .Where(n => grid[n] != '#' && c < grid.Width + extra))
                    .Count(kv => (distance % 2) switch
                    {
                        0 => kv.Value % 2 != target % 2,
                        _ => kv.Value % 2 == target % 2
                    });
                long remainderSmall = Utils.FloodFill(corner, (v, c) => grid.Neighbors(v, false)
                    .Where(n => grid[n] != '#' && c < 2 * grid.Width + extra))
                    .Count(kv => kv.Key.X - corner.X + kv.Key.Y - corner.Y > grid.Width 
                        && (distance % 2) switch
                        {
                            0 => kv.Value % 2 == target % 2,
                            _ => kv.Value % 2 != target % 2
                        });
                long remainderSmallExtra = Utils.FloodFill(corner, (v, c) => grid.Neighbors(v, false)
                        .Where(n => grid[n] != '#' && c < extra))
                    .Count(kv => (distance % 2) switch
                    {
                        0 => kv.Value % 2 == target % 2,
                        _ => kv.Value % 2 != target % 2
                    });
                quarter += distance * remainderBig;
                quarter += (distance - 1) * remainderSmall;
                quarter += (distance + 1) * remainderSmallExtra;
                total += quarter;
            }

            foreach (var edge in new[]
                     {
                         new Vector(grid.Width/2, 0),
                         new Vector(grid.Width/2, grid.Height-1),
                         new Vector(0, grid.Height/2),
                         new Vector(grid.Width-1, grid.Height/2)
                     })
            {
                total += distance/2 * same.Count + distance/2 * off.Count;
                if (distance % 2 == 1)
                {
                    total += off.Count;
                }

                var remainder = Utils.FloodFill(edge, (v, c) => grid.Neighbors(v, false)
                        .Where(n => grid[n] != '#' && c <= grid.Width / 2 + extra))
                    .Count(kv => (distance % 2) switch
                    {
                        0 => kv.Value % 2 == target % 2,
                        _ => kv.Value % 2 != target % 2
                    });
                total += remainder;
            }
            total += same.Count;

            Console.WriteLine(total);

            if(distance < 10)
            {
                var d = (int)distance;
                var start = grid.Indexes().First(v => grid[v] == 'S');
                var res = Utils.FloodFill(start, (p, n) =>
                    p.Neighbors(false)
                    .Where(v => grid[grid.ModulusVector(v)] != '#' && n < target));

                var center = grid.Indexes().Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);

                var left = grid.Indexes().Select(i => i + new Vector(-grid.Width, 0)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var right = grid.Indexes().Select(i => i + new Vector(grid.Width, 0)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var top = grid.Indexes().Select(i => i + new Vector(0, -grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var bottom = grid.Indexes().Select(i => i + new Vector(0, grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);

                var left2 = grid.Indexes().Select(i => i + new Vector(-(d + 1)*grid.Width, 0)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var right2 = grid.Indexes().Select(i => i + new Vector((d + 1)*grid.Width, 0)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var top2 = grid.Indexes().Select(i => i + new Vector(0, -(d + 1)*grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var bottom2 = grid.Indexes().Select(i => i + new Vector(0, (d + 1)*grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);

                var nw = grid.Indexes().Select(i => i + new Vector(-d*grid.Width, -grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var ne = grid.Indexes().Select(i => i + new Vector(d*grid.Width, -grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var se = grid.Indexes().Select(i => i + new Vector(d*grid.Width, grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);
                var sw = grid.Indexes().Select(i => i + new Vector(-d*grid.Width, grid.Height)).Count(i => res.ContainsKey(i) && res[i] % 2 == target % 2);

                Console.WriteLine($"Same: {same.Count}");
                Console.WriteLine($"Off: {off.Count}");
                Console.WriteLine($"Center: {center}");
                Console.WriteLine($"Left: {left}");
                Console.WriteLine($"Right: {right}");
                Console.WriteLine($"Top: {top}");
                Console.WriteLine($"Bottom: {bottom}");
                Console.WriteLine($"nw: {nw}");
                Console.WriteLine($"ne: {ne}");
                Console.WriteLine($"se: {se}");
                Console.WriteLine($"sw: {sw}");
                Console.WriteLine($"Left2: {left2}");
                Console.WriteLine($"Right2: {right2}");
                Console.WriteLine($"Top2: {top2}");
                Console.WriteLine($"Bottom2: {bottom2}");

                Console.WriteLine(res.Count(p => p.Value % 2 == target % 2));
                Console.WriteLine(total - res.Count(p => p.Value % 2 == target % 2));
                Console.WriteLine((total - res.Count(p => p.Value % 2 == target % 2))/3802);
            }
        }
    }
}
