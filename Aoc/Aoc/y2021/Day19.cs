using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aoc.y2021
{
    public class Day19 : DayBase
    {
        public Day19() : base(19)
        {
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
                            var normalized = this.Beacons.Select(v => v.Reorient(orientation).Scale(scaling)).ToList();
                            foreach (var b in normalized)
                            {
                                var offset = b - a;
                                if (normalized.Count(x => other.Beacons.Contains(x - offset)) >= 12)
                                {
                                    this.Beacons = normalized.Select(x => x - offset).ToHashSet();
                                    this.Position = offset;
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
            foreach (var line in this.GetInputLines(false))
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
            List<Scanner> solved = this.NormalizeScanners();
            Console.WriteLine(solved.SelectMany(s => s.Beacons).Distinct().Count());
        }

        private List<Scanner> NormalizeScanners()
        {
            var sw = new Stopwatch();
            sw.Start();
            var scanners = this.GetInput();
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
            var solved = this.NormalizeScanners();
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
