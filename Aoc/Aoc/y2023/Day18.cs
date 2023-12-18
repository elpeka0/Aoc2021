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

            var y = (grid.MaxY + grid.MinY) / 2;
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

            File.WriteAllText("C:\\dev\\fuck.txt", grid.ToString());
            Console.WriteLine(grid.Count(c => c != '.'));
        }

        public override void SolveMain()
        {
            var instructions = GetInputLines(false).Select(Parse).ToList();
            var p = Vector.Origin;
            var loop = new List<Vector>();
            foreach (var i in instructions)
            {
                loop.Add(p);
                p += i.Steps * i.Direction;
            }
            loop.Add(p);

            Console.WriteLine(Area(loop));
        }

        private long Circumference(List<Vector> path)
        {
            var res = 0L;
            for (var i = 0; i < path.Count; ++i)
            {
                var n = path[i + 1 < path.Count ? i + 1 : 0];
                res += Math.Abs(n.X - path[i].X) + Math.Abs(n.Y - path[i].Y);
            }
            return res;
        }

        private long Distance(Vector a, Vector b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private long Area(List<Vector> path)
        {
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

            var sub = new List<Vector>();
            var neg = 0L;
            foreach (var p in path)
            {
                var xBounded = p.X == xMin || p.X == xMax;
                var yBounded = p.Y == yMin || p.Y == yMax;

                if (xBounded != yBounded)
                {
                    if (sub.Count > 0)
                    {
                        sub.Add(p);
                        neg += Area(sub) - Circumference(sub) + Distance(sub[0], p) - 1;
                        sub.Clear();
                    }
                    else
                    {
                        sub.Add(p);
                    }
                }
                else if (!xBounded)
                {
                    sub.Add(p);
                }
            }

            if (sub.Count > 0)
            {
                neg += Area(sub) - Circumference(sub) + Distance(sub[0], path.Last()) - 1;
            }

            return (xMax - xMin + 1)*(yMax - yMin + 1) - neg;
        }
    }
}
