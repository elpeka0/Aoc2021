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
            var code = string.Join("?", Enumerable.Range(0, 5).Select(_ => s.Code));
            var l = new List<long>();
            for (var i = 0; i < 5; ++i)
            {
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

        private long CountMatchesOld(ReadOnlySpan<char> code, ReadOnlySpan<long> groups)
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

        private long CountMatches(string code, IReadOnlyList<long> groups)
        {
            var lookup = new Dictionary<(int, int), long>();
            var sw = new Stopwatch();
            sw.Start();
            return Impl(0, 0);

            long Impl(int codePos, int groupPos)
            {
                var key = (codePos, groupPos);
                if (sw.ElapsedMilliseconds > 5000)
                {
                    throw new TimeoutException();
                }

                if (lookup.TryGetValue(key, out var v))
                {
                    return v;
                }

                var g = (int)groups[groupPos];
                var res = 0L;
                while (code.Length - codePos >= g + 2)
                {
                    if (IsMatch(code.AsSpan(codePos, g + 2)))
                    {
                        if (groupPos < groups.Count - 1)
                        {
                            res += Impl(codePos + g + 1, groupPos + 1);
                        }
                        else
                        {
                            if (code.IndexOf('#', codePos + g + 2) == -1)
                            {
                                res++;
                            }
                        }
                    }

                    codePos++;

                    if (code[codePos] == '#')
                    {
                        break;
                    }
                }

                lookup[key] = res;
                return res;
            }
        }

        private long CountMatches(State line)
        {
            var res = CountMatches($".{line.Code}.", line.Groups.ToArray());
            Console.WriteLine($"{line} {res}");
            return res;
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
