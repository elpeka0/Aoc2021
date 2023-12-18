using System;
using System.Collections.Generic;
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

            var ysteps = loop.Select(v => v.Y).Distinct().OrderBy(k => k).ToList();
            var minx = loop.Min(v => v.X);

            var total = 0L;
            for (var i = 0; i < ysteps.Count - 1; i++)
            {
                var cnt = CountInside(ysteps[i]);
                var height = ysteps[i + 1] - ysteps[i] - 1;
                total += cnt.Normal * height + cnt.Special;
            }
            Console.WriteLine(total);

            (long Normal, long Special) CountInside(int y)
            {
                var sum = 0L;
                var isIn = false;
                var x = minx;
                var d = new List<(int X, bool IsIn)>();
                foreach(var s in Intersections(y + 1).OrderBy(k => k))
                {
                    if (isIn)
                    {
                        sum += s - x + 1;
                    }

                    d.Add((x, isIn));
                    x = s;
                    isIn = !isIn;
                }
                
                x = minx;
                isIn = false;
                foreach (var s in Intersections(y - 1).OrderBy(k => k))
                {
                    d.Add((x, isIn));
                    x = s;
                    isIn = !isIn;
                }

                d = d.GroupBy(t => t.X)
                    .Select(g => (X: g.Key, g.Any(u => u.IsIn)))
                    .OrderByDescending(t => t.X)
                    .ToList();
                var special = 0L;
                foreach (var s in Intersections(y).OrderBy(k => k))
                {
                    if(d.FirstOrDefault(t => t.X < s).IsIn)
                    {
                        special += s - x + 1;
                    }

                    x = s;
                }

                return (sum, special);
            }

            IEnumerable<int> Intersections(int y)
            {
                for (var i = 0; i < loop.Count; i++)
                {
                    var n = i + 1 < loop.Count ? i + 1 : 0;
                    if (IsBetween(y, loop[i].Y, loop[n].Y))
                    {
                        yield return loop[i].X;
                    }
                }
            }

            bool IsBetween(int check, int a, int b)
            {
                return check >= Math.Min(a, b) && check <= Math.Max(a, b);
            }
        }
    }
}
