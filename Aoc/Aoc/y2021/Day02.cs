using System;
using System.Linq;

namespace Aoc.y2021
{
    public class Day02 : DayBase
    {
        public Day02() : base(2)
        {
        }

        public override void Solve()
        {
            var d = this.GetInputLines(false)
                .Select(s => s.Split(' '))
                .Select(p => (Direction: p[0], Value: int.Parse(p[1])))
                .GroupBy(t => t.Direction)
                .ToDictionary(g => g.Key, g => g.Select(t => t.Value).Sum());
            var horizontal = d["forward"];
            var vertical = d["down"] - d["up"];
            Console.WriteLine(horizontal * vertical);
        }

        public override void SolveMain()
        {
            var aim = 0;
            var pos = 0;
            var depth = 0;
            foreach (var t in this.GetInputLines(false)
                .Select(s => s.Split(' '))
                .Select(p => (Direction: p[0], Value: int.Parse(p[1]))))
            {
                if (t.Direction == "forward")
                {
                    pos += t.Value;
                    depth += t.Value * aim;
                }
                else if (t.Direction == "up")
                {
                    aim -= t.Value;
                }
                else
                {
                    aim += t.Value;
                }
            }
            Console.WriteLine(pos * depth);
        }
    }
}
