using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2025;

public class Day02 : DayBase
{
    public Day02() : base(2)
    {
    }


    public override void Solve()
    {
        var ranges = this.GetInputLines().First().Split(',').Select(p => p.Split('-'))
            .Select(a => (From: long.Parse(a[0]), To: long.Parse(a[1]))).ToList();
        var sum = 0L;
        foreach (var r in ranges)
        {
            for (var l = r.From; l <= r.To; l++)
            {
                if (this.IsDoubled(l.ToString()))
                {
                    sum += l;
                }
            }
        }
        Console.WriteLine(sum);
    }

    private bool IsDoubled(string s)
    {
        return s.Length % 2 == 0 && s.Substring(0, s.Length / 2) == s.Substring(s.Length / 2);
    }

    private IEnumerable<string> Partion(string s, int n)
    {
        if (s.Length % n != 0)
        {
            yield break;
        }

        for (var i = 0; i < s.Length; i += n)
        {
            yield return s.Substring(i, n);
        }
    }

    private bool IsMultiString(string s)
    {
        return Enumerable.Range(1, s.Length - 1).Any(n => Partion(s, n).Distinct().Count() == 1);
    }

    public override void SolveMain()
    {
        var ranges = this.GetInputLines().First().Split(',').Select(p => p.Split('-'))
            .Select(a => (From: long.Parse(a[0]), To: long.Parse(a[1]))).ToList();
        var sum = 0L;
        foreach (var r in ranges)
        {
            for (var l = r.From; l <= r.To; l++)
            {
                
                if (this.IsMultiString(l.ToString()))
                {
                    sum += l;
                    //Console.WriteLine(l);
                }
            }
        }
        Console.WriteLine(sum);
    }
}
