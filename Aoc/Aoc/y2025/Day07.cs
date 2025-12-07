using System;
using System.Collections.Generic;
using System.Linq;
using Aoc.Geometry;

namespace Aoc.y2025;

public class Day07 : DayBase
{
    public Day07() : base(7)
    {
    }

    private (Dictionary<Vector, long>, int) Simulate()
    {
        var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
        var pos = grid.Indexes().First(i => grid[i] == 'S');
        var positions = new Dictionary<Vector, long>();
        positions[pos] = 1;
        var cnt = 0;

        for (var pointer = new Vector(0, 1); grid.IsInBounds(pos + pointer); pointer += new Vector(0, 1))
        {
            foreach (var kv in positions.ToList())
            {
                if (grid[kv.Key + pointer] == '^')
                {
                    positions.Remove(kv.Key);
                    ++cnt;
                    Increase(kv.Key + new Vector(1, 0), kv.Value);
                    Increase(kv.Key + new Vector(-1, 0), kv.Value);
                }
            }
        }
        
        return (positions, cnt);

        void Increase(Vector v, long value)
        {
            if (grid.IsInBounds(v))
            {
                positions.TryGetValue(v, out var l);
                positions[v] = l + value;
            }
        }
    }

    public override void Solve()
    {
        var (_, cnt) = Simulate();
        Console.WriteLine(cnt);
    }

    public override void SolveMain()
    {
        var (res, _) = Simulate();
        Console.WriteLine(res.Sum(k => k.Value));
    }
}
