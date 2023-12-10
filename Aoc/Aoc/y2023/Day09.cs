using Aoc.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day09 : DayBase
    {
        public Day09() : base(9)
        {
        }

        private IEnumerable<long> DiscreteDerivative(IReadOnlyList<long> rawSequence)
        {
            for (int i = 0; i < rawSequence.Count - 1; i++)
            {
                yield return rawSequence[i + 1] - rawSequence[i];
            }
        }

        public override void Solve()
        {
            var sequences = GetInputLines(false)
                .Select(l => Parser.IntegerListSigned().Map(e => e.ToList()).Parse(new Input(l)).Value)
                .ToList();

            var res = 0L;
            foreach (var s in sequences)
            {
                var d = s;
                var last = new List<long>();
                while (d.Any(n => n != 0))
                {
                    last.Add(d.Last());
                    d = DiscreteDerivative(d).ToList();
                }
                res += last.Sum();
            }
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var sequences = GetInputLines(false)
                .Select(l => Parser.IntegerListSigned().Map(e => e.ToList()).Parse(new Input(l)).Value)
                .ToList();

            var res = 0L;
            foreach (var s in sequences)
            {
                var d = s;
                var first = new List<long>();
                while (d.Any(n => n != 0))
                {
                    first.Add(d.First());
                    d = DiscreteDerivative(d).ToList();
                }
                first.Reverse();
                res += first.Aggregate(0L, (a, b) => b - a);
            }
            Console.WriteLine(res);
        }
    }
}
