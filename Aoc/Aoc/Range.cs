using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public record Range(long From, long To)
    {
        private readonly bool _ = To < From ? throw new ArgumentException("Invalid range") : false;

        public bool Contains(long Value) => Value >= From && Value < To;

        public long Length => To - From;

        public bool Overlaps(Range other) => other.From < To && other.To > From;

        public Range Intersect(Range other)
        {
            if (!Overlaps(other))
            {
                return null;
            }

            return new Range(Math.Max(From, other.From), Math.Min(To, other.To));
        }

        public IEnumerable<Range> Minus(Range other)
        {
            if (From < other.From)
            {
                yield return this with { To = Math.Min(other.From, this.To) };
            }

            if (To > other.To)
            {
                yield return this with { From = Math.Max(other.To, this.From) };
            }
        }
    }
}
