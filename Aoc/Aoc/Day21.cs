using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day21 : DayBase
    {
        private const int Start1 = 1;
        private const int Start2 = 5;
        //private const int Start1 = 4;
        //private const int Start2 = 8;

        public Day21() : base(21)
        {
        }

        private (int, int) PlayDeterministic()
        {
            var p1 = Start1 - 1;
            var p2 = Start2 - 1;
            var s1 = 0;
            var s2 = 0;
            var die = -1;
            var rolls = 0;

            int Next()
            {
                ++rolls;
                die = (die + 1) % 100;
                return die + 1;
            }

            while (s1 < 1000 && s2 < 1000)
            {
                p1 = (p1 + Next() + Next() + Next()) % 10;
                s1 += p1 + 1;
                if (s1 < 1000)
                {
                    p2 = (p2 + Next() + Next() + Next()) % 10;
                    s2 += p2 + 1;
                }
            }

            return (Math.Min(s1, s2), rolls);
        }

        private readonly int[] rolls = new[]
        {
            0,0,0,1,3,6,7,6,3,1
        };

        private (long, long) QuantumPlay(int p1, int p2, int s1, int s2, bool turn)
        {
            var w1 = 0L;
            var w2 = 0L;
            for (var d = 3; d <= 9; ++d)
            {
                var pp1 = p1;
                var pp2 = p2;
                var ss1 = s1;
                var ss2 = s2;
                if (turn)
                {
                    pp1 = (pp1 + d) % 10;
                    ss1 += pp1 + 1;
                    if (ss1 >= 21)
                    {
                        w1 += rolls[d];
                        ++this.iterCnt;
                        continue;
                    }
                }
                else
                {
                    pp2 = (pp2 + d) % 10;
                    ss2 += pp2 + 1;
                    if (ss2 >= 21)
                    {
                        w2 += rolls[d];
                        ++this.iterCnt;
                        continue;
                    }
                }

                var (sub1, sub2) = QuantumPlay(pp1, pp2, ss1, ss2, !turn);
                w1 += this.rolls[d] * sub1;
                w2 += this.rolls[d] * sub2;
            }

            return (w1, w2);
        }

        public override void Solve()
        {
            var (score, rolls) = this.PlayDeterministic();
            Console.WriteLine(score * rolls);
        }

        public override void SolveMain()
        {
            var (w1, w2) = QuantumPlay(Start1 - 1, Start2 - 1, 0, 0, true);
            Console.WriteLine(Math.Max(w1, w2));
        }
    }
}
