using System;
using System.Collections.Generic;
using System.Linq;
using Aoc.Geometry;

namespace Aoc.y2025;

public class Day04 : DayBase
{
    public Day04() : base(4)
    {
    }

    private IEnumerable<Vector> GetAccessible(Grid<char> grid)
    {
        return grid.Indexes().Where(i => grid[i] == '@' && grid.Neighbors(i, true).Count(j => grid[j] == '@') < 4);
    }

    public override void Solve()
    {
        var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
        var res = GetAccessible(grid).Count();
        Console.WriteLine(res);
    }

    public override void SolveMain()
    {
        var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
        var n = 0;
        while (true)
        {
            var l = GetAccessible(grid).ToList();
            n += l.Count;
            if (l.Count == 0)
            {
                break;
            }

            foreach (var i in l)
            {
                grid[i] = '.';
            }
        }
        Console.WriteLine(n);
    }
}
