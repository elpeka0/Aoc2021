using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day22 : DayBase
    {
        public Day22() : base(22)
        {
        }

        private struct Cuboid
        {
            public Vector From { get; }
            public Vector To { get; }
            public bool On { get; }

            public Cuboid(Vector from, Vector to, bool on)
            {
                this.From = from;
                this.To = to;
                this.On = on;
            }

            public Cuboid(int fx, int fy, int fz, int tx, int ty, int tz, bool on)
            {
                this.From = new Vector(fx, fy, fz);
                this.To = new Vector(tx, ty, tz);
                this.On = on;
            }

            public Cuboid GetOverlap(Cuboid other)
            {
                return new Cuboid(
                    Math.Max(this.From.X, other.From.X),
                    Math.Max(this.From.Y, other.From.Y),
                    Math.Max(this.From.Z, other.From.Z),
                    Math.Min(this.To.X, other.To.X),
                    Math.Min(this.To.Y, other.To.Y),
                    Math.Min(this.To.Z, other.To.Z),
                    other.On
                );
            }

            public bool IsValid => this.From.X <= this.To.X && this.From.Y <= this.To.Y && this.From.Z <= this.To.Z;

            private IEnumerable<Cuboid> HandleOverlapInternal(Cuboid inner)
            {
                // Floor
                yield return new Cuboid(this.From.X, this.From.Y, this.From.Z, this.To.X, this.To.Y, inner.From.Z - 1, this.On);
                // Ceiling
                yield return new Cuboid(this.From.X, this.From.Y, inner.To.Z + 1, this.To.X, this.To.Y, this.To.Z, this.On);
                // Left
                yield return new Cuboid(this.From.X, this.From.Y, inner.From.Z, inner.From.X - 1, this.To.Y, inner.To.Z, this.On);
                // Right
                yield return new Cuboid(inner.To.X + 1, this.From.Y, inner.From.Z, this.To.X, this.To.Y, inner.To.Z, this.On);
                // Down
                yield return new Cuboid(inner.From.X, this.From.Y, inner.From.Z, inner.To.X, inner.From.Y - 1, inner.To.Z, this.On);
                // Up
                yield return new Cuboid(inner.From.X, inner.To.Y + 1, inner.From.Z, inner.To.X, this.To.Y, inner.To.Z, this.On);
            }

            public IEnumerable<Cuboid> HandleOverlap(Cuboid other)
            {
                var intersection = this.GetOverlap(other);
                if (intersection.IsValid)
                {
                    foreach (var o in this.HandleOverlapInternal(intersection).Where(x => x.IsValid))
                    {
                        yield return o;
                    }
                }
                else
                {
                    yield return this;
                }
            }

            public long PointCount => 
                ((long)this.To.X - this.From.X + 1) * 
                ((long)this.To.Y - this.From.Y + 1) *
                ((long)this.To.Z - this.From.Z + 1);

            public override string ToString()
            {
                return $"{this.From} -> {this.To}";
            }
        }

        private List<Cuboid> GetInput()
        {
            var res = new List<Cuboid>();
            foreach (var line in this.GetInputLines(false))
            {
                var idx = line.IndexOf("x");
                var on = idx == 3;
                var rest = line.Substring(idx);
                var parts = rest.Split(',');
                var parsed = parts.Select(p => p.Substring(2).Split("..").Select(s => int.Parse(s)).ToArray()).Select(p => (p[0], p[1])).ToList();
                var from = new Vector(parsed[0].Item1, parsed[1].Item1, parsed[2].Item1);
                var to = new Vector(parsed[0].Item2, parsed[1].Item2, parsed[2].Item2);
                res.Add(new Cuboid(from, to, on));
            }

            return res;
        }

        public override void Solve()
        {
            var input = this.GetInput();
            var result = new Queue<Cuboid>();
            foreach (var i in input)
            {
                var next = new Queue<Cuboid>();
                while (result.Any())
                {
                    var current = result.Dequeue();
                    foreach (var o in current.HandleOverlap(i))
                    {
                        next.Enqueue(o);
                    }
                }
                next.Enqueue(i);
                result = next;
            }

            var sum = result.Where(c => c.On).Sum(c => c.PointCount);
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            this.Solve();
        }
    }
}
