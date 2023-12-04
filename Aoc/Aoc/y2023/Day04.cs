using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Parsing;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day04 : DayBase

    {
        private record Game(int Id, HashSet<int> Winning, HashSet<int> Mine)
        {
            public int Evaluate()
            {
                var count = Mine.Count(Winning.Contains);
                return count == 0
                    ? 0
                    : 1 << count - 1;
            }
        }

        private Game Parse(string line)
        {
            var set = 
                Integer()
                    .ThenOther(Whitespace())
                    .Repeat()
                    .Map(e => e.ToHashSet());
            var x = set.Parse(new Input("1 2 3"));
            var parser = "Card"
                .ThenWs(Integer())
                .ThenWs(":")
                .ThenWs(set)
                .ThenWs("|")
                .ThenWs(set)
                .Map(t => new Game(t.Item1.Item1, t.Item1.Item2, t.Item2));
            return parser.Parse(new Input(line)).Value;
        }

        public Day04() : base(4)
        {
        }

        public override void Solve()
        {
            var games = GetInputLines(false).Select(Parse);
            var sum = games.Sum(g => g.Evaluate());
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var games = GetInputLines(false)
                .Select(Parse)
                .OrderBy(g => g.Id)
                .ToList();
            var counter = games.ToDictionary(g => g.Id, _ => 1L);
            foreach (var game in games)
            {
                var n = 1;
                foreach (var next in game.Mine.Where(game.Winning.Contains))
                {
                    counter[game.Id + n] += counter[game.Id];
                    ++n;
                }
            }

            Console.WriteLine(counter.Sum(kv => kv.Value));
        }
    }
}
