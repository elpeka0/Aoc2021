using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day01 : DayBase
    {
        public Day01() : base(1)
        {
        }

        private (List<int>, List<int>) Read()
        {
            var a = new List<int>();
            var b = new List<int>();
            foreach (var line in GetInputLines(false))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                a.Add(parts[0]);
                b.Add(parts[1]);
            }

            return (a, b);
        }

        public override void Solve()
        {
            var (a, b) = Read();
            a.Sort();
            b.Sort();
            Console.WriteLine(a.Select((_, i) => Math.Abs(a[i] - b[i])).Sum());
        }

        public override void SolveMain()
        {
            var (a, b) = Read();
            var map = b.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());
            Console.WriteLine(a.Select(n => map.TryGetValue(n, out var c) ? n*c : 0).Sum());
        }
    }
}
