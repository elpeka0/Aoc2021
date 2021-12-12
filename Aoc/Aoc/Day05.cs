using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day05 : DayBase
    {
        public struct Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Point AdvanceTo(Point to)
            {
                int x = X;
                int y = Y;
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
                return HashCode.Combine(X, Y);
            }

            public override bool Equals(object obj)
            {
                return obj is Point o && o.X == X && o.Y == Y;
            }

            public override string ToString()
            {
                return $"{X}, {Y}";
            }
        }

        public class Line
        {
            public Point From { get; }
            public Point To { get; }

            public Line(Point from, Point to)
            {
                From = from;
                To = to;
            }

            public bool IsHorizontal => From.Y == To.Y;

            public bool IsVertical => From.X == To.X;

            public IEnumerable<Point> GetPointsOnLine()
            {
                for (Point p = From; p.X != To.X || p.Y != To.Y; p = p.AdvanceTo(To))
                {
                    yield return p;
                }
                yield return To;
            }

            public override string ToString()
            {
                return $"{From} -> {To}";
            }
        }

        public Day05() : base(5)
        {
        }

        public override void Solve()
        {
            FindOverlaps(false);
        }

        private void FindOverlaps(bool diagonal)
        {
            var lines = GetInputLines(false)
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
            FindOverlaps(true);
        }
    }
}
