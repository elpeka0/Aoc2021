using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day13 : DayBase
    {
        private class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.X, this.Y);
            }

            public override bool Equals(object obj)
            {
                return obj is Point p && p.X == this.X && p.Y == this.Y;
            }
        }

        private class Input
        {
            public HashSet<Point> Grid { get; set; }

            public List<Point> Instructions { get; set; }
        }

        public Day13() : base(13)
        {
        }

        private Input GetInput()
        {
            var lines = this.GetInputLines(false).ToList();
            var gridLines = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).ToList();
            var instructionLines = lines.SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).ToList();

            var res = new Input
            {
                Grid = gridLines.Select(l => l.Split(',')).Select(p => new Point(int.Parse(p[0]), int.Parse(p[1]))).ToHashSet(),
                Instructions = instructionLines.Select(l => l.Substring("fold along ".Length).Split('='))
                    .Select(p => (IsX: p[0] == "x", Value: int.Parse(p[1])))
                    .Select(i => new Point(i.IsX ? i.Value : int.MaxValue, i.IsX ? int.MaxValue : i.Value)).ToList()
            };
            return res;
        }

        private HashSet<Point> Fold(HashSet<Point> original, Point instruction)
        {
            return original.Select(
                p =>
                {
                    var (x, y) = (p.X, p.Y);
                    if (p.X > instruction.X)
                    {
                        x = 2 * instruction.X - p.X;
                    }

                    if (p.Y > instruction.Y)
                    {
                        y = 2 * instruction.Y - p.Y;
                    }

                    return new Point(x, y);
                }
            ).GroupBy(p => p).Select(g => g.Key).ToHashSet();
        }

        public override void Solve()
        {
            var input = this.GetInput();
            var res = this.Fold(input.Grid, input.Instructions.First());
            Console.WriteLine(res.Count);
        }

        public override void SolveMain()
        {
            var input = this.GetInput();
            var state = input.Grid;
            
            foreach (var instruction in input.Instructions)
            {
                state = this.Fold(state, instruction);
            }
            this.Visualize(state);
        }

        private void Visualize(HashSet<Point> grid)
        {
            var maxx = grid.Max(p => p.X);
            var maxy = grid.Max(p => p.Y);
            for (int y = 0; y <= maxy; ++y)
                
            {
                for (int x = 0; x <= maxx; ++x)
                {
                    if (grid.Contains(new Point(x, y)))
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("---------");
        }
    }
}
