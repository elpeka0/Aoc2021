using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Aoc.y2025;

public class Day10 : DayBase
{
    public Day10() : base(10)
    {
    }

    private class MultiVector
    {
        public IReadOnlyList<int> Values { get; }

        public MultiVector(IReadOnlyList<int> values)
        {
            this.Values = values;
        }

        public MultiVector Increase(uint mask, int times)
        {
            var res = new List<int>(Values);
            for (var i = 0; i < Values.Count; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    res[i] += times;
                }
            }
            return new MultiVector(res);
        }

        public int MaskFits(MultiVector other, uint mask)
        {
            var min = int.MaxValue;
            for (var i = 0; i < Values.Count; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    var n = other.Values[i] - Values[i];
                    if (n < min)
                    {
                        min = n;
                    }
                }
            }
            return min;
        }
        
        public bool Exceeds(MultiVector other)
        {
            for (var i = 0; i < Values.Count; i++)
            {
                if (Values[i] > other.Values[i])
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() => Values.Aggregate(19, (a, b) => unchecked(31 * a + b));

        public override bool Equals(object obj)
        {
            if (obj is not MultiVector other)
            {
                return false;
            }

            if (Values.Count != other.Values.Count)
            {
                return false;
            }

            for (var i = 0; i < Values.Count; i++)
            {
                if (Values[i] != other.Values[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    private readonly record struct Layout(uint Target, IReadOnlyList<uint> Buttons, MultiVector Joltage);

    private IEnumerable<Layout> GetInput()
    {
        foreach (var line in this.GetInputLines())
        {
            var parts = line.Split(' ');
            var targetStr = parts[0].Substring(1, parts[0].Length - 2);
            uint target = 0;
            foreach (var c in targetStr.Reverse())
            {
                target <<= 1;
                target |= c switch
                {
                    '#' => 1u,
                    _ => 0u,
                };
            }

            var buttons = new List<uint>();
            foreach (var buttonStr in parts.Skip(1).Take(parts.Length - 2).Select(s => s.Substring(1, s.Length - 2)))
            {
                uint button = 0;
                foreach (var i in SplitInts(buttonStr, ','))
                {
                    button |= (uint)(1 << i);
                }
                buttons.Add(button);
            }
            
            var joltageStr = parts.Last().Substring(1,  parts.Last().Length - 2);
            var joltage = SplitInts(joltageStr, ',').ToList();
            yield return new Layout(target, buttons,  new MultiVector(joltage));
        }
    }

    public override void Solve()
    {
        var input = this.GetInput().ToList();

        var res = 0;
        foreach (var layout in input)
        {
            var path = Utils.Bfs(0u, u => layout.Buttons.Select(b => u ^ b), u => u == layout.Target);
            res += path.Count - 1;
        }
        Console.WriteLine(res);
    }

    private readonly record struct SolutionAttempt(int Count, MultiVector State);
    
    public override void SolveMain()
    {
        var input = this.GetInput().ToList();

        var res = 0;
        foreach (var layout in input)
        {
            Console.WriteLine(layout.Target);

            var remaining = new HashSet<uint>(layout.Buttons);
            var solutions = new List<SolutionAttempt>();
            solutions.Add(new SolutionAttempt(0, new MultiVector(layout.Joltage.Values.Select(_ => 0).ToList())));

            var indexes = Enumerable.Range(0, layout.Joltage.Values.Count)
                .Select(i => (i, layout.Buttons.Count(b => (b & (1 << i)) != 0)))
                .OrderBy(t => t.Item2)
                .Select(t => t.i)
                .ToList();
            
            foreach(var i in indexes)
            {
                var n = layout.Joltage.Values[i];
                var relevant = new List<uint>();
                foreach (var r in remaining.Where(x => (x & (1 << i)) != 0).ToList())
                {
                    remaining.Remove(r);
                    relevant.Add(r);
                }

                if (relevant.Count == 0)
                {
                    continue;
                }

                var newSolutions = new List<SolutionAttempt>();
                foreach (var a in solutions)
                {
                    newSolutions.AddRange(GetAttempts(0, n - a.State.Values[i], a.State).Select(x => x with { Count = x.Count + a.Count }));
                }

                if (newSolutions.Count == 0)
                {
                    throw new InvalidOperationException();
                }

                solutions = newSolutions.GroupBy(s => s.State).Select(g => g.OrderBy(x => x.Count).First()).ToList();

                IEnumerable<SolutionAttempt> GetAttempts(int pos, int rem, MultiVector state)
                {
                    if (pos + 1 >= relevant.Count)
                    {
                        var n = state.Increase(relevant[pos], rem);
                        if (!n.Exceeds(layout.Joltage))
                        {
                            yield return new SolutionAttempt(rem, n);
                        }
                        yield break;
                    }

                    for (var j = 0; j <= rem; j++)
                    {
                        var mid = state.Increase(relevant[pos], j);
                        if (mid.Exceeds(layout.Joltage))
                        {
                            break;
                        }

                        foreach (var a in GetAttempts(pos + 1, rem - j, mid))
                        {
                            yield return a with { Count = rem };
                        }
                    }
                }
            }
            res +=  solutions.Where(a => a.State.Equals(layout.Joltage)).Select(s => s.Count).Min();
        }
        Console.WriteLine(res);
    }
}
