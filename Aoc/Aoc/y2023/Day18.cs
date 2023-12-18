using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aoc.Geometry;
using Aoc.Parsing;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day18 : DayBase
    {
        private record Instruction(Vector Direction, int Steps)
        {
            public static Instruction FromHex(string s)
            {
                var steps = int.Parse(s.Substring(0, 5), NumberStyles.HexNumber);
                var dir = s[5] switch
                {
                    '0' => new Vector(1, 0),
                    '1' => new Vector(0, 1),
                    '2' => new Vector(-1, 0),
                    _ => new Vector(0, -1)
                };
                return new Instruction(dir, steps);
            }
        }

        public Day18() : base(18)
        {
        }

        private Instruction Parse(string line)
        {
            return "R".Map(_ => new Vector(1, 0))
                .Or("L".Map(_ => new Vector(-1, 0)))
                .Or("U".Map(_ => new Vector(0, -1)))
                .Or("D".Map(_ => new Vector(0, 1)))
                .ThenWs(Integer())
                .ThenWs("(#")
                .ThenWs(Char(c => char.IsLetterOrDigit(c)).String())
                .ThenWs(")")
                .Map(t => new Instruction(t.Item1.Item1, (int) t.Item1.Item2))
                .Evaluate(line);
        }

        private Instruction ParseMain(string line)
        {
            return Char(char.IsLetter).Discard()
                .ThenWs(Integer().Discard())
                .ThenWs("(#")
                .ThenWs(Char(c => char.IsLetterOrDigit(c)).String())
                .ThenWs(")")
                .Map(t => Instruction.FromHex(t))
                .Evaluate(line);
        }

        public override void Solve()
        {
            var instructions = GetInputLines(false).Select(Parse).ToList();
            var grid = Grid<char>.Dynamic('.');
            var p = Vector.Origin;
            foreach (var i in instructions)
            {
                for (var n = 0; n < i.Steps; ++n)
                {
                    grid[p] = '#';
                    p += i.Direction;
                }
            }

            Console.WriteLine(grid.Count(c => c != '.'));
        }

        private void FloodFillArea(Grid<char> grid)
        {
            var y = (grid.MaxY + grid.MinY) / 2 + 1;
            var x = grid.MinX - 1;
            var delta = Math.Sign(grid.MaxX - grid.MinX);
            while (grid[x, y] != '#')
            {
                x += delta;
            }

            x++;

            var filled = Utils.FloodFill(new Vector(x, y), p => grid.Neighbors(p, false).Where(i => grid[i] == '.'));
            foreach (var c in filled)
            {
                grid[c] = 'O';
            }
        }

        public override void SolveMain()
        {
            var instructions = GetInputLines(false).Select(ParseMain).ToList();
            var p = Vector.Origin;
            var loop = new List<Vector>();
            foreach (var i in instructions)
            {
                loop.Add(p);
                p += i.Steps * i.Direction;
            }
            loop.Add(p);

            Console.WriteLine(Area(loop));
            /*var loop = new List<Vector>
            {
                new Vector(6, 5) * 2,
                new Vector(2, 5) * 2,
                new Vector(2, 3) * 2,
                new Vector(3, 3) * 2,
                new Vector(3, 2) * 2,
                new Vector(1, 2) * 2,
                new Vector(1, 5) * 2,
                new Vector(0, 5) * 2,
                new Vector(0, 8) * 2,
                new Vector(1, 8) * 2,
                new Vector(1, 7) * 2,
                new Vector(2, 7) * 2,
                new Vector(2, 10) * 2,
                new Vector(6, 10) * 2,
                new Vector(6, 9) * 2,
                new Vector(4, 9) * 2,
                new Vector(4, 7) * 2,
                new Vector(5, 7) * 2,
                new Vector(5, 8) * 2,
                new Vector(7, 8) * 2,
                new Vector(7, 3) * 2,
                new Vector(5, 3) * 2,
                new Vector(5, 0) * 2,
                new Vector(3, 0) * 2,
                new Vector(3, 1) * 2,
                new Vector(4, 1) * 2,
                new Vector(4, 4) * 2,
                new Vector(6, 4) * 2,
                new Vector(6, 5) * 2
            };
            var grid = Gridify(loop, true);
            FloodFillArea(grid);
            Console.WriteLine(grid);
            Console.WriteLine(grid.Count(c => c != '.'));
            Console.WriteLine(Area(loop));*/
        }

        private Grid<char> Gridify(List<Vector> loop, bool close)
        {
            var res = Grid<char>.Dynamic('.');
            for (var i = 0; i < (close ? loop.Count : loop.Count - 1); i++)
            {
                var n = loop[i + 1 < loop.Count ? i + 1 : 0];
                var delta = n - loop[i];
                var dx = Math.Sign(delta.X);
                var dy = Math.Sign(delta.Y);
                Vector p;
                for (p = loop[i]; p != new Vector(n.X, loop[i].Y);p += new Vector(dx, 0))
                {
                    res[p] = '#';
                }
                for (; p != n; p += new Vector(0, dy))
                {
                    res[p] = '#';
                }
                res[p] = '#';
            }
            return res;
        }

        private long Circumference(List<Vector> path)
        {
            var res = 0L;
            for (var i = 0; i < path.Count; ++i)
            {
                var n = path[i + 1 < path.Count ? i + 1 : 0];
                res += Distance(path[i], n);
            }
            return res;
        }

        private long Distance(Vector a, Vector b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private long Area(List<Vector> path)
        {
            // Console.WriteLine(Gridify(path, false));
            var xMin = long.MaxValue;
            var xMax = long.MinValue;
            var yMin = long.MaxValue;
            var yMax = long.MinValue;

            foreach (var v in path)
            {
                if(v.X < xMin) xMin = v.X;
                if(v.Y < yMin) yMin = v.Y;
                if(v.X > xMax) xMax = v.X;
                if(v.Y > yMax) yMax = v.Y;
            }

            var start = 0;
            while (path[start].X != xMin
                && path[start].Y != yMin
                && path[start].X < xMax
                && path[start].Y < yMax)
            {
                start++;
            }

            var sub = new List<Vector>();
            var neg = 0L;
            for(var i = 0;i < path.Count;i++)
            {
                var p = path[(start + i) % path.Count];
                var xBounded = p.X == xMin || p.X == xMax;
                var yBounded = p.Y == yMin || p.Y == yMax;

                if (xBounded != yBounded)
                {
                    if (sub.Count > 1)
                    {
                        sub.Add(p);
                        neg += NegativeArea(sub);
                    }
                    sub.Clear();
                }
                else if (xBounded)
                {
                    sub.Clear();
                }
                sub.Add(p);
            }

            if (sub.Count > 1)
            {
                var end = path[(start + path.Count - 1) % path.Count];
                sub.Add(end);
                sub.Add(path[start]);
                neg += NegativeArea(sub);
            }

            var res = (xMax - xMin + 1)*(yMax - yMin + 1) - neg;
            // Console.WriteLine($"Area: {res} Neg: {res - Circumference(path) + Distance(path.First(), path.Last()) - 1}");
            return res;
        }

        private long NegativeArea(List<Vector> path)
        {
            return Area(path) - Circumference(path) + Distance(path.First(), path.Last()) - 1;
        }
    }
}
