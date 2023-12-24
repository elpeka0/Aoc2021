using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day24 : DayBase
    {
        public Day24() : base(24)
        {
        }

        private record Trajectory(LongVector Position, LongVector Velocity)
        {
            public DoubleVector? Intersection(Trajectory other)
            {
                var point = Position - other.Position;

                var matrix = new[]
                {
                    (D: point.X, A: Velocity.X, B: other.Velocity.X),
                    (D: point.Y, A: Velocity.Y, B: other.Velocity.Y),
                    (D: point.Z, A: Velocity.Z, B: other.Velocity.Z)
                };

                var equations = FindUseableEquations();

                if (equations == null)
                {
                    return null;
                }

                var (e0, e1) = (matrix[equations.Value.Item1], matrix[equations.Value.Item2]);

                // D0 + R*A0 = S*B0
                // D1 + R*A1 = S*B1
                // S = (D0 + R*A0)/B0
                // D1*B0 + R*A1*B0 = B1*D0 + B1*A0*R
                // R*A1*B0 - R*A0*B1 = B1*D0 - B0*D1
                // R = (B1*D0 - B0*D1)/(A1*B0 - A0*B1)

                var r = (e1.B * e0.D - e0.B * e1.D) / (double) (e1.A * e0.B - e0.A * e1.B);
                var s = (e0.D + r * e0.A) / (double) e0.B;
                var res = (DoubleVector)Position + r * (DoubleVector)Velocity;
                /*var check = other.Position + s * other.Velocity;
                if (res == check)
                {
                    return res;
                }*/

                if (r > 0 && s > 0)
                {
                    return res;
                }

                return null;

                (int, int)? FindUseableEquations()
                {
                    for (var i0 = 0; i0 < matrix.Length; i0++)
                    {
                        for (var i1 = 0; i1 < matrix.Length; i1++)
                        {
                            if (i0 != i1 
                                && matrix[i0].B != 0 
                                && matrix[i1].A != 0
                                && (matrix[i1].A * matrix[i0].B - matrix[i0].A * matrix[i1].B) != 0)
                            {
                                return (i0, i1);
                            }
                        }
                    }

                    return null;
                }
            }
        }

        private List<Trajectory> Load()
        {
            var vec = IntegerSigned()
                .RepeatWithSeperatorWs(",")
                .Map(e =>
                {
                    var a = e.ToArray();
                    return new LongVector(a[0], a[1], a[2]);
                });
            return vec
                .ThenWs("@")
                .ThenWs(vec)
                .Map(t => new Trajectory(t.Item1, t.Item2))
                .Evaluate(GetInputLines(false).ToList())
                .ToList();
        }

        private const long Min = 200000000000000;
        private const long Max = 400000000000000;

        public override void Solve()
        {
            var all = Load()
                .Select(t => new Trajectory(new LongVector(t.Position.X, t.Position.Y), new LongVector(t.Velocity.X, t.Velocity.Y)))
                .ToList();
            var cnt = 0;
            for(var i = 0;i < all.Count;i++)
            {
                for (var j = i + 1; j < all.Count; j++)
                {
                    if (all[i].Intersection(all[j]) is { } p)
                    {
                        if (p.X >= Min && p.X <= Max
                            && p.Y >= Min && p.Y <= Max)
                        {
                            cnt++;
                        }
                    }
                }
            }
            Console.WriteLine(cnt);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
