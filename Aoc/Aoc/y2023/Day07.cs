using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Parsing;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day07 : DayBase
    {
        private int GetCard(char c, bool part1)
        {
            return c switch
            {
                'T' => 10,
                'J' when part1 => 11,
                'J' => 1,
                'Q' => 12,
                'K' => 13,
                'A' => 14,
                _ => c - '0'
            };
        }

        private enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeOfAKind = 3,
            FullHouse = 4,
            FourOfAKind = 5,
            FiveOfAKind = 6
        }

        private record Hand(HandType Type, IReadOnlyList<int> Full, long Bet) : IComparable<Hand>
        {
            public int CompareTo(Hand other)
            {
                if (Type != other.Type)
                {
                    return Type.CompareTo(other.Type);
                }

                for (var i = 0; i < Full.Count; ++i)
                {
                    if (Full[i] != other.Full[i])
                    {
                        return Full[i].CompareTo(other.Full[i]);
                    }
                }

                return 0;
            }

            public override string ToString()
            {
                return $"{Type}: {string.Join("", Full.Select(c => c.ToString("X")))}";
            }
        }

        private Hand MakeHand1(IReadOnlyList<int> cards, long bet)
        {
            var groups = cards.GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .ToList();
            return groups[0].Count() switch
            {
                5 => Make(HandType.FiveOfAKind),
                4 => Make(HandType.FourOfAKind),
                3 when groups[1].Count() == 2 => Make(HandType.FullHouse),
                3 => Make(HandType.ThreeOfAKind),
                2 when groups[1].Count() == 2 => Make(HandType.TwoPair),
                2 => Make(HandType.OnePair),
                _ => Make(HandType.HighCard)
            };

            Hand Make(HandType type)
            {
                return new Hand(
                    type,
                    cards,
                    bet);
            }
        }

        private Hand MakeHand2(IReadOnlyList<int> cards, long bet)
        {
            var groups = cards.GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .ToList();
            var jokers = groups.FirstOrDefault(g => g.Key == 1)?.Count() ?? 0;

            var gidx = 0;
            if (groups[0].Key == 1 && groups.Count > 1)
            {
                gidx = 1;
            }

            return groups[gidx].Count() switch
            {
                5 => Make(HandType.FiveOfAKind),
                4 when jokers == 1 => Make(HandType.FiveOfAKind),
                3 when jokers == 2 => Make(HandType.FiveOfAKind),
                2 when jokers == 3 => Make(HandType.FiveOfAKind),
                1 when jokers == 4 => Make(HandType.FiveOfAKind),
                4 => Make(HandType.FourOfAKind),
                3 when jokers == 1 => Make(HandType.FourOfAKind),
                2 when jokers == 2 => Make(HandType.FourOfAKind),
                1 when jokers == 3 => Make(HandType.FourOfAKind),
                3 when groups[1].Count() == 2 => Make(HandType.FullHouse),
                2 when groups[1].Count() == 2 && jokers == 1 => Make(HandType.FullHouse),
                3 => Make(HandType.ThreeOfAKind),
                2 when jokers == 1 => Make(HandType.ThreeOfAKind),
                1 when jokers == 2 => Make(HandType.ThreeOfAKind),
                2 when groups[1].Count() == 2 => Make(HandType.TwoPair),
                2 => Make(HandType.OnePair),
                1 when jokers == 1 => Make(HandType.OnePair),
                _ => Make(HandType.HighCard)
            };

            Hand Make(HandType type)
            {
                return new Hand(
                    type,
                    cards,
                    bet);
            }
        }

        Hand ParseHand(string line, bool part1)
        {
            return Char(_ => true)
                .Map(c => GetCard(c, part1))
                .Repeat(5, 5)
                .ThenWs(Integer())
                .Map(t => part1 ? MakeHand1(t.Item1.ToList(), t.Item2) : MakeHand2(t.Item1.ToList(), t.Item2))
                .Parse(new Input(line))
                .Value;
        }

        private IEnumerable<Hand> Read(bool part1) => GetInputLines(false).Select(l => this.ParseHand(l, part1));

        public Day07() : base(7)
        {
        }

        public override void Solve()
        {
            var hands = this.Read(true)
                .OrderBy(h => h)
                .ToList();

            var res = hands.Select((h, i) => h.Bet * (i + 1))
                .Sum();
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var hands = this.Read(false)
                .OrderBy(h => h)
                .ToList();

            var res = hands.Select((h, i) => h.Bet * (i + 1))
                .Sum();
            Console.WriteLine(res);
        }
    }
}
