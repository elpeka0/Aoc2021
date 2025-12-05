using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2025;

public class Day05 : DayBase
{
    public Day05() : base(5)
    {
    }

    private record Range(long Min, long Max)
    {
        public bool Includes(long value)
        {
            return value >= Min && value <= Max;
        }

        public Range Union(Range range)
        {
            return new Range(Math.Min(Min, range.Min), Math.Max(Max, range.Max));
        }

        public bool Intersects(Range range)
        {
            return Includes(range.Min) || Includes(range.Max) || range.Includes(Min) || range.Includes(Max);
        }
        
        public long Length => Max - Min + 1;
    }
    
    private record Input(IReadOnlyList<Range> Ranges, IReadOnlyList<long> Values);

    private Input Normalize(Input input)
    {
        var l = new List<Range>();
        var seen = new HashSet<Range>();
        
        for (var i = 0; i < input.Ranges.Count; i++)
        {
            var current = input.Ranges[i];
            if (!seen.Add(current))
            {
                continue;
            }

            var effective = true;
            while (effective)
            {
                effective = false;
                for (var j = i + 1; j < input.Ranges.Count; j++)
                {
                    var check = input.Ranges[j];
                    if (current.Intersects(check) && seen.Add(check))
                    {
                        current = current.Union(check);
                        effective = true;
                    }
                }
            }
            l.Add(current);
        }

        return input with { Ranges = l };
    }

    private Input GetInput()
    {
        var lines = this.GetInputLines().ToList();
        var ranges = lines.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Split('-'))
            .Select(a => new Range(long.Parse(a[0]), long.Parse(a[1]))).ToList();
        var values = lines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1).Select(long.Parse).ToList();
        return new Input(ranges, values);
    }

    public override void Solve()
    {
        var input = this.GetInput();
        var cnt = input.Values.Count(v => input.Ranges.Any(r => r.Includes(v)));
        Console.WriteLine(cnt);
    }

    public override void SolveMain()
    {
        var input = this.Normalize(this.GetInput());
        Console.WriteLine(input.Ranges.Sum(r => r.Length));
    }
}
