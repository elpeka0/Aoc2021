using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day21 : DayBase
    {
        private class KeyPad
        {
            public Grid<char> Keys { get; }

            public KeyPad(List<string> keyLines)
            {
                Keys = Grid<char>.FromLines(keyLines, c => c);
            }

            public string Type(string code)
            {
                var pos = Keys.Indexes().First(i => Keys[i] == 'A');
                var sb = new StringBuilder();
                foreach (var c in code)
                {
                    var target = Keys.Indexes().First(i => Keys[i] == c);
                    var intermediate1 = new Vector(target.X, pos.Y);
                    var intermediate2 = new Vector(pos.X, target.Y);
                    if (pos.X < target.X)
                    {
                        (intermediate1, intermediate2) = (intermediate2, intermediate1);
                    }
                    if (Keys[intermediate1] == '#')
                    {
                        Move(pos, intermediate2, sb);
                        Move(intermediate2, target, sb);
                    }
                    else
                    {
                        Move(pos, intermediate1, sb);
                        Move(intermediate1, target, sb);
                    }
                    sb.Append('A');
                    pos = target;
                }
                return sb.ToString();
            }

            private void Move(Vector from, Vector to, StringBuilder sb)
            {
                for (var i = 0; i < to.X - from.X; i++)
                {
                    sb.Append('>');
                }
                for (var i = 0; i < from.X - to.X; i++)
                {
                    sb.Append('<');
                }
                for (var i = 0; i < to.Y - from.Y; i++)
                {
                    sb.Append('v');
                }
                for (var i = 0; i < from.Y - to.Y; i++)
                {
                    sb.Append('^');
                }
            }
        }
        private KeyPad target = new KeyPad(new List<string>
        {
            "789",
            "456",
            "123",
            "#0A"
        });
        private KeyPad directional = new KeyPad(new List<string>
        {
            "#^A",
            "<v>"
        });

        public Day21() : base(21)
        {
        }

        public override void Solve()
        {
            var res = 0;
            foreach (var line in GetInputLines())
            {
                var code1 = target.Type(line);
                var code2 = directional.Type(code1);
                var code3 = directional.Type(code2);
                var num = int.Parse(line.Substring(0, line.Length - 1));
                var c = num * code3.Length;
                res += c;
            }
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var res = 0L;
            var cache = new Dictionary<(string, int), long>();
            foreach (var line in GetInputLines())
            {
                var code = target.Type(line);

                var sum = 0L;
                foreach (var part in Parts(code))
                {
                    sum += DetermineLength(part, 25);
                }

                var num = int.Parse(line.Substring(0, line.Length - 1));
                var c = num * sum;
                res += c;
            }
            Console.WriteLine(res);

            long DetermineLength(string p, int iterations)
            {
                if (cache.TryGetValue((p, iterations), out var result))
                {
                    return result;
                }

                var code = directional.Type(p);
                if (iterations == 1)
                {
                    cache[(p, 1)] = code.Length;
                    return code.Length;
                }
                var sum = 0L;
                foreach (var part in Parts(code))
                {
                    sum += DetermineLength(part, iterations - 1);
                }
                cache[(p, iterations)] = sum;
                return sum;
            }

            IEnumerable<string> Parts(string code)
            {
                var s = 0;
                for (var i = 0; i < code.Length; i++)
                {
                    if (code[i] == 'A')
                    {
                        yield return code.Substring(s, i - s + 1);
                        s = i + 1;
                    }
                }
            }
        }
    }
}
