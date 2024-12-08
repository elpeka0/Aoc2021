using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day08 : DayBase
    {
        public Day08() : base(8)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines().ToList(), c => c);
            var nodes = grid.Indexes().Where(i => grid[i] != '.').GroupBy(i => grid[i]).ToDictionary(g => g.Key, g => g.ToList());
            var anti = new HashSet<Vector>();

            foreach (var group in nodes)
            {
                foreach (var a in group.Value)
                {
                    foreach (var b in group.Value)
                    {
                        if (a != b)
                        {
                            var diff = a - b;

                            for (var i = 0; grid.IsInBounds(a + i * diff); i++)
                            {
                                anti.Add(a + i * diff);
                            }

                            for (var i = 0; grid.IsInBounds(b - i * diff); i++)
                            {
                                anti.Add(b - i * diff);
                            }
                        }
                    }
                }
            }

            Console.WriteLine(anti.Count);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
