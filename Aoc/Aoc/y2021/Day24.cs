using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
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
                this.Input = input;
            }

            private (string Instruction, Action<long> Target, long A, long B) Decode(string line)
            {
                var parts = line.Split(' ');
                Action<long> target = l => this.Reg[parts[1][0] - 'w'] = l;
                var a = this.Reg[parts[1][0] - 'w'];
                var b = 0L;
                if(parts.Length > 2 && !long.TryParse(parts[2], out b))
                {
                    b = this.Reg[parts[2][0] - 'w'];
                }
                return (parts[0], target, a, b);
            }

            public bool Evaluate(string line)
            {
                var (instruction, target, a, b) = this.Decode(line);
                switch(instruction)
                {
                    case "inp":
                        if (this.Pos >= this.Input.Length) return false;
                        target(this.Input[this.Pos++] - '0');
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
                return $"W={this.Reg[0]} X={this.Reg[1]} Y={this.Reg[2]} Z={this.Reg[3]}";
            }
        }

        public override void Solve()
        {
            var buffer = this.GetInputLines(false);
            var population = new List<(long Value, long Score, int Generation)>();
            var seed = 99999999999999;
            population.Add((seed, Execute(buffer, seed), 0));
            while (true)
            {
                population = population.OrderBy(t => t.Score).ThenByDescending(t => t.Value).Take(1000).ToList();
                foreach(var p in population.ToList())
                {
                    var l = this.Mutate(p.Value);
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
            var buffer = this.GetInputLines(false);
            var population = new List<(long Value, long Score, int Generation)>();
            var seed = 11111111111111;
            population.Add((seed, Execute(buffer, seed), 0));
            while (true)
            {
                population = population.OrderBy(t => t.Score).ThenBy(t => t.Value).Take(1000).ToList();
                foreach (var p in population.ToList())
                {
                    var l = this.Mutate(p.Value);
                    population.Add((l, Execute(buffer, l), 0));
                }
                Console.WriteLine(population[0]);
            }
        }
    }
}
