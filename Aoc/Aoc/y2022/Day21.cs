using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day21 : DayBase
    {
        private interface INode
        {
            long? Calculate();
            long Ensure(long value);
        }

        private class Literal : INode
        {
            public long Value { get; set; }
            public long? Calculate() => Value;
            public long Ensure(long value)
            {
                return Value;
            }
        }

        private abstract class Binary : INode
        {
            public INode Left { get; set; }
            public INode Right { get; set; }

            protected readonly Lazy<long?> leftCache;
            protected readonly Lazy<long?> rightCache;

            public Binary()
            {
                leftCache = new Lazy<long?>(() => Left.Calculate());
                rightCache = new Lazy<long?>(() => Right.Calculate());
            }

            protected abstract long? Op(long? a, long? b);
            protected abstract long Inverse(long a, long b, bool lhs);

            public long? Calculate() => Op(leftCache.Value, rightCache.Value);
            public long Ensure(long value)
            {
                if (leftCache.Value == null)
                {
                    return Left.Ensure(Inverse(value, rightCache.Value.Value, true));
                }
                else
                {
                    return Right.Ensure(Inverse(value, leftCache.Value.Value, false));
                }
            }
        }

        private class Addition : Binary
        {
            protected override long? Op(long? a, long? b) => a + b;
            protected override long Inverse(long a, long b, bool lhs) => a - b;
        }

        private class Subtraction : Binary
        {
            protected override long? Op(long? a, long? b) => a - b;
            protected override long Inverse(long a, long b, bool lhs) => lhs ? a + b : b - a;
        }

        private class Division : Binary
        {
            protected override long? Op(long? a, long? b) => a / b;
            protected override long Inverse(long a, long b, bool lhs) => lhs ? a * b : b / a;
        }

        private class Multiplication : Binary
        {
            protected override long? Op(long? a, long? b) => a * b;
            protected override long Inverse(long a, long b, bool lhs) => a / b;
        }

        private class Human : INode
        {
            public long? Calculate()
            {
                return null;
            }

            public long Ensure(long value)
            {
                return value;
            }
        }

        private class Root : INode
        {
            public Binary Node { get; set; }

            public long? Calculate() => Node.Calculate();

            public long Ensure(long value)
            {
                if (Node.Left.Calculate() == null)
                {
                    return Node.Left.Ensure(Node.Right.Calculate().Value);
                }
                return Node.Right.Ensure(Node.Left.Calculate().Value);
            }
        }

        private class Ref : INode
        {
            private Dictionary<string, INode> lookup;
            private string key;

            public Ref(Dictionary<string, INode> lookup, string key)
            {
                this.lookup = lookup;
                this.key = key;
            }

            public long? Calculate() => lookup[key].Calculate();

            public long Ensure(long value) => lookup[key].Ensure(value);
        }

        private INode GetInput(bool part1)
        {
            var lookup = new Dictionary<string, INode>();

            INode Get(string name) => new Ref(lookup, name);

            foreach (var line in GetInputLines(false))
            {
                var parts = line.Split(':');
                var name = parts[0];
                parts = parts[1].Trim().Split(' ');
                INode n;
                if (parts.Length == 1)
                {
                    if (name == "humn" && !part1)
                    {
                        n = new Human();
                    }
                    else
                    {
                        n = new Literal { Value = long.Parse(parts[0]) };
                    }
                }
                else
                {
                    var bin = (Binary)(parts[1] switch
                    {
                        "+" => new Addition(),
                        "-" => new Subtraction(),
                        "*" => new Multiplication(),
                        "/" => new Division()
                    });
                    bin.Left = Get(parts[0]);
                    bin.Right = Get(parts[2]);
                    if (name == "root" && !part1)
                    {
                        n = new Root { Node = bin };
                    }
                    else
                    {
                        n = bin;
                    }
                }
                lookup[name] = n;
            }

            return lookup["root"];
        }

        public Day21() : base(21)
        {
        }

        public override void Solve()
        {
            Console.WriteLine(GetInput(true).Calculate());
        }

        public override void SolveMain()
        {
            Console.WriteLine(GetInput(false).Ensure(0));
        }
    }
}
