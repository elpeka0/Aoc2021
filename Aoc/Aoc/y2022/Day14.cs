using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day14 : DayBase
    {
        public Day14() : base(14)
        {
        }

        private IEnumerable<List<(int X, int Y)>> GetLines()
        {
            foreach (var line in GetInputLines(false))
            {
                yield return line.Split(" -> ").Select(p =>
                {
                    var a = p.Split(',');
                    return (X: int.Parse(a[0]), Y: int.Parse(a[1]));
                }).ToList();
            }
        }

        private Grid<char> GetInput(bool addFloor)
        {
            var lines = GetLines().ToList();
            var res = new Grid<char>(1000, 1000);
            res.Fill('.');
            foreach (var points in lines)
            {
                for (var i = 0; i < points.Count - 1; ++i)
                {
                    var a = points[i];
                    var b = points[i + 1];
                    while (!a.Equals(b))
                    {
                        res[a.X, a.Y] = '#';
                        a = (a.X + Math.Sign(b.X - a.X), a.Y + Math.Sign(b.Y - a.Y));
                    }

                    res[b.X, b.Y] = '#';
                }
            }

            var maxY = lines.Max(l => l.Max(p => p.Y));

            res.Row(maxY + 2).Apply((x, y) => res[x, y] = '#');
            return res;
        }

        public override void Solve()
        {
            var grid = this.GetInput(false);
            var cnt = this.SolveInternal(grid);
            Console.WriteLine(cnt);
        }

        private int SolveInternal(Grid<char> grid)
        {
            var allowed = new[] { (X: 0, Y: 1), (X: -1, Y: 1), (X: 1, Y: 1) };
            var cnt = 0;
            mainLoop:
            {
                var x = 500;
                var y = 0;
                if (grid[x, y] != '.')
                {
                    return cnt;
                }

                droppingSand:
                {
                    if (y + 1 < grid.Height)
                    {
                        foreach (var a in allowed)
                        {
                            if (grid[x + a.X, y + a.Y] == '.')
                            {
                                x += a.X;
                                y += a.Y;
                                goto droppingSand;
                            }
                        }

                        grid[x, y] = 's';
                        ++cnt;
                        goto mainLoop;
                    }
                }
            }
            return cnt;
        }

        public override void SolveMain()
        {
            var grid = this.GetInput(true);
            var cnt = this.SolveInternal(grid);
            Console.WriteLine(cnt);
        }
    }
}
