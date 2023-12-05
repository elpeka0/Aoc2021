using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Aoc.Parsing;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day05 : DayBase
    {
        private interface IMap
        {
            long? this[long index] { get; }

            IEnumerable<Range> Transform(IEnumerable<Range> ranges);
        }

        private record RangeMap(long Source, long Destination, long Count) : IMap
        {
            public long? this[long index] => index >= Source && index < Source + this.Count
                ? index - Source + Destination
                : null;

            public Range SourceRange => new Range(Source, Source + Count);
            public Range DestinationRange => new Range(Destination, Destination + Count);

            public IEnumerable<Range> Transform(IEnumerable<Range> ranges)
            {
                foreach (var r in ranges)
                {
                    var diff = r.Minus(SourceRange).ToList();
                    var overlap = r.Intersect(SourceRange);

                    Debug.Assert(diff.Sum(d => d.Length) + (overlap?.Length ?? 0) == r.Length);

                    foreach (var s in diff)
                    {
                        yield return s;
                    }

                    if (overlap != null)
                    {
                        yield return new Range(this[overlap.From].Value, this[overlap.To] ?? (Destination + Count));
                    }
                }
            }
        }

        private record CombinedMap(IReadOnlyList<IMap> Maps) : IMap
        {
            public long? this[long index] => Maps.Select(m => m[index]).FirstOrDefault(j => j != null);

            public IEnumerable<Range> Transform(IEnumerable<Range> ranges)
            {
                foreach (var m in Maps)
                {
                    ranges = m.Transform(ranges);
                }

                return ranges;
            }
        }

        private record State(IReadOnlyList<long> Seeds, IReadOnlyList<IMap> Maps)
        {
            public long Lookup(long value)
            {
                return Maps.Aggregate(value, (a, m) => m[a] ?? a);
            }

            public IEnumerable<Range> Transform(IEnumerable<Range> ranges)
            {
                foreach (var m in Maps)
                {
                    ranges = m.Transform(ranges).ToList();
                }

                return ranges;
            }
        }

        private State Load()
        {
            var lines = GetInputLines(false).ToList();
            var seeds = "seeds:"
                .ThenWs(IntegerList())
                .Map(e => e.ToList())
                .Parse(new Input(lines[0]))
                .Value;

            var maps = new List<IMap>();
            IMap active = null;

            foreach (var line in lines.Skip(2))
            {
                if (line.Trim().Length == 0 && active != null)
                {
                    maps.Add(active);
                    active = null;
                }
                else if (IntegerList().Parse(new Input(line)) is ({Count: 3} ints, true))
                {
                    var range = new RangeMap(ints[1], ints[0], ints[2]);
                    active = active == null
                        ? range
                        : new CombinedMap(new[] { active, range });
                }
            }

            if (active != null)
            {
                maps.Add(active);
            }

            return new State(seeds, maps);
        }

        public Day05() : base(5)
        {
        }

        public override void Solve()
        {
            var state = this.Load();
            var res = state.Seeds.Min(s => state.Lookup(s));
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var state = this.Load();
            var ranges = new List<Range>();
            for (var i = 0; i < state.Seeds.Count; i += 2)
            {
                ranges.Add(new Range(state.Seeds[i], state.Seeds[i] + state.Seeds[i + 1]));
            }

            var transformed = state.Transform(ranges);
            var res = transformed.Select(r => r.From).Min();
            Console.WriteLine(res);
        }
    }
}
