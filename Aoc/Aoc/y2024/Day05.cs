using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day05 : DayBase
    {
        public Day05() : base(5)
        {
        }

        private (Dictionary<int, HashSet<int>> Rules, List<List<int>> Lists) GetInput()
        {
            var l = this.GetInputLines();
            var rules = l
                .TakeWhile(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => this.SplitInts(s, '|').ToArray())
                .Select(a => (Before: a[0], After: a[1]))
                .GroupBy(t => t.Before)
                .ToDictionary(g => g.Key, g => g.Select(t => t.After).ToHashSet());
            var lists = l
                .SkipWhile(s => !string.IsNullOrWhiteSpace(s))
                .Skip(1)
                .Select(s => this.SplitInts(s, ',').ToList())
                .ToList();
            return (rules, lists);
        }

        private bool IsCorrect(Dictionary<int, HashSet<int>> rules, List<int> n)
        {
            for (var i = 0; i < n.Count; i++)
            {
                for (var j = i + 1; j < n.Count; j++)
                {
                    if (rules.TryGetValue(n[j], out var r) && r.Contains(n[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void Solve()
        {
            var (rules, lists) = this.GetInput();
            var sum = lists.Where(n => IsCorrect(rules, n)).Sum(n => n[n.Count/2]);
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var (rules, lists) = this.GetInput();
            var wrong = lists.Where(n => !IsCorrect(rules, n)).ToList();

            var sum = 0;
            foreach (var n in wrong)
            {
                var newList = n.PartialSort((i, j) => rules[i].Contains(j)).ToList();
                sum += newList[newList.Count / 2];
            }

            Console.WriteLine(sum);
        }
    }
}
