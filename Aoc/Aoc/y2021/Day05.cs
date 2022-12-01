using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day05 : DayBase
    {
        public struct Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public Point AdvanceTo(Point to)
            {
                int x = this.X;
                int y = this.Y;
                if (x != to.X)
                {
                    x += Math.Sign(to.X - x);
                }
                if (y != to.Y)
                {
                    y += Math.Sign(to.Y - y);
                }
                return new Point(x, y);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.X, this.Y);
            }

            public override bool Equals(object obj)
            {
                return obj is Point o && o.X == this.X && o.Y == this.Y;
            }

            public override string ToString()
            {
                return $"{this.X}, {this.Y}";
            }
        }

        public class Line
        {
            public Point From { get; }
            public Point To { get; }

            public Line(Point from, Point to)
            {
                this.From = from;
                this.To = to;
            }

            public bool IsHorizontal => this.From.Y == this.To.Y;

            public bool IsVertical => this.From.X == this.To.X;

            public IEnumerable<Point> GetPointsOnLine()
            {
                for (Point p = this.From; p.X != this.To.X || p.Y != this.To.Y; p = p.AdvanceTo(this.To))
                {
                    yield return p;
                }
                yield return this.To;
            }

            public override string ToString()
            {
                return $"{this.From} -> {this.To}";
            }
        }

        public Day05() : base(5)
        {
        }

        public override void Solve()
        {
            this.FindOverlaps(false);
        }

        private void FindOverlaps(bool diagonal)
        {
            var lines = this.GetInputLines(false)
                            .Select(
                                l => l.Split("->", StringSplitOptions.TrimEntries)
                                    .Select(p => p.Split(','))
                                    .Select(a => new Point(int.Parse(a[0]), int.Parse(a[1])))
                            ).Select(l => new Line(l.First(), l.Skip(1).First()))
                            .ToList();
            var online = new Dictionary<Point, int>();
            foreach (var line in lines.Where(l => diagonal || l.IsHorizontal || l.IsVertical))
            {
                //Console.WriteLine(line);
                foreach (var p in line.GetPointsOnLine())
                {
                    if (!online.TryGetValue(p, out var cnt))
                    {
                        cnt = 0;
                    }
                    online[p] = cnt + 1;
                }
                //Visualize(online);
                Console.WriteLine();
            }
            //Visualize(online);
            var sum = online.Values.Count(v => v > 1);
            Console.WriteLine(sum);
        }

        public void Visualize(Dictionary<Point, int> map)
        {
            var maxx = map.Keys.Max(p => p.X);
            var maxy = map.Keys.Max(p => p.Y);

            for (int y = 0; y <= maxy; ++y)
            {
                for (int x = 0; x <= maxx; ++x)
                {
                    if (!map.TryGetValue(new Point(x, y), out var cnt))
                    {
                        cnt = 0;
                    }
                    Console.Write(cnt);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public override void SolveMain()
        {
            this.FindOverlaps(true);
        }
    }
}
