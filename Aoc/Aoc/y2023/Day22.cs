using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day22 : DayBase
    {
        private class Brick
        {
            public Vector From { get; set; }

            public Vector To { get; set; }

            public string Tag { get; set; }

            public Brick(Vector from, Vector to)
            {
                this.From = from;
                this.To = to;
            }

            public IEnumerable<Vector> Shadow 
            {
                get 
                {
                    for (var x = From.X; x <= To.X; x++)
                    {
                        for (var y = From.Y; y <= To.Y; y++)
                        {
                            yield return new Vector(x, y);
                        }
                    }
                }
            }

            public bool Contains(Vector point)
            {
                return point.X.Between(From.X, To.X)
                    && point.Y.Between(From.Y, To.Y)
                    && point.Z.Between(From.Z, To.Z);
            }

            public bool Intersects(Brick other)
            {
                return From.X <= other.To.X && To.X >= other.From.X
                    && From.Y <= other.To.Y && To.Y >= other.From.Y
                    && From.Z <= other.To.Z && To.Z >= other.From.Z;
            }

            public List<Brick> Above { get; } = new List<Brick>();

            public List<Brick> Below { get; } = new List<Brick>();

            public override string ToString()
            {
                var s = string.Empty;
                if (Tag != null)
                {
                    s += Tag;
                }

                return s + $"{From} -> {To}";
            }
        }

        private IEnumerable<Brick> Load()
        {
            var vector = Integer()
                .ThenWs(",")
                .ThenWs(Integer())
                .ThenWs(",")
                .ThenWs(Integer())
                .Map(t => new Vector((int)t.Item1.Item1, (int)t.Item1.Item2, (int)t.Item2));
            var parser = vector
                .ThenWs("~")
                .ThenWs(vector)
                .Map(t =>
                {
                    var from = new Vector(
                        Math.Min(t.Item1.X, t.Item2.X),
                        Math.Min(t.Item1.Y, t.Item2.Y),
                        Math.Min(t.Item1.Z, t.Item2.Z)
                    );
                    var to = new Vector(
                        Math.Max(t.Item1.X, t.Item2.X),
                        Math.Max(t.Item1.Y, t.Item2.Y),
                        Math.Max(t.Item1.Z, t.Item2.Z)
                    );
                    return new Brick(from, to);
                });
            return parser
                .Evaluate(GetInputLines(false).ToList());
        }

        public Day22() : base(22)
        {
        }

        private IEnumerable<Brick> SimulateFall()
        {
            var raw = this.Load().ToList();
            for (var i = 'A'; i <= 'Z' && (i - 'A') < raw.Count; ++i)
            {
                raw[i - 'A'].Tag = i.ToString();
            }

            var bricks = raw
                .GroupBy(b => b.From.Z)
                .ToList();

            var ground = new Dictionary<Vector, List<Brick>>();

            foreach (var g in bricks.OrderBy(x => x.Key))
            {
                foreach (var b in g)
                {
                    var h = b.Shadow.Max(s => ground.GetWithDefault(s, null)?.Last().To.Z ?? 0) + 1;
                    var delta = b.From.Z - h;
                    Debug.Assert(delta >= 0);
                    b.From = new Vector(b.From.X, b.From.Y, b.From.Z - delta);
                    b.To = new Vector(b.To.X, b.To.Y, b.To.Z - delta);
                    foreach (var p in b.Shadow)
                    {
                        if (!ground.TryGetValue(p, out var l))
                        {
                            l = new List<Brick>();
                            ground[p] = l;
                        }

                        var below = l.LastOrDefault();
                        if (below != null && below.To.Z + 1 == b.From.Z)
                        {
                            b.Below.Add(below);
                            below.Above.Add(b);
                        }

                        l.Add(b);
                    }
                }
            }

            return bricks.SelectMany(g => g);
        }

        public override void Solve()
        {
            var res = this.SimulateFall().Count(b => b.Above.All(o => o.Below.Any(x => x != b)));
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var bricks = this.SimulateFall().ToList();
            var res = bricks.Sum(b => Dependencies(b).Count - 1);
            Console.WriteLine(res);
        }

        private HashSet<Brick> Dependencies(Brick b)
        {
            var q = new Queue<Brick>();
            var visited = new HashSet<Brick>();
            q.Enqueue(b);
            while (q.TryDequeue(out var next))
            {
                if (!visited.Contains(next) && (next.Below.All(visited.Contains) || next == b))
                {
                    visited.Add(next);
                    foreach (var a in next.Above)
                    {
                        q.Enqueue(a);
                    }
                }
            }

            return visited;
        }
    }
}
