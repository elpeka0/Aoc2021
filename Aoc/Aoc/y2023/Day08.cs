using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Aoc.Parsing;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day08 : DayBase
    {
        public Day08() : base(8)
        {
        }

        private record Successor(string Left, string Right);

        private record State(string Instructions, Dictionary<string, Successor> Links)
        {
            public long FindPath(string from, Func<string, bool> targetCondition)
            {
                var node = from;
                var pos = 0;
                var n = 0L;
                while (!targetCondition(node))
                {
                    ++n;
                    node = Instructions[pos] switch
                    {
                        'L' => Links[node].Left,
                        'R' => Links[node].Right
                    };
                    pos = (pos + 1) % Instructions.Length;
                }
                return n;
            }
        }

        private State Read()
        {
            var l = GetInputLines(false).ToList();
            var i = l[0];

            var atom = Char(char.IsLetterOrDigit).String();
            var d = l.Skip(2).Select(line =>
                atom
                    .ThenWs("=")
                    .ThenWs("(")
                    .ThenWs(atom)
                    .ThenWs(",")
                    .ThenWs(atom)
                    .ThenWs(")")
                    .Map(t => (Root: t.Item1.Item1, Left: t.Item1.Item2, Right: t.Item2))
                    .Parse(new Input(line))
                    .Value)
                .ToDictionary(t => t.Root, t => new Successor(t.Left, t.Right));
            return new State(i, d);
        }

        private static long GCF(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private static long LCM(long a, long b)
        {
            return (a / GCF(a, b)) * b;
        }

        public override void Solve()
        {
            var state = Read();
            Console.WriteLine(state.FindPath("AAA", n => n == "ZZZ"));
        }

        public override void SolveMain()
        {
            var state = Read();

            var nodes = state.Links.Keys.Where(k => k.EndsWith('A')).ToList();
            var shortest = nodes.Select(n => state.FindPath(n, n => n.EndsWith('Z'))).ToList();

            var res = shortest.Aggregate(LCM);
            Console.WriteLine(res);
        }
    }
}
