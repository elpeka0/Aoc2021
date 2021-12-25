using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day24 : DayBase
    {
        private static Random rand = new Random();

        public Day24() : base(24)
        {
        }

        private class Alu
        {
            public long[] Reg { get; } = new long[4];
            public int Pos { get; set; }
            public string Input { get; set; }

            public Alu(string input)
            {
                Input = input;
            }

            private (string Instruction, Action<long> Target, long A, long B) Decode(string line)
            {
                var parts = line.Split(' ');
                Action<long> target = l => Reg[parts[1][0] - 'w'] = l;
                var a = Reg[parts[1][0] - 'w'];
                var b = 0L;
                if(parts.Length > 2 && !long.TryParse(parts[2], out b))
                {
                    b = Reg[parts[2][0] - 'w'];
                }
                return (parts[0], target, a, b);
            }

            public bool Evaluate(string line)
            {
                var (instruction, target, a, b) = Decode(line);
                switch(instruction)
                {
                    case "inp":
                        if (Pos >= Input.Length) return false;
                        target(Input[Pos++] - '0');
                        break;
                    case "add":
                        target(a + b);
                        break;
                    case "mul":
                        target(a * b);
                        break;
                    case "div":
                        target(a / b);
                        break;
                    case "mod":
                        target(a % b);
                        break;
                    case "eql":
                        target(a == b ? 1 : 0);
                        break;
                }
                return true;
            }

            public override string ToString()
            {
                return $"W={Reg[0]} X={Reg[1]} Y={Reg[2]} Z={Reg[3]}";
            }
        }

        public override void Solve()
        {
            var buffer = GetInputLines(false);
            var population = new List<(long Value, long Score, int Generation)>();
            var seed = 99999999999999;
            population.Add((seed, Execute(buffer, seed), 0));
            while (true)
            {
                population = population.OrderBy(t => t.Score).ThenByDescending(t => t.Value).Take(1000).ToList();
                foreach(var p in population.ToList())
                {
                    var l = Mutate(p.Value);
                    population.Add((l, Execute(buffer, l), 0));
                }
                Console.WriteLine(population[0]);
            }
        }

        private long Mutate(long l)
        {
            var n = rand.Next(14);
            var p10 = (long) Math.Pow(10, n+1);
            var r10 = p10 / 10;
            var rem = l % r10;
            var div = l / p10;
            return div * p10 + (rand.Next(9) + 1)*r10 + rem;
        }

        private static long Execute(IEnumerable<string> buffer, long l)
        {
            var alu = new Alu(l.ToString());
            foreach (var line in buffer)
            {
                if (!alu.Evaluate(line))
                {
                    break;
                }
            }

            return alu.Reg[3];
        }

        public override void SolveMain()
        {
            var buffer = GetInputLines(false);
            var population = new List<(long Value, long Score, int Generation)>();
            var seed = 11111111111111;
            population.Add((seed, Execute(buffer, seed), 0));
            while (true)
            {
                population = population.OrderBy(t => t.Score).ThenBy(t => t.Value).Take(1000).ToList();
                foreach (var p in population.ToList())
                {
                    var l = Mutate(p.Value);
                    population.Add((l, Execute(buffer, l), 0));
                }
                Console.WriteLine(population[0]);
            }
        }
    }
}
