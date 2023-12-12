using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;
using static Aoc.y2021.Day05;

namespace Aoc.y2023
{
    public class Day12 : DayBase
    {
        public Day12() : base(12)
        {
        }

        private record State(string Code, IReadOnlyList<long> Groups)
        {
            public override string ToString()
            {
                var g = string.Join(",", Groups.Select(g => g.ToString()));
                return $"{Code} {g}";
            }
        }

        private State Quintuple(State s)
        {
            var code = string.Empty;
            var l = new List<long>();
            for (var i = 0; i < 5; ++i)
            {
                code += s.Code;
                l.AddRange(s.Groups);
            }
            return new State(code, l);
        }

        private IEnumerable<State> Load()
        {
            return Char(c => c != ' ')
                .String()
                .ThenWs(Integer().RepeatWithSeperator(","))
                .Map(t => new State(t.Item1, t.Item2.ToList()))
                .Evaluate(GetInputLines(false));
        }

        private bool IsMatch(ReadOnlySpan<char> slice)
        {
            if (slice[0] == '#')
            {
                return false;
            }

            if (slice[^1] == '#')
            {
                return false;
            }

            for (var i = 1; i < slice.Length - 1; i++)
            {
                if (slice[i] == '.')
                {
                    return false;
                }
            }

            return true;
        }

        private long CountMatches(ReadOnlySpan<char> code, ReadOnlySpan<long> groups)
        {
            var sw = new Stopwatch();
            sw.Start();
            return Impl(code, groups);

            long Impl(ReadOnlySpan<char> code, ReadOnlySpan<long> groups)
            {
                if (sw.ElapsedMilliseconds > 5000)
                {
                    throw new TimeoutException();
                }

                var g = (int)groups[0];
                var res = 0L;
                while (code.Length >= g + 2)
                {
                    if (IsMatch(code.Slice(0, g + 2)))
                    {
                        if (groups.Length > 1)
                        {
                            res += Impl(code.Slice(g + 1), groups.Slice(1));
                        }
                        else
                        {
                            if (!code.Slice(g + 2).Contains('#'))
                            {
                                res++;
                            }
                        }
                    }

                    code = code.Slice(1);

                    if (code[0] == '#')
                    {
                        break;
                    }
                }

                return res;
            }
        }

        private long CountMatches(State line)
        {
            var parts = line.Code.Split('.');
            var groups = line.Groups.ToArray().AsSpan();
            var carry = new Dictionary<int, long> { [0] = 1 };

            foreach (var p in parts)
            {
                var l = new Dictionary<int, long>();
                foreach(var (o, m) in carry)
                {
                    FitParts($".{p}.", groups.Slice(o), (mm, oo) =>
                    {
                        l.TryGetValue(o + oo, out var s);
                        l[o + oo] = s + m * mm;
                    });
                }

                carry = l;
            }

            return carry.GetWithDefault(line.Groups.Count, 0L);

            void FitParts(ReadOnlySpan<char> code, ReadOnlySpan<long> groups, Action<long, int> yield)
            {
                var matches = 0L;
                var i = 1;
                for (; i <= groups.Length && matches == 0; i++)
                {
                    matches = CountMatches(code, groups.Slice(0, i));
                    if (matches > 0)
                    {
                        yield(matches, i);
                    }
                }

                if (!code.Contains('#'))
                {
                    yield(1, 0);
                }

                for (; i <= groups.Length; i++)
                {
                    matches = CountMatches(code, groups.Slice(0, i));
                    if (matches > 0)
                    {
                        yield(matches, i);
                    }
                }
            }
        }

        public override void Solve()
        {
            var lines = this.Load().ToList();
            var res = 0L;
            foreach (var line in lines)
            {
                res += CountMatches(line);
            }
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var lines = this.Load().ToList();
            var res = 0L;
            foreach (var line in lines.Select(Quintuple))
            {
                try
                {
                    res += CountMatches(line);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine(line);
                }
            }
            Console.WriteLine(res);
        }
    }
}
