using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day06 : DayBase
    {
        public Day06() : base(6)
        {
        }

        public override void Solve()
        {
            var swarm = SplitInts(GetInputLines(false).First(), ',').GroupBy(n => n).ToDictionary(g => g.Key, g => g.LongCount());
            for (var i = 0; i < 256; ++i)
            {
                swarm = swarm.ToDictionary(kv => kv.Key - 1, kv => kv.Value);
                if (swarm.TryGetValue(-1, out var n))
                {
                    swarm.Remove(-1);
                    swarm.TryGetValue(6, out var cnt);
                    cnt += n;
                    swarm[6] = cnt;
                    swarm[8] = n;
                }
            }
            Console.WriteLine(swarm.Sum(kv => kv.Value));
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
