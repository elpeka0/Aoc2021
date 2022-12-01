using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day16 : DayBase
    {
        public Day16() : base(16)
        {
        }

        private IEnumerable<bool> GetInput()
        {
            var line = this.GetInputLines(false).First();
            foreach (var c in line)
            {
                int n;
                if (char.IsDigit(c))
                {
                    n = c - '0';
                }
                else
                {
                    n = c - 'A' + 10;
                }
                yield return (n & 8) != 0;
                yield return (n & 4) != 0;
                yield return (n & 2) != 0;
                yield return (n & 1) != 0;
            }
        }

        private abstract class Packet
        {
            public int Version { get; }
            public int Type { get; }

            protected Packet(int version, int type)
            {
                this.Version = version;
                this.Type = type;
            }

            public abstract IEnumerable<Packet> Recurse();
            public abstract long Eval();
        }

        private class LiteralPacket : Packet
        {
            public LiteralPacket(int version, int type) : base(version, type)
            {
            }

            public long Value { get; set; }

            public override IEnumerable<Packet> Recurse()
            {
                yield return this;
            }

            public override long Eval()
            {
                return this.Value;
            }
        }

        private class OperatorPacket : Packet
        {
            public OperatorPacket(int version, int type) : base(version, type)
            {
            }

            public List<Packet> Children { get; } = new List<Packet>();

            public override IEnumerable<Packet> Recurse()
            {
                yield return this;
                foreach (var c in this.Children.SelectMany(x => x.Recurse()))
                {
                    yield return c;
                }
            }

            public override long Eval()
            {
                switch (this.Type)
                {
                    case 0:
                        return this.Children.Sum(s => s.Eval());
                    case 1:
                        return this.Children.Aggregate(1L, (a, b) => a * b.Eval());
                    case 2:
                        return this.Children.Min(c => c.Eval());
                    case 3:
                        return this.Children.Max(c => c.Eval());
                    case 5:
                        return this.Children[0].Eval() > this.Children[1].Eval() ? 1 : 0;
                    case 6:
                        return this.Children[0].Eval() < this.Children[1].Eval() ? 1 : 0;
                    case 7:
                        return this.Children[0].Eval() == this.Children[1].Eval() ? 1 : 0;
                    default:
                        return 0;
                }
            }
        }

        private int ReadInt(IEnumerator<(bool Bit, int Index)> iter, int n)
        {
            var res = 0;
            for (var i = 0; i < n && iter.MoveNext(); ++i)
            {
                res = res << 1 | (iter.Current.Bit ? 1 : 0);
            }

            return res;
        }

        private Packet ReadPacket(IEnumerator<(bool Bit, int Index)> iter, int limit)
        {
            var version = this.ReadInt(iter, 3);
            var type = this.ReadInt(iter, 3);
            if (type == 4)
            {
                var literal = new LiteralPacket(version, type);
                var more = true;
                while (more)
                {
                    more = false;
                    if (iter.MoveNext() && iter.Current.Index < limit - 1)
                    {
                        more = iter.Current.Bit;
                        literal.Value = (literal.Value << 4) | (long) this.ReadInt(iter, 4);
                    }
                }

                return literal;
            }
            else
            {
                var op = new OperatorPacket(version, type);
                var lenBit = iter.MoveNext() && iter.Current.Bit;
                if (lenBit)
                {
                    var len = this.ReadInt(iter, 11);
                    for (var i = 0; i < len; ++i)
                    {
                        op.Children.Add(this.ReadPacket(iter, limit));
                    }
                }
                else
                {
                    var len = this.ReadInt(iter, 15);
                    limit = Math.Min(limit, iter.Current.Index + len);
                    while (iter.Current.Index < limit - 1)
                    {
                        op.Children.Add(this.ReadPacket(iter, limit));
                    }
                }

                return op;
            }
        }

        public override void Solve()
        {
            var roots = new List<Packet>();
            var input = this.GetInput().ToList();
            using var iter = input.Select((b, i) => (Bit: b, Index: i)).GetEnumerator();
            while (iter.Current.Index < input.Count - 1)
            {
                roots.Add(this.ReadPacket(iter, input.Count));
            }

            var vsum = roots.SelectMany(x => x.Recurse()).Select(x => x.Version).Sum();
            Console.WriteLine(vsum);
        }

        public override void SolveMain()
        {
            var input = this.GetInput().ToList();
            using var iter = input.Select((b, i) => (Bit: b, Index: i)).GetEnumerator();
            var root = this.ReadPacket(iter, input.Count);
            
            Console.WriteLine(root.Eval());
        }
    }
}
