using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day03 : DayBase
    {
        private enum ElementType
        {
            Nothing,
            Symbol,
            Number
        }

        private class DistinctInt
        {
            public int Value { get; set; }
        }

        private class Element
        {
            public ElementType Type { get; }

            public DistinctInt Number { get; }

            public Element(ElementType type, DistinctInt number)
            {
                Type = type;
                Number = number;
            }


        }

        public Day03() : base(3)
        {
        }

        public override void Solve()
        {
            DistinctInt active = null;
            var grid = Grid<Element>.FromLines(GetInputLines(false).ToList(), c =>
            {
                if (c == '.')
                {
                    active = null;
                    return new Element(ElementType.Nothing, null);
                }
                else if (char.IsDigit(c))
                {
                    active ??= new DistinctInt();
                    active.Value = 10 * active.Value + (c - '0');
                    return new Element(ElementType.Number, active);
                }
                else
                {
                    active = null;
                    return new Element(ElementType.Symbol, null);
                }
            });

            var sum = grid
                .Indexes()
                .Where(i => grid[i].Type == ElementType.Number && grid.Neighbors(i, true).Any(j => grid[j].Type == ElementType.Symbol))
                .Select(i => grid[i].Number)
                .Distinct()
                .Sum(n => n.Value);
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            DistinctInt active = null;
            var grid = Grid<Element>.FromLines(GetInputLines(false).ToList(), c =>
            {
                if (c == '*')
                {
                    active = null;
                    return new Element(ElementType.Symbol, null);
                }
                else if (char.IsDigit(c))
                {
                    active ??= new DistinctInt();
                    active.Value = 10 * active.Value + (c - '0');
                    return new Element(ElementType.Number, active);
                }
                else
                {
                    active = null;
                    return new Element(ElementType.Nothing, null);
                }
            });

            var sum = grid
                .Indexes()
                .Where(i => grid[i].Type == ElementType.Symbol)
                .Select(i => grid
                    .Neighbors(i, true)
                    .Where(j => grid[j].Type == ElementType.Number)
                    .Select(j => grid[j].Number)
                    .Distinct()
                    .ToList())
                .Where(l => l.Count == 2)
                .Select(l => l[0].Value * l[1].Value)
                .Sum();
            Console.WriteLine(sum);
        }
    }
}
