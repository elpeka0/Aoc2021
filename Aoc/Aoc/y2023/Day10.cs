using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day10 : DayBase
    {
        public Day10() : base(10)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var loop = GetLoop(grid);
            Console.WriteLine((loop.Count + 1) / 2);
        }

        private List<Vector> GetLoop(Grid<char> grid)
        {
            var pos = grid.Indexes().First(i => grid[i] == 'S');

            foreach (var dir in grid.Neighbors(pos, false).Where(n => PipeConnects(grid[n], 'S', n, pos)))
            {
                var path = Utils.Dfs(dir,
                    p => grid.Neighbors(p, false)
                        .Where(n => PipeConnects(grid[p], grid[n], p, n)
                            && (p != dir || n != pos)),
                        p => p == pos);
                if (path != null)
                {
                    return path;
                }
            }

            throw new InvalidOperationException("Fuck this shit!");
        }

        private IEnumerable<Vector> Connections(char c)
        {
            return c switch
            {
                'S' => Vector.Origin.Neighbors(false),
                '|' => Vector.Origin.Neighbors(false).Where(v => v.X == 0),
                '-' => Vector.Origin.Neighbors(false).Where(v => v.Y == 0),
                'L' => new[] { new Vector(0, -1), new Vector(1, 0) },
                'J' => new[] { new Vector(0, -1), new Vector(-1, 0) },
                '7' => new[] { new Vector(0, 1), new Vector(-1, 0) },
                'F' => new[] { new Vector(0, 1), new Vector(1, 0) },
                _ => Enumerable.Empty<Vector>()
            };
        }

        private bool PipeConnects(char p0, char p1, Vector v0, Vector v1)
        {
            var a = Connections(p0);
            var b = Connections(p1);
            return (a.Any(aa => v0 + aa == v1)
                && b.Any(bb => v1 + bb == v0));
        }

        private record NormalLoopElement(Vector Pos, Vector NormalSign);

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var loop = GetLoop(grid);


            var normalLoop = new List<NormalLoopElement>();
            Vector sign = new Vector(1, 1);
            var v = new Vector(1, 1);

            for (var i = 0; i < loop.Count; i++)
            {
                var delta = loop[i == loop.Count - 1 ? 0 : i + 1] - loop[i];
                var use = sign;
                if (delta != v)
                {
                    if (delta.Y == -v.X && delta.X == v.Y)
                    {
                        if (delta.X != v.X)
                        {
                            sign = new Vector(-sign.Y, sign.X);
                        }
                        else if (delta.Y != v.Y)
                        {
                            sign = new Vector(sign.Y, -sign.X);
                        }
                        use = sign;
                    }
                    else
                    {
                        if (delta.X != v.X)
                        {
                            sign = new Vector(sign.Y, -sign.X);
                        }
                        else if (delta.Y != v.Y)
                        {
                            sign = new Vector(-sign.Y, sign.X);
                        }
                    }
                }
                v = delta;
                normalLoop.Add(new NormalLoopElement(loop[i], use));
            }

            Visualize(grid, normalLoop);
            var d = normalLoop.ToDictionary(n => n.Pos, n => n.NormalSign);

            var cnt = 0;
            foreach (var p in grid.Indexes().Where(i => !d.ContainsKey(i)))
            {
                var path = Utils.Bfs(p, n => grid.Neighbors(n, false), n => d.ContainsKey(n));
                var delta = path.Last() - path[path.Count - 2];
                var sx = delta.X * d[path.Last()].X;
                var sy = delta.Y * d[path.Last()].Y;
                if (sx <= 0 && sy >= 0)
                {
                    ++cnt;
                }
            }

            Console.WriteLine(cnt);
        }

        private static void Visualize(Grid<char> grid, List<NormalLoopElement> normalLoop)
        {
            var points = normalLoop.ToDictionary(n => n.Pos, n => n.NormalSign);
            foreach (var row in grid.Rows())
            {
                foreach (var i in row.Indexes())
                {
                    if (points.TryGetValue(i, out var s))
                    {
                        var xs = s.X == 1 ? '<' : '>';
                        var ys = s.Y == 1 ? 'v' : '^';
                        Console.Write($"({xs}, {ys}) ");
                    }
                    else
                    {
                        Console.Write("       ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
