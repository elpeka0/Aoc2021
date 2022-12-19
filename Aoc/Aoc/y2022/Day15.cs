using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day15 : DayBase
    {
        private record Sensor(int X, int Y, int Radius);

        public Day15() : base(15)
        {
        }

        private IEnumerable<Sensor> GetInput()
        {
            foreach (var line in GetInputLines(false))
            {
                var parts = line.Split('=');
                var x = Utils.ExtractInt(parts[1], true);
                var y = Utils.ExtractInt(parts[2], true);
                var sx = Utils.ExtractInt(parts[3], true);
                var sy = Utils.ExtractInt(parts[4], true);
                var radius = Math.Abs(x - sx) + Math.Abs(y - sy);
                yield return new Sensor(x, y, radius);
            }
        }

        private HashSet<(int A, int B)> ScanLine(int line, List<Sensor> sensors)
        {
            var covered = new HashSet<(int A, int B)>();

            void Merge(int a, int b)
            {
                foreach (var r in covered)
                {
                    if (r.A >= a && r.A <= b && r.B >= b)
                    {
                        b = r.A;
                    }
                    else if (r.B >= a && r.B <= b && r.A < a)
                    {
                        a = r.B;
                    }
                    else if (r.A >= a && r.A <= b && r.B >= a && r.B <= b)
                    {
                        Merge(a, r.A);
                        Merge(r.B, b);
                        return;
                    }
                    else if (a >= r.A && a <= r.B && b >= r.A && b <= r.B)
                    {
                        return;
                    }
                }
                if (a != b)
                {
                    covered.Add((a, b));
                }
            }

            foreach (var sensor in sensors)
            {
                var ydiff = Math.Abs(sensor.Y - line);
                if (ydiff < sensor.Radius)
                {
                    var range = sensor.Radius - ydiff;
                    Merge(sensor.X - range, sensor.X + range);
                }
            }

            return covered;
        }

        public override void Solve()
        {
            var covered = ScanLine(2000000, GetInput().ToList());
            Console.WriteLine(covered.Sum(c => c.B - c.A));
        }

        public override void SolveMain()
        {
            var max = 4000000;
            var sensors = GetInput().ToList();
            for (int line = 0; line < max; ++line)
            {
                var covered = ScanLine(line, sensors).OrderBy(s => s.A).ToList();
                var pos = 0;
                while (pos < covered.Count && covered[pos].B < 0)
                {
                    ++pos;
                }
                while (pos < covered.Count && covered[pos].A < max)
                {
                    if (covered[pos].B < max && pos + 1 < covered.Count && covered[pos + 1].A != covered[pos].B)
                    {
                        Console.WriteLine((covered[pos].B + 1L) * max + line);
                        return;
                    }
                    ++pos;
                }
            }
        }
    }
}
