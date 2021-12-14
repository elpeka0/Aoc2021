using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day14 : DayBase
    {
        private class Polymer
        {
            public string Chain { get; set; }

            public Dictionary<(char, char), char> Rules { get; set; }

            public void Transform()
            {
                this.Chain = this.Chain.Zip(this.Chain.Skip(1)).SelectMany(
                t =>
                {
                    if (Rules.TryGetValue((t.First, t.Second), out var c))
                    {
                        return new[] {t.First, c};
                    }
                    else
                    {
                        return new[] {t.First};
                    }
                }).Aggregate(string.Empty, (a, b) => a + b) + this.Chain.Last();
            }
        }

        private Polymer GetInput()
        {
            var l = GetInputLines(false).ToList();
            var res = new Polymer();
            res.Chain = l.First();
            res.Rules = l.Skip(2).Select(x => x.Split(" -> ")).ToDictionary(p => (p[0][0], p[0][1]), p => p[1][0]);
            return res;
        }

        public Day14() : base(14)
        {
        }

        public override void Solve()
        {
            var input = this.GetInput();
            for (var i = 0; i < 10; ++i)
            {
                input.Transform();
            }

            var g = input.Chain.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            var min = g.Values.Min();
            var max = g.Values.Max();
            Console.WriteLine(max - min);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
