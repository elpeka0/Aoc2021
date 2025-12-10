using Aoc.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2025
{
    public class Day08 : DayBase
    {
        public Day08() : base(8)
        {
        }

        private record Circuit(List<Vector> Positions)
        {
            public override string ToString()
            {
                return $"{Positions.Count}";
            }
        }

        private readonly record struct Link(Vector A, Vector B);

        public override void Solve()
        {
            var positions = GetInputLines().Select(l => l.Split(',')).Select(p => new Vector(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]))).ToList();
            var circuits = positions.ToDictionary(p => p, p => new Circuit(new List<Vector> { p }));
            var distances = new List<(Link Link, double D)>();

            for(var i = 0; i < positions.Count; i++)
            {
                for(var j = i + 1; j < positions.Count; j++)
                {
                    var a = positions[i];
                    var b = positions[j];
                    distances.Add((new Link(a, b), (a - b).Abs()));
                }
            }

            var shortest = distances.OrderBy(x => x.D).ToList();

            foreach (var x in shortest.Take(1000))
            {
                var a = circuits[x.Link.A];
                var b = circuits[x.Link.B];

                if (a != b)
                {
                    a.Positions.AddRange(b.Positions);
                    foreach (var p in a.Positions)
                    {
                        circuits[p] = a;
                    }
                }
            }

            var ordered = OrderedCircuits();
            var res = ordered.Take(3).Aggregate(1, (a, b) => a * b.Positions.Count);
            Console.WriteLine(res);

            IEnumerable<Circuit> OrderedCircuits() => circuits.Values.Distinct().OrderByDescending(c => c.Positions.Count).ToList();
        }

        public override void SolveMain()
        {
            var positions = GetInputLines().Select(l => l.Split(',')).Select(p => new Vector(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]))).ToList();
            var circuits = positions.ToDictionary(p => p, p => new Circuit(new List<Vector> { p }));
            var distinct = circuits.Values.ToList();
            var distances = new List<(Link Link, double D)>();

            for (var i = 0; i < positions.Count; i++)
            {
                for (var j = i + 1; j < positions.Count; j++)
                {
                    var a = positions[i];
                    var b = positions[j];
                    distances.Add((new Link(a, b), (a - b).Abs()));
                }
            }

            var shortest = distances.OrderBy(x => x.D).ToList();

            foreach (var x in shortest)
            {
                var a = circuits[x.Link.A];
                var b = circuits[x.Link.B];

                if (a != b)
                {
                    a.Positions.AddRange(b.Positions);
                    distinct.Remove(b);
                    foreach (var p in a.Positions)
                    {
                        circuits[p] = a;
                    }
                    if (distinct.Count == 1)
                    {
                        Console.WriteLine(x.Link.A.X * x.Link.B.X);
                    }
                }
            }
        }
    }
}
