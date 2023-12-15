using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day15 : DayBase
    {
        public Day15() : base(15)
        {
        }

        private static int Hash(string s)
        {
            var n = 0;
            foreach (var c in s)
            {
                n = ((n + c) * 17) % 256;
            }

            return n;
        }

        private List<string> Load()
        {
            var line = GetInputLines(false).First();
            return Char(c => char.IsLetterOrDigit(c) || c == '=' || c == '-')
                .String()
                .RepeatWithSeperator(",")
                .Map(e => e.ToList())
                .Evaluate(line);
        }

        private record Entry(string Key, int Focus);

        private List<Entry> LoadMain()
        {
            var line = GetInputLines(false).First();
            return Char(c => char.IsLetterOrDigit(c))
                .String()
                .ThenWs(Literal("-").Map(_ => -1L).Or("=".ThenWs(Integer())))
                .Map(t => new Entry(t.Item1, (int)t.Item2))
                .RepeatWithSeperator(",")
                .Map(e => e.ToList())
                .Evaluate(line);
        }

        public override void Solve()
        {
            var res = this.Load().Select(Hash).Sum();
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var entries = Enumerable.Range(0, 256).Select(_ => new List<Entry>()).ToArray();
            var commands = this.LoadMain();
            foreach (var cmd in commands)
            {
                var hc = Hash(cmd.Key);
                if (cmd.Focus == -1)
                {
                    entries[hc].RemoveAll(x => x.Key == cmd.Key);
                }
                else
                {
                    var i = entries[hc].FindIndex(e => e.Key == cmd.Key);
                    if (i >= 0)
                    {
                        entries[hc][i] = cmd;
                    }
                    else
                    {
                        entries[hc].Add(cmd);
                    }
                }
            }

            var res = entries
                .Select((l, i) => (l, i))
                .Sum(t => t.l.Select((s, j) => (s, j)).Sum(k => (1 + t.i) * (1 + k.j) * k.s.Focus));
            Console.WriteLine(res);
        }
    }
}
