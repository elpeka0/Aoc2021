using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day08 : DayBase
    {
        private class Digit
        {
            private readonly bool[] segments;
            public Digit(int value, params bool[] s)
            {
                this.segments = s;
                this.Value = value;
            }

            public bool this[int index]
            {
                get => this.segments[index];
            }

            public int Value { get; set; }

            public int On => this.segments.Count(s => s);

            public void Render()
            {
                if (this.segments[0])
                {
                    Console.WriteLine(" aaaa ");
                }
                for (int i = 0; i < 2; ++i)
                {
                    var pattern = this.segments[1] ? "b    " : "     ";
                    if (this.segments[2])
                    {
                        pattern += "c";
                    }
                    Console.WriteLine(pattern);
                }
                if (this.segments[3])
                {
                    Console.WriteLine(" dddd ");
                }
                for (int i = 0; i < 2; ++i)
                {
                    var pattern = this.segments[4] ? "e    " : "     ";
                    if (this.segments[5])
                    {
                        pattern += "f";
                    }
                    Console.WriteLine(pattern);
                }
                if (this.segments[6])
                {
                    Console.WriteLine(" gggg ");
                }
            }
        }

        private readonly Digit[] digits = new[]
        {
            new Digit(0, true, true, true, false, true, true, true),
            new Digit(1, false, false, true, false, false, true, false),
            new Digit(2, true, false, true, true, true, false, true),
            new Digit(3, true, false, true, true, false, true, true),
            new Digit(4, false, true, true, true, false, true, false),
            new Digit(5, true, true, false, true, false, true, true),
            new Digit(6, true, true, false, true, true, true, true),
            new Digit(7, true, false, true, false, false, true, false),
            new Digit(8, true, true, true, true, true, true, true),
            new Digit(9, true, true, true, true, false, true, true)
        };

        public Day08() : base(8)
        {
        }

        public override void Solve()
        {
            var map = this.GetDigitMap();
            var cnt = 0;
            foreach (var (_, line) in this.GetInput())
            {
                foreach (var digit in line)
                {
                    if (map[digit.Length].Count == 1)
                    {
                        ++cnt;
                    }
                }
            }
            Console.WriteLine(cnt);
        }

        private IEnumerable<(List<string> All, List<string> Target)> GetInput()
        {
            return this.GetInputLines(false).Select(l => l.Split('|').Select(SplitDigitGroup).ToList()).Select(p => (p[0].Concat(p[1]).ToList(), p[1].ToList()));
        }

        private Dictionary<int, List<Digit>> GetDigitMap()
        {
            return this.digits.GroupBy(x => x.On).ToDictionary(g => g.Key, g => g.ToList());
        }

        public override void SolveMain()
        {
            var map = this.GetDigitMap();
            var sum = 0;
            foreach (var (all, target) in this.GetInput())
            {
                var mapping = this.FindMapping(all, map);
                var result = 0;
                foreach (var digit in target)
                {
                    result *= 10;
                    result += mapping[digit].Value;
                }
                sum += result;
            }
            Console.WriteLine(sum);
        }

        private Dictionary<(int, int, int), int> Score6 = new Dictionary<(int, int, int), int>
        {
            { (2, 3, 3), 0 },
            { (1, 3, 2), 6 },
            { (2, 4, 3), 9 },

            { (0, 3, 3), 0 },
            { (0, 3, 2), 6 },
            { (0, 4, 3), 9 },

            { (1, 0, 2), 6 },

            { (2, 3, 0), 0 },
            { (1, 3, 0), 6 },
            { (2, 4, 0), 9 },

            { (1, 0, 0), 6 },
            { (0, 4, 0), 9 },
            { (0, 0, 2), 6 }
        };

        private Dictionary<(int, int, int), int> Score5 = new Dictionary<(int, int, int), int>
        {
            { (1, 2, 2), 2 },
            { (2, 3, 3), 3 },
            { (1, 3, 2), 5 },

            { (0, 2, 2), 2 },
            { (0, 3, 3), 3 },
            { (0, 3, 2), 5 },

            { (2, 0, 3), 3 },

            { (1, 2, 0), 2 },
            { (2, 3, 0), 3 },
            { (1, 3, 0), 5 },

            { (2, 0, 0), 3 },
            { (0, 2, 0), 2 },
            { (0, 0, 3), 3 }
        };

        private Dictionary<string, Digit> FindMapping(List<string> all, Dictionary<int, List<Digit>> map)
        {
            var result = new Dictionary<string, Digit>();
            var back = new Dictionary<int, string>();

            foreach (var digit in all)
            {
                if (map[digit.Length].Count == 1)
                {
                    var actual = map[digit.Length][0];
                    result[digit] = actual;
                    back[actual.Value] = digit;
                }
            }

            int Score(int n, string digit)
            {
                return back.TryGetValue(n, out var s) ? this.Common(s, digit) : 0;
            }

            (int One, int Four, int Seven) ScoreAll(string digit)
            {
                return (Score(1, digit), Score(4, digit), Score(7, digit));
            }

            foreach (var digit in all)
            {
                if (!result.ContainsKey(digit))
                {
                    var score = ScoreAll(digit);

                    if (digit.Length == 6 && this.Score6.TryGetValue(score, out var d))
                    {
                        result[digit] = this.digits[d];
                    }
                    else if (digit.Length == 5 && this.Score5.TryGetValue(score, out var d2))
                    {
                        result[digit] = this.digits[d2];
                    }
                }
            }

            return result;
        }

        private int Common(string a, string b)
        {
            return a.Count(c => b.Contains(c));
        }

        private static IEnumerable<string> SplitDigitGroup(string s)
        {
            return s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
