using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day14 : DayBase
    {
        private class Polymer
        {
            public Dictionary<(char, char), long> Chain { get; set; } = new Dictionary<(char, char), long>();

            public Dictionary<(char, char), char> Rules { get; set; }

            public char Tail { get; set; }

            public void Transform()
            {
                var next = new Dictionary<(char, char), long>();

                void Update((char, char) key, long value)
                {
                    next.TryGetValue(key, out var cnt);
                    next[key] = cnt + value;
                }
                foreach (var (key, value) in this.Chain)
                {
                    if (this.Rules.TryGetValue(key, out var rule))
                    {
                        Update((key.Item1, rule), value);
                        Update((rule, key.Item2), value);
                    }
                    else
                    {
                        Update(key, value);
                    }
                }

                this.Chain = next;
            }

            public long Evaluate()
            {
                var eval = this.Chain.Select(kv => (kv.Key.Item1, kv.Value)).Concat(
                    new[]
                    {
                        (this.Tail, 1L)
                    }
                ).GroupBy(t => t.Item1).ToDictionary(g => g.Key, g => g.Sum(x => x.Item2));
                var min = eval.Values.Min();
                var max = eval.Values.Max();
                return max - min;
            }
        }

        private Polymer GetInput()
        {
            var l = this.GetInputLines(false).ToList();
            var res = new Polymer();
            foreach (var (a, b) in l[0].Zip(l[0].Substring(1)))
            {
                res.Chain.TryGetValue((a, b), out var cnt);
                res.Chain[(a, b)] = cnt + 1;
            }
            res.Rules = l.Skip(2).Select(x => x.Split(" -> ")).ToDictionary(p => (p[0][0], p[0][1]), p => p[1][0]);
            res.Tail = l[0].Last();
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
            Console.WriteLine(input.Evaluate());
        }

        public override void SolveMain()
        {
            var input = this.GetInput();
            for (var i = 0; i < 40; ++i)
            {
                input.Transform();
            }
            Console.WriteLine(input.Evaluate());
        }
    }
}
