using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day17 : DayBase
    {
        private record Instruction(int Code, int Operand);

        private class Machine
        {
            private bool firstWrite = true;

            public long A { get; private set; }
            public long B { get; private set; }
            public long C { get; private set; }
            public int Ip { get; private set; }

            public IReadOnlyList<Instruction> Instructions { get; }

            public Machine(long a, long b, long c, IReadOnlyList<Instruction> instructions)
            {
                A = a;
                B = b;
                C = c;
                Instructions = instructions;
            }

            public bool Step()
            {
                if (Ip >= Instructions.Count)
                {
                    return false;
                }

                var i = Instructions[Ip];
                switch (i.Code)
                {
                    case 0:
                        A = A / (1<<(int)Combo()); break;
                    case 1:
                        B = B ^ i.Operand; break;
                    case 2:
                        B = Combo() % 8; break;
                    case 3 when A != 0:
                        Ip = i.Operand - 1; break;
                    case 3:
                        break;
                    case 4:
                        B = B ^ C; break;
                    case 5:
                        if (!firstWrite)
                        {
                            Console.Write(",");
                        }
                        firstWrite = false;
                        Console.Write(Combo()%8);
                        break;
                    case 6:
                        B = A / (1 << (int)Combo()); break;
                    case 7:
                        C = A / (1 << (int)Combo()); break;

                }
                Ip++;
                return true;

                long Combo() => i.Operand switch
                {
                    < 4 => i.Operand,
                    4 => A,
                    5 => B,
                    6 => C,
                    _ => throw new InvalidOperationException("Illegal operand")
                };
            }
        }

        private Machine Load()
        {
            var lines = GetInputLines().ToList();
            var re = new Regex(@"\d+");
            long Reg(int n) => long.Parse(re.Match(lines[n]).Value);
            var a = Reg(0);
            var b = Reg(1);
            var c = Reg(2);

            var i = SplitInts(lines[4].Substring("Program: ".Length), ',').ToList();
            var l = new List<Instruction>();
            for (var n = 0; n < i.Count; n += 2)
            {
                l.Add(new Instruction(i[n], i[n + 1]));
            }

            return new Machine(a, b, c, l);
        }

        public Day17() : base(17)
        {
        }

        public override void Solve()
        {
            var m = Load();
            while (m.Step())
            { 
            }
        }

        public override void SolveMain()
        {
            var m = Load();
            //Compiled(m.A);
            Console.WriteLine();
            var target = m.Instructions.SelectMany(i => new[] { i.Code, i.Operand }).ToList();
            Console.WriteLine(string.Join(',', target));

            var res = FindMatch(target, 0).First();

            Console.WriteLine(string.Join(',', Compiled(res)));
            Console.WriteLine(res);
        }

        private IEnumerable<long> FindMatch(List<int> target, int pos)
        {
            if (pos >= target.Count)
            {
                yield return 0L;
            }
            foreach (var t in FindMatch(target, pos + 1))
            {
                for (var n = 0; n < 8; n++)
                {
                    var test = (t << 3) | (long)n;
                    var l = Compiled(test).ToList();
                    if (l.Count > 0 && l[0] == target[pos])
                    {
                        yield return test;
                    }
                }
            }
        }

        private IEnumerable<int> Compiled(long a)
        {
            while (a > 0)
            {
                var b = (a % 8) ^ 3;
                var c = a / (1 << (int)b);
                a /= 8;
                b ^= c;
                b ^= 5;
                yield return (int)(b % 8);
            }
        }

        // 0 => 000 => 011 => 3
        // 1 => 001 => 010 => 2
        // 2 => 010 => 001 => 1
        // 3 => 011 => 000 => 0
        // 4 => 100 => 111 => 7
        // 5 => 101 => 110 => 6
        // 6 => 110 => 101 => 5
        // 7 => 111 => 100 => 4
    }
}
