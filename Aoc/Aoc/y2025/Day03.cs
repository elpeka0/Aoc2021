using System;
using System.Linq;

namespace Aoc.y2025;

public class Day03 : DayBase
{
    public Day03() : base(3)
    {
    }

    private int Evaluate(string s)
    {
        var first = 0;
        var second = 0;
        for (var i = 0; i < s.Length; i++)
        {
            var n = s[i] - '0';
            if (i + 1 < s.Length && n > first)
            {
                first = n;
                second = 0;
            }
            else if(n > second)
            {
                second = n;
            }
        }
        return 10*first + second;
    }

    public override void Solve()
    {
        var sum = this.GetInputLines().Select(this.Evaluate).Sum();
        Console.WriteLine(sum);
    }

    private long EvaluateMain(string s)
    {
        var relevant = new long[12];
        for (var i = 0; i < s.Length; i++)
        {
            var n = s[i] - '0';
            var clear = false;
            for (var j = 0; j < relevant.Length; j++)
            {
                if (clear)
                {
                    relevant[j] = 0;
                } 
                else if (i + (relevant.Length - j) <= s.Length && n > relevant[j])
                {
                    relevant[j] = n;
                    clear = true;
                }
            }
        }

        return relevant.Aggregate(0L, (a, b) => 10L * a + b);
    }

    public override void SolveMain()
    {
        var sum = this.GetInputLines().Select(this.EvaluateMain).Sum();
        Console.WriteLine(sum);
    }
}
