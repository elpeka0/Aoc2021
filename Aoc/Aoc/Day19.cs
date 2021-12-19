using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day19 : DayBase
    {
        public Day19() : base(19)
        {
        }

        private struct Vector : IEquatable<Vector>
        {
            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public Vector(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Vector Reorient(int permutation)
            {
                switch (permutation)
                {
                    case 0: return new Vector(X, Y, Z);
                    case 1: return new Vector(X, Z, Y);
                    case 2: return new Vector(Y, X, Z);
                    case 3: return new Vector(Y, Z, X);
                    case 4: return new Vector(Z, X, Y);
                    case 5: return new Vector(Z, Y, X);
                    default: throw new InvalidOperationException();
                }
            }

            public Vector Scale(Vector scaling)
            {
                return new Vector(X * scaling.X, Y * scaling.Y, Z * scaling.Z);
            }

            public bool Equals(Vector other)
            {
                return other.X == X && other.Y == Y && other.Z == Z;
            }

            public override bool Equals(object obj)
            {
                return obj is Vector v && Equals(v);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }

            public static Vector operator +(Vector a, Vector b)
            {
                return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            }

            public static Vector operator -(Vector a, Vector b)
            {
                return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            }

            public override string ToString()
            {
                return $"{X}, {Y}, {Z}";
            }

            public static IEnumerable<Vector> Directions()
            {
                yield return new Vector(1, 1, 1);
                yield return new Vector(1, 1, -1);
                yield return new Vector(1, -1, 1);
                yield return new Vector(1, -1, -1);
                yield return new Vector(-1, 1, 1);
                yield return new Vector(-1, 1, -1);
                yield return new Vector(-1, -1, 1);
                yield return new Vector(-1, -1, -1);
            }
        }

        private class Scanner
        {
            public Vector Position { get; private set; }

            public HashSet<Vector> Beacons { get; private set; }

            public Scanner(IEnumerable<Vector> beacons)
            {
                this.Beacons = beacons.ToHashSet();
            }

            public bool CheckOverlap(Scanner other)
            {
                foreach (var a in other.Beacons)
                {
                    for (var orientation = 0; orientation < 6; ++orientation)
                    {
                        foreach (var scaling in Vector.Directions())
                        {
                            var normalized = Beacons.Select(v => v.Reorient(orientation).Scale(scaling)).ToList();
                            foreach (var b in normalized)
                            {
                                var offset = b - a;
                                if (normalized.Count(x => other.Beacons.Contains(x - offset)) >= 12)
                                {
                                    Beacons = normalized.Select(x => x - offset).ToHashSet();
                                    Position = offset;
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

        private List<Scanner> GetInput()
        {
            var res = new List<Scanner>();
            var l = new List<Vector>();
            foreach (var line in GetInputLines(false))
            {
                if (line.StartsWith("--"))
                {
                    if (l.Count > 0)
                    {
                        res.Add(new Scanner(l));
                        l.Clear();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    var p = line.Split(',');
                    l.Add(new Vector(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2])));
                }
            }
            res.Add(new Scanner(l));
            return res;
        }

        public override void Solve()
        {
            List<Scanner> solved = NormalizeScanners();
            Console.WriteLine(solved.SelectMany(s => s.Beacons).Distinct().Count());
        }

        private List<Scanner> NormalizeScanners()
        {
            var sw = new Stopwatch();
            sw.Start();
            var scanners = GetInput();
            var solved = new List<Scanner>();
            var processing = new Queue<Scanner>();
            processing.Enqueue(scanners[0]);
            scanners.RemoveAt(0);
            while (processing.Any())
            {
                var next = processing.Dequeue();
                foreach (var other in scanners.ToList())
                {
                    if (other.CheckOverlap(next))
                    {
                        scanners.Remove(other);
                        processing.Enqueue(other);
                    }
                }
                solved.Add(next);
                Console.WriteLine($"[S:{solved.Count} Q:{processing.Count}]");
            }
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");
            return solved;
        }

        public override void SolveMain()
        {
            var solved = NormalizeScanners();
            var max = 0;
            foreach (var a in solved)
            {
                foreach (var b in solved)
                {
                    var distance = b.Position - a.Position;
                    var manhattan = Math.Abs(distance.X) + Math.Abs(distance.Y) + Math.Abs(distance.Z);
                    if (manhattan > max)
                    {
                        max = manhattan;
                    }
                }
            }
            Console.WriteLine(max);
        }
    }
}
