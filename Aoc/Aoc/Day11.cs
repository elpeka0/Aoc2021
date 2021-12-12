using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day11 : DayBase
    {
        private class Octopus
        {
            public bool Flashed { get; set; }
            
            public int Value { get; set; }
            public int X { get; set; }

            public int Y { get; set; }

            public IEnumerable<(int X, int Y)> Neighbors()
            {
                if (X > 0)
                {
                    yield return (X - 1, Y);
                    if (Y > 0)
                    {
                        yield return (X - 1, Y - 1);
                    }
                    if (Y < 9)
                    {
                        yield return (X - 1, Y + 1);
                    }
                }
                if (Y > 0)
                {
                    yield return (X, Y - 1);
                }
                if (Y < 9)
                {
                    yield return (X, Y + 1);
                }
                if (X < 9)
                {
                    yield return (X + 1, Y);
                    if (Y > 0)
                    {
                        yield return (X + 1, Y - 1);
                    }
                    if (Y < 9)
                    {
                        yield return (X + 1, Y + 1);
                    }
                }
            }
        }

        public Day11() : base(11)
        {
        }

        public override void Solve()
        {
            var octopuses = new Octopus[10, 10];
            int y = 0;
            foreach (var line in GetInputLines(false))
            {
                int x = 0;
                foreach (var c in line)
                {
                    octopuses[x, y] = new Octopus
                    {
                        Value = c - '0',
                        X = x,
                        Y = y
                    };
                    ++x;
                }
                ++y;
            }

            for (int step = 0;; ++step)
            {
                foreach (var octopus in octopuses)
                {
                    ++octopus.Value;
                }
                var cnt = 0;
                var any = true;
                while (any)
                {
                    any = false;
                    foreach (var octopus in octopuses)
                    {
                        if (octopus.Value > 9 && !octopus.Flashed)
                        {
                            octopus.Flashed = true;
                            any = true;
                            ++cnt;
                            foreach (var (xn, yn) in octopus.Neighbors())
                            {
                                ++octopuses[xn, yn].Value;
                            }
                        }
                    }
                }
                foreach (var octopus in octopuses)
                {
                    if (octopus.Flashed)
                    {
                        octopus.Value = 0;
                        octopus.Flashed = false;
                    }
                }
                if (cnt == 100)
                {
                    Console.WriteLine(step + 1);
                    break;
                }
            }
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
