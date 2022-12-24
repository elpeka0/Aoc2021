using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day05 : DayBase
    {
        private class Instruction
        {
            public int From { get; }
            public int To { get; }
            public int Count { get; }

            public Instruction(int count, int from, int to)
            {
                this.Count = count;
                this.From = from;
                this.To = to;
            }

            public override string ToString()
            {
                return $"{Count} {From} -> {To}";
            }
        }

        private readonly List<Instruction> instructions = new List<Instruction>();
        private List<Stack<char>> stacks;

        public Day05() : base(5)
        {
        }

        private void GetInput()
        {
            var all = GetInputLines(false).ToList();
            var lp = 0;
            var s = new List<List<char>>();
            while (true)
            {
                var line = all[lp];
                for (int i = 0; i < line.Length; i += 4)
                {
                    var c = line[i + 1];
                    if (char.IsDigit(c))
                    {
                        lp += 2;
                        goto done;
                    }
                    
                    if (c != ' ')
                    {
                        var sp = i / 4;
                        while (s.Count < sp + 1)
                        {
                            s.Add(new List<char>());
                        }
                        s[sp].Insert(0, c);
                    }
                }

                ++lp;
            }
            done:
            this.stacks = s.Select(l => new Stack<char>(l)).ToList();
            for (; lp < all.Count; ++lp)
            {
                var i = all[lp].Split();
                this.instructions.Add(new Instruction(int.Parse(i[1]), int.Parse(i[3]), int.Parse(i[5])));
            }
        }

        public override void Solve()
        {
            this.GetInput();
            foreach (var i in this.instructions)
            {
                var tmp = new Stack<char>();
                for (var j = 0; j < i.Count; ++j)
                {
                    var c = this.stacks[i.From - 1].Pop();
                    tmp.Push(c);
                }
                for (var j = 0; j < i.Count; ++j)
                {
                    var c = tmp.Pop();
                    this.stacks[i.To - 1].Push(c);
                }
            }

            var res = this.stacks.Select(s => s.Peek()).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c));
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
        }
    }
}
