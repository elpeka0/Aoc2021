using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Parsing;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day06 : DayBase
    {
        public Day06() : base(6)
        {
        }

        private record Race(long Time, long Distance)
        {
            public long CountWins()
            {
                var a = (long)Math.Ceiling((Time - Math.Sqrt(Time * Time - 4 * Distance)) / 2.0);
                if (a * (Time - a) == Distance)
                {
                    ++a;
                }

                return Time - 2*a + 1;
            }
        }

        private record State(IReadOnlyList<Race> races);

        private State Load()
        {
            var lines = GetInputLines(false).ToList();
            var times = "Time:".ThenWs(IntegerList()).Map(e => e.ToList()).Parse(new Input(lines[0])).Value;
            var distances = "Distance:".ThenWs(IntegerList()).Map(e => e.ToList()).Parse(new Input(lines[1])).Value;
            return new State(Enumerable.Range(0, times.Count).Select(i => new Race(times[i], distances[i])).ToList());
        }

        public override void Solve()
        {
            var state = this.Load();
            Console.WriteLine(state.races.Aggregate(1L, (a, b) => a * b.CountWins()));
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
