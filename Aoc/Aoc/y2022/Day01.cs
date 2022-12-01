using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day01 : DayBase
    {
        public Day01() : base(1)
        {
        }

        private List<List<int>> GetInput()
        {
            var lines = GetInputLines(false).ToList();
            var next = new List<int>();
            var res = new List<List<int>>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    res.Add(next);
                    next = new List<int>();
                }
                else
                {
                    next.Add(int.Parse(line));
                }
            }

            return res;
        }

        public override void Solve()
        {
            var res = this.GetInput().Select(l => l.Sum()).OrderByDescending(i => i).Take(3).Sum();
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
