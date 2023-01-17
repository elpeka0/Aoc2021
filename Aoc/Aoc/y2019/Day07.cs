using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2019
{
    internal class Day07 : DayBase
    {
        public Day07() : base(7)
        {
        }

        private long Try(int phase, long input)
        {
            var res = 0L;
            var i = new IntCodeInterpreter(
                GetInputLines(false).First(), 
                IntCodeInterpreter.MakeInput(phase, input),
                v => res = v);
            i.Run();
            return res;
        }

        private IEnumerable<int[]> GetSequences()
        {
            var setup = new[] { 0, 0, 0, 0, 0 };
            while (true)
            {
                if (setup.Distinct().Count() == setup.Length)
                {
                    yield return setup.ToArray();
                }

                var d = 4;
                var c = 1;
                while (c == 1 && d >= 0)
                {
                    setup[d] += c;
                    c = 0;
                    if (setup[d] > 4)
                    {
                        c = 1;
                        setup[d] = 0;
                        d--;
                    }
                }

                if (c == 1)
                {
                    yield break;
                }
            }
        }

        public override void Solve()
        {
            var best = 0L;
            foreach(var setup in GetSequences())
            {
                var input = 0L;
                for (var i = 0; i < setup.Length; i++)
                {
                    input = Try(setup[i], input);
                }

                if (input > best)
                {
                    best = input;
                }
            }

            Console.WriteLine(best);
        }

        public override void SolveMain()
        {
            var best = 0L;
            foreach (var setup in GetSequences())
            {
                var channels = setup.Select((_, i) => IntCodeInterpreter.MakeChannel(i, false)).ToList();
                var amps = setup.Select((_, i) => new IntCodeInterpreter(GetInputLines(false).First(), channels[(i + 1) % channels.Count].Receive, channels[i].Send)).ToList();

                var all = Task.WhenAll(amps.Select(a => Task.Run(a.Run)));

                for (var i = 0; i < setup.Length; i++)
                {
                    channels[i].Send(setup[i] + 5);
                }

                channels[0].Send(0);
                all.Wait();

                var res = channels[0].Receive();

                if (res > best)
                {
                    best = res;
                }
            }

            Console.WriteLine(best);
        }
    }
}
