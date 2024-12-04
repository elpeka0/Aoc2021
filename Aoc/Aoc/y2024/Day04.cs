using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2024
{
    public class Day04 : DayBase
    {
        public Day04() : base(4)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);

            var cnt = 0;
            foreach (var index in grid.Indexes())
            {
                if (grid[index] == 'X')
                {
                    foreach (var n in grid.Neighbors(index, true))
                    {
                        if (grid[n] == 'M')
                        {
                            var dir = n - index;
                            var a = n + dir;
                            var s = n + dir + dir;

                            if (grid.IsInBounds(a) && grid[a] == 'A' && grid.IsInBounds(s) && grid[s] == 'S')
                            {
                                cnt++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine(cnt);
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);

            var cnt = 0;
            foreach (var index in grid.Indexes())
            {
                if (grid[index] == 'A')
                {
                    var local = 0;
                    foreach (var n in grid.Neighbors(index, true))
                    {
                        if (grid[n] == 'M' && (n - index).X != 0 && (n - index).Y != 0)
                        {
                            var opposite = index - (n - index);
                            if (grid.IsInBounds(opposite) && grid[opposite] == 'S')
                            {
                                local++;
                            }
                        }
                    }

                    if (local == 2)
                    {
                        cnt++;
                    }
                }
            }
            Console.WriteLine(cnt);
        }
    }
}
