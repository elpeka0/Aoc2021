﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day11 : DayBase
    {
        private class Monkey
        {
            public List<long> Items { get; } = new List<long>();

            public Func<long, long> Transformer { get; set; }

            public int Divisor { get; set; }

            public Monkey TrueMonkey { get; set; }

            public Monkey FalseMonkey { get; set; }

            public long Counter { get; set; }

            public void ThrowStuff(int modulus)
            {
                foreach (var g in Items
                    .Select(Transformer)
                    .Select(n => n % modulus)
                    .GroupBy(i => i % Divisor == 0 ? TrueMonkey : FalseMonkey))
                {
                    g.Key.Items.AddRange(g);
                }
                Counter += Items.Count;
                Items.Clear();
            }
        }

        public Day11() : base(11)
        {
        }

        private List<Monkey> GetInput(int divide)
        {
            var res = new List<Monkey>();
            Monkey GetMonkey(int nr)
            {
                while (res.Count <= nr) res.Add(new Monkey());
                return res[nr];
            }

            var lines = GetInputLines(false).ToList();
            for (int mcnt = 0; mcnt < lines.Count; mcnt += 7)
            {
                var nr = int.Parse(lines[mcnt].Split()[1].Replace(":", ""));
                var monkey = GetMonkey(nr);
                string GetMeat(int lnr) => lines[mcnt + lnr + 1].Split(": ")[1];
                monkey.Items.AddRange(GetMeat(0).Split(", ").Select(long.Parse));
                var math = GetMeat(1).Split();
                long Arg(long n) => math[4] == "old" ? n : int.Parse(math[4]);
                monkey.Transformer = n => (math[3] switch
                {
                    "+" => n + Arg(n),
                    "*" => n * Arg(n),
                    _ => throw new InvalidOperationException("Op " + math[3])
                })/divide;
                monkey.Divisor = int.Parse(GetMeat(2).Split().Last());
                monkey.TrueMonkey = GetMonkey(int.Parse(GetMeat(3).Split().Last()));
                monkey.FalseMonkey = GetMonkey(int.Parse(GetMeat(4).Split().Last()));
            }

            return res;
        }

        private void SolveInternal(int divide, int rounds)
        {
            var monkeys = GetInput(divide);
            var modulus = monkeys.Aggregate(1, (a, b) => a * b.Divisor);

            for (int i = 0; i < rounds; ++i)
            {
                foreach (var monkey in monkeys)
                {
                    monkey.ThrowStuff(modulus);
                }
            }
            var ordered = monkeys.OrderByDescending(m => m.Counter).ToList();
            Console.WriteLine(ordered[0].Counter * ordered[1].Counter);
        }

        public override void Solve()
        {
            SolveInternal(3, 20);
        }

        public override void SolveMain()
        {
            SolveInternal(1, 10000);
        }
    }
}
