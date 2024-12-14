using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day14 : DayBase
    {
        public Day14() : base(14)
        {
        }

        private record Entry(LongVector Pos, LongVector Velocity);

        private const int Width = 101;
        private const int Height = 103;

        private IReadOnlyList<Entry> Load()
        {
            var re = new Regex(@"(\d|-)+");
            return GetInputLines()
                .Select(l =>
                {
                    var m = re.Matches(l);
                    return new Entry(new LongVector(long.Parse(m[0].Value), long.Parse(m[1].Value)), new LongVector(long.Parse(m[2].Value), long.Parse(m[3].Value)));
                }).ToImmutableList();
        }

        public override void Solve()
        {
            long a = 0, b = 0, c = 0, d = 0;

            foreach (var e in Load())
            {
                var p = e.Pos + 100 * e.Velocity;
                p = new LongVector((p.X % Width + Width) % Width, (p.Y % Height + Height) % Height);

                if (p.X < Width / 2 && p.Y < Height / 2)
                {
                    a++;
                }
                if (p.X > Width / 2 && p.Y < Height / 2)
                {
                    b++;
                }
                if (p.X < Width / 2 && p.Y > Height / 2)
                {
                    c++;
                }
                if (p.X > Width / 2 && p.Y > Height / 2)
                {
                    d++;
                }
            }

            Console.WriteLine(a * b * c * d);
        }

        public override void SolveMain()
        {
            var entries = Load();
            var dict = new Dictionary<int, int>();

            for(var i = 0;i < entries.Count;i++)
            {
                var seen = new HashSet<LongVector>();
                var e = entries[i];
                var n = 0;
                while (true)
                {
                    var p = e.Pos + n * e.Velocity;
                    p = new LongVector((p.X % Width + Width) % Width, (p.Y % Height + Height) % Height);
                    if (!seen.Add(p))
                    {
                        break;
                    }
                    n++;
                }
                dict[i] = seen.Count;
            }

            for (var i = 0; i < dict[0]; i++)
            {
                //Console.WriteLine($"Iteration {i}");
                var grid = Grid<char>.WithSize(Width, Height);
                grid.Fill(' ');
                var set = new HashSet<LongVector>();
                foreach (var e in entries)
                {
                    var p = e.Pos + i * e.Velocity;
                    p = new LongVector((p.X % Width + Width) % Width, (p.Y % Height + Height) % Height);
                    grid[p.ToVector()] = 'X';
                    set.Add(p);
                }
                if (set.Count == entries.Count)
                {
                    Console.WriteLine($"Iteration {i}");
                    Console.Write(grid.ToString(null));
                }
            }

            Debugger.Break();
        }
    }
}
