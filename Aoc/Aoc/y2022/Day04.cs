using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day04 : DayBase
    {
        public Day04() : base(4)
        {
        }

        private List<((int From,int To) Left, (int From,int To) Right)> GetInput()
        {
            return GetInputLines(false)
                .Select(l => 
                    l
                        .Split(',')
                        .Select(p=>p
                            .Split('-')
                            .Select(x => int.Parse(x))
                        )
                        .Select(p => (p.First(), p.Skip(1).First()))
                )
                .Select(p => (p.First(), p.Skip(1).First()))
                .ToList();
        }

        public override void Solve()
        {
            var res = 0;
            foreach (var pair in GetInput())
            {
                if (pair.Left.From <= pair.Right.To && pair.Left.To >= pair.Right.From)
                {
                    ++res;
                }
            }
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
        }
    }
}
