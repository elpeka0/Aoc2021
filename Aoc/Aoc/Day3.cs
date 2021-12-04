using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day3 : DayBase
    {
        public Day3() : base(3)
        {
        }

        public override void Solve()
        {
            var original = this.GetInputLines(false).ToList();
            
            var gamma = original
                .Select(l => l.Select(c => c - '0'))
                .Aggregate((a, b) => a.Zip(b).Select(t => t.First + t.Second))
                .Reverse()
                .Select((b, i) => b > original.Count / 2 ? 1 << i : 0)
                .Sum();
            Console.WriteLine(gamma * ~(gamma | -1 << original[0].Length));
        }

        public override void SolveMain()
        {
            var original = this.GetInputLines(false).ToList();
            var intify = original
                .Select(
                    l => l
                        .Select(c => c - '0')
                        .Reverse()
                        .Select((b, i) => b << i)
                        .Sum()
                ).ToList();


            int Find(Func<int, int, bool> check)
            {
                var shift = original[0].Length - 1;
                var set = intify.ToList();
                while (set.Count != 1)
                {
                    var grouped = set.GroupBy(i => (i & (1 << shift)) >> shift).ToDictionary(g => g.Key, g => g.ToList());
                    var bit = grouped.ContainsKey(1) && check(grouped[1].Count, set.Count - grouped[1].Count) ? 1 : 0;
                    set = grouped[bit];
                    --shift;
                }

                return set[0];
            }

            var a = Find((on, off) => on >= off);
            var b = Find((on, off) => on < off);
            Console.WriteLine(a*b);
        }
    }
}
