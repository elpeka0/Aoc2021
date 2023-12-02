using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var parts = line.Split(':');
            var id = int.Parse(parts[0].Split(' ')[1]);
            parts = parts[1].Split(';');
            var l = new List<Dictionary<Color, int>>();
            foreach (var p in parts)
            {
                var sub = p.Split(',');
                var d = new Dictionary<Color, int>()
                {
                    [Color.Green] = 0,
                    [Color.Red] = 0,
                    [Color.Blue] = 0
                };
                foreach (var s in sub)
                {
                    var a = s.Split();
                    var nr = int.Parse(a[1]);
                    var color = (Color) Enum.Parse(typeof(Color), a[2], true);
                    d[color] = nr;
                }
                l.Add(d);
            }
            return new Game(id, l);
        }

        public Day02() : base(2)
        {
        }

        public override void Solve()
        {
            var sum = GetInputLines(false)
                .Select(Parse)
                .Where(game => game.Rounds.All(round => round[Color.Red] <= 12 && round[Color.Blue] <= 14 && round[Color.Green] <= 13))
                .Sum(game => game.Id);
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var sum = GetInputLines(false)
                .Select(Parse)
                .Select(game => (game.Rounds.Max(round => round[Color.Red]), game.Rounds.Max(round => round[Color.Blue]), game.Rounds.Max(round => round[Color.Green])))
                .Select(t => t.Item1 * t.Item2 * t.Item3)
                .Sum();
            Console.WriteLine(sum);
        }
    }
}
