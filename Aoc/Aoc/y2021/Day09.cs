using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day09 : DayBase
    {
        private class Point
        {
            public int Value { get; set; }

            public List<Point> Basin { get; set; }
        }

        private class Board
        {
            public Point[,] Map { get; set; }

            public int MaxX { get; set; }

            public int MaxY { get; set; }

            public Board(int[,] map, int maxX, int maxY)
            {
                this.MaxX = maxX;
                this.MaxY = maxY;
                this.Map = new Point[maxX, maxY];
                for (var x = 0; x < this.MaxX; ++x)
                {
                    for (var y = 0; y < this.MaxY; ++y)
                    {
                        this.Map[x, y] = new Point
                        {
                            Value = map[x, y]
                        };
                    }
                }
            }

            public IEnumerable<Point> Neighbors(int x, int y)
            {
                if (x > 0)
                {
                    yield return this.Map[x - 1, y];
                }

                if (x < this.MaxX - 1)
                {
                    yield return this.Map[x + 1, y];
                }

                if (y > 0)
                {
                    yield return this.Map[x, y - 1];
                }

                if (y < this.MaxY - 1)
                {
                    yield return this.Map[x, y + 1];
                }
            }

            public IEnumerable<Point> GetPoints()
            {
                for (var x = 0; x < this.MaxX; ++x)
                {
                    for (var y = 0; y < this.MaxY; ++y)
                    {
                        yield return this.Map[x, y];
                    }
                }
            }
        }

        public Day09() : base(9)
        {
        }

        public override void Solve()
        {
            var board = this.GetInput();
            var sum = 0;
            for (int x = 0; x < board.MaxX; ++x)
            {
                for (int y = 0; y < board.MaxY; ++y)
                {
                    if (board.Neighbors(x, y).All(n => n.Value > board.Map[x, y].Value))
                    {
                        sum += board.Map[x, y].Value + 1;
                    }
                }
            }

            Console.WriteLine(sum);
        }

        private Board GetInput()
        {
            var lines = this.GetInputLines(false).ToList();
            var maxx = lines[0].Length;
            var maxy = lines.Count;
            var map = new int[lines[0].Length, lines.Count];


            for (int x = 0; x < maxx; ++x)
            {
                for (int y = 0; y < maxy; ++y)
                {
                    map[x, y] = lines[y][x] - '0';
                }
            }

            return new Board(map, maxx, maxy);
        }

        public override void SolveMain()
        {
            var board = this.GetInput();
            var anyChange = true;
            while (anyChange)
            {
                anyChange = false;
                for (int x = 0; x < board.MaxX; ++x)
                {
                    for (int y = 0; y < board.MaxY; ++y)
                    {
                        foreach (var n in board.Neighbors(x, y))
                        {
                            if (board.Map[x, y].Value != 9 && n.Value != 9 && n.Basin != null)
                            {
                                if (board.Map[x, y].Basin != n.Basin)
                                {
                                    this.Merge(board.Map[x, y], n);
                                    anyChange = true;
                                }
                            }

                        }

                        if (board.Map[x, y].Basin == null)
                        {
                            board.Map[x, y].Basin = new List<Point>();
                            anyChange = true;
                        }

                        if (!board.Map[x, y].Basin.Contains(board.Map[x, y]))
                        {
                            board.Map[x, y].Basin.Add(board.Map[x, y]);
                            anyChange = true;
                        }
                    }
                }
            }

            var top3 = board.GetPoints().Select(p => p.Basin).Distinct().OrderByDescending(b => b.Count).Take(3).Aggregate(1, (a, b) => a*b.Count);
            Console.WriteLine(top3);
        }

        private void Merge(Point a, Point b)
        {
            if (a.Basin == null)
            {
                a.Basin = b.Basin;
            }
            else if (b.Basin == null)
            {
                b.Basin = a.Basin;
            }
            else if (a.Basin.Count < b.Basin.Count)
            {
                TransferPoints(a, b);
            }
            else
            {
                TransferPoints(b, a);
            }
        }

        private static void TransferPoints(Point a, Point b)
        {
            foreach (var p in a.Basin)
            {
                if (!b.Basin.Contains(p))
                {
                    b.Basin.Add(p);
                }
            }

            a.Basin = b.Basin;
        }
    }
}
