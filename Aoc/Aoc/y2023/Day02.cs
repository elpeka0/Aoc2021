using Aoc.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day02 : DayBase
    {
        private enum Color
        {
            Green,
            Red,
            Blue
        }

        private record Game(int Id, List<Dictionary<Color, int>> Rounds);

        private Game Parse(string line)
        {
            var atom = Integer()
                .ThenWs(Enum<Color>());
            var round = atom
                .RepeatWithSeperatorWs(",")
                .Map(e => e.ToDictionary(t => t.Item2, t => t.Item1));
            var parser = "Game"
                .ThenWs(Integer())
                .ThenWs(":")
                .ThenWs(round.RepeatWithSeperatorWs(";"))
                .Map(t => new Game(t.Item1, t.Item2.ToList()));

            var res = parser.Parse(new Input(line)).Value;
            return res;
        }

        public Day02() : base(2)
        {
        }

        public override void Solve()
        {
            var sum = GetInputLines(false)
                .Select(Parse)
                .Where(game => game.Rounds.All(round => 
                    round.GetWithDefault(Color.Red, 0) <= 12 
                    && round.GetWithDefault(Color.Blue, 0) <= 14 
                    && round.GetWithDefault(Color.Green, 0) <= 13))
                .Sum(game => game.Id);
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var sum = GetInputLines(false)
                .Select(Parse)
                .Select(game => (
                    game.Rounds.Max(round => round.GetWithDefault(Color.Red, 0)), 
                    game.Rounds.Max(round => round.GetWithDefault(Color.Blue, 0)), 
                    game.Rounds.Max(round => round.GetWithDefault(Color.Green, 0))))
                .Select(t => t.Item1 * t.Item2 * t.Item3)
                .Sum();
            Console.WriteLine(sum);
        }
    }
}
