using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2025
{
    public class Day06 : DayBase
    {
        public Day06() : base(6)
        {
        }

        public override void Solve()
        {
            var original = GetInputLines().Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToList();
            var pivot = original.Last().Select(l => (Values: new List<long>(), Operation: l[0])).ToList();

            foreach (var line in original.Take(original.Count - 1))
            {
                for (var i = 0; i < line.Length; i++)
                {
                    pivot[i].Values.Add(long.Parse(line[i]));
                }
            }

            var res = pivot.Sum(p => p.Operation switch
            {
                '+' => p.Values.Sum(),
                _ => p.Values.Aggregate(1L, (a, b) => a * b)
            });
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines().ToList(), c => c);

            var values = new List<long>();
            var op = '+';
            var res = 0L;
            foreach (var column in grid.Columns())
            {
                if (column.Last() != ' ')
                {
                    op = column.Last();
                }
                var value = column.Select(c => char.IsDigit(c) ? c - '0' : 0).Aggregate(0L, (a, b) => b == 0 ? a : 10 * a + b);
                if (value == 0)
                {
                    Consume();
                }
                else
                {
                    values.Add(value);
                }
            }
            Consume();
            Console.WriteLine(res);

            void Consume()
            {
                res += op switch
                {
                    '+' => values.Sum(),
                    _ => values.Aggregate(1L, (a, b) => a * b)
                };
                values.Clear();
            }
        }
    }
}
