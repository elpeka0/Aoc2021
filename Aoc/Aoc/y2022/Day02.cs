using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day02 : DayBase
    {
        private enum RPS
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        public Day02() : base(2)
        {
        }

        private List<(char, char)> GetInput()
        {
            return GetInputLines(false)
                .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Select(a => (a[0][0], a[1][0])).ToList();
        }

        public override void Solve()
        {
            var score = 0;
            var rps = this.GetInput().Select(t => 
            (
                t.Item1 switch
                {
                    'A' => RPS.Rock,
                    'B' => RPS.Paper,
                    _ => RPS.Scissors
                },
                t.Item2 switch
                {
                    'X' => 0,
                    'Y' => 3,
                    _ => 6
                }
            ));
            foreach (var (a, r) in rps)
            {
                var b = Enum.GetValues<RPS>().First(x => Score(x, a) == r);

                score += (int)b;
                score += Score(b, a);
            }

            Console.WriteLine(score);
        }

        private int Score(RPS a, RPS b)
        {
            return (a, b) switch
            {
                (RPS.Rock, RPS.Rock) => 3,
                (RPS.Rock, RPS.Paper) => 0,
                (RPS.Rock, RPS.Scissors) => 6,
                
                (RPS.Paper, RPS.Rock) => 6,
                (RPS.Paper, RPS.Paper) => 3,
                (RPS.Paper, RPS.Scissors) => 0,
                
                (RPS.Scissors, RPS.Rock) => 0,
                (RPS.Scissors, RPS.Paper) => 6,
                (RPS.Scissors, RPS.Scissors) => 3,
            };
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
