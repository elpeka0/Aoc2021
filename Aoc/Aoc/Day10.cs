using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day10 : DayBase
    {
        private Dictionary<char, char> tokens = new Dictionary<char, char>
        {
            {'(', ')'},
            {'[', ']'},
            {'<', '>'},
            {'{', '}'}
        };

        private Dictionary<char, int> scores1 = new Dictionary<char, int>
        {
            {')', 3},
            {']', 57},
            {'}', 1197},
            {'>', 25137}
        };

        private Dictionary<char, int> scores2 = new Dictionary<char, int>
        {
            {')', 1},
            {']', 2},
            {'}', 3},
            {'>', 4}
        };

        public Day10() : base(10)
        {
        }

        private (string Tail, char Break) ValidateLine(string line)
        {
            var stack = new Stack<char>();
            foreach (var c in line)
            {
                if (this.tokens.TryGetValue(c, out var close))
                {
                    stack.Push(close);
                }
                else
                {
                    if (stack.Pop() != c)
                    {
                        return (null, c);
                    }
                }
            }
            return (stack.Aggregate(string.Empty, (s, x) => s + x), ' ');
        }

        public override void Solve()
        {
            var sum = 0;
            foreach (var line in GetInputLines(false))
            {
                var (tail, c) = ValidateLine(line);
                if (tail == null)
                {
                    sum += this.scores1[c];
                }
            }
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var total = new List<long>();
            foreach (var line in GetInputLines(false))
            {
                var (tail, _) = ValidateLine(line);
                if (tail != null)
                {
                    total.Add(tail.Select(x => this.scores2[x]).Aggregate(0L, (a, b) => 5 * a + b));
                }
            }

            total = total.OrderBy(i => i).ToList();
            Console.WriteLine(total[total.Count / 2]);
        }
    }
}
