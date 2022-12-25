using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day25 : DayBase
    {
        private class SnafuNumber
        {
            private readonly sbyte[] digits;

            public SnafuNumber(string s)
            {
                digits = s
                    .Select(c => c switch
                    {
                        '=' => -2,
                        '-' => -1,
                        _ => c - '0'
                    })
                    .Select(d => (sbyte)d)
                    .Reverse()
                    .ToArray();
            }

            public SnafuNumber(sbyte[] digits)
            {
                this.digits = digits;
            }

            public static SnafuNumber operator +(SnafuNumber a, SnafuNumber b)
            {
                sbyte i = 0;
                sbyte carry = 0;
                var l = new List<sbyte>();

                (sbyte R, sbyte C) FullAdd(sbyte a, sbyte b, sbyte c)
                {
                    var r = (sbyte)(a + b + c);
                    if (r < -2) return ((sbyte)(r + 5), -1);
                    if (r > 2) return ((sbyte)(r - 5), 1);
                    return (r, 0);
                }

                while (i < a.digits.Length && i < b.digits.Length)
                {
                    sbyte n;
                    (n, carry) = FullAdd(a.digits[i], b.digits[i], carry);
                    l.Add(n);
                    ++i;
                }

                while (i < a.digits.Length)
                {
                    sbyte n;
                    (n, carry) = FullAdd(a.digits[i], 0, carry);
                    l.Add(n);
                    ++i;
                }

                while (i < b.digits.Length)
                {
                    sbyte n;
                    (n, carry) = FullAdd(0, b.digits[i], carry);
                    l.Add(n);
                    ++i;
                }

                if (carry != 0)
                {
                    l.Add(carry);
                }
                return new SnafuNumber(l.ToArray());
            }

            public override string ToString()
            {
                return string.Join(string.Empty, digits.Reverse().Select(d => d switch
                {
                    -2 => '=',
                    -1 => '-',
                    _ => (char)('0' + d)
                }));
            }
        }

        public Day25() : base(25)
        {
        }

        public override void Solve()
        {
            var res = GetInputLines(false).Select(s => new SnafuNumber(s)).Aggregate((a, b) => a + b);
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
