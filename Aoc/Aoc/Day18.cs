using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day18 : DayBase
    {
        public Day18() : base(18)
        {
        }

        private abstract class SnailNum
        {
            public SnailNumPair Parent { get; set; }

            public SnailNumTerminus GetLeftNeighbor()
            {
                if (Parent == null)
                {
                    return null;
                }
                else if (Parent.Left != this)
                {
                    return Parent.Left is SnailNumPair p ? p.GetRightMostChild() : (SnailNumTerminus)Parent.Left;
                }
                else
                {
                    return Parent.GetLeftNeighbor();
                }
            }

            public SnailNumTerminus GetRightNeighbor()
            {
                if (Parent == null)
                {
                    return null;
                }
                else if (Parent.Right != this)
                {
                    return Parent.Right is SnailNumPair p ? p.GetLeftMostChild() : (SnailNumTerminus)Parent.Right;
                }
                else
                {
                    return Parent.GetRightNeighbor();
                }
            }

            public static SnailNum operator +(SnailNum a, SnailNum b)
            {
                var res = new SnailNumPair(a.Clone(), b.Clone());
                res.Reduce();
                return res;
            }

            public abstract int Magnitude
            {
                get;
            }

            public abstract bool Split();

            public abstract bool Explode(int depth);

            public void Reduce()
            {
                bool s;
                do
                {
                    while (Explode(0))
                    {
                    }
                    s = Split();
                } while (s);
            }

            public static SnailNum Parse(string line)
            {
                int pos = 0;
                return Parse(line, ref pos);
            }

            private static SnailNum Parse(string line, ref int pos)
            {
                if (line[pos] == '[')
                {
                    return ParsePair(line, ref pos);
                }
                else
                {
                    return ParseNum(line, ref pos);
                }
            }

            private static SnailNumPair ParsePair(string line, ref int pos)
            {
                ++pos; //[
                var left = Parse(line, ref pos);
                ++pos; //,
                var right = Parse(line, ref pos);
                ++pos; //]
                return new SnailNumPair(left, right);
            }

            private static SnailNumTerminus ParseNum(string line, ref int pos)
            {
                var num = line[pos] - '0';
                ++pos;
                return new SnailNumTerminus(num);
            }

            public abstract SnailNum Clone();
        }

        private class SnailNumPair : SnailNum
        {
            public SnailNum Left { get; set; }
            public SnailNum Right { get; set; }

            public SnailNumTerminus GetLeftMostChild()
            {
                return Left is SnailNumPair p ? p.GetLeftMostChild() : (SnailNumTerminus)Left;
            }
            public SnailNumTerminus GetRightMostChild()
            {
                return Right is SnailNumPair p ? p.GetRightMostChild() : (SnailNumTerminus)Right;
            }

            public SnailNumPair(SnailNum left, SnailNum right)
            {
                Left = left;
                Right = right;
                left.Parent = this;
                right.Parent = this;
            }

            public override string ToString()
            {
                return $"[{Left}, {Right}]";
            }

            public override int Magnitude => 3*Left.Magnitude + 2*Right.Magnitude;

            public void Replace(SnailNum old, SnailNum @new)
            {
                if (Left == old) Left = @new;
                if (Right == old) Right = @new;
                @new.Parent = this;
            }

            public override bool Explode(int depth)
            {
                if (depth >= 4 && Left is SnailNumTerminus lt && Right is SnailNumTerminus rt)
                {
                    var ln = GetLeftNeighbor();
                    var rn = GetRightNeighbor();
                    if (ln != null)
                    {
                        ln.Value += lt.Value;
                    }
                    if (rn != null)
                    {
                        rn.Value += rt.Value;
                    }
                    Parent.Replace(this, new SnailNumTerminus(0));
                    return true;
                }
                else
                {
                    return Left.Explode(depth + 1) || Right.Explode(depth + 1);
                }
            }

            public override bool Split()
            {
                return Left.Split() || Right.Split();
            }

            public override SnailNum Clone()
            {
                return new SnailNumPair(Left.Clone(), Right.Clone());
            }
        }

        private class SnailNumTerminus : SnailNum
        {
            public int Value { get; set; }

            public SnailNumTerminus(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }

            public override int Magnitude => Value;

            public override bool Split()
            {
                if (Value >= 10)
                {
                    var l = Value / 2;
                    var r = 2 * l == Value ? l : l + 1;
                    Parent.Replace(this, new SnailNumPair(new SnailNumTerminus(l), new SnailNumTerminus(r)));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override bool Explode(int depth)
            {
                return false;
            }

            public override SnailNum Clone()
            {
                return new SnailNumTerminus(Value);
            }
        }

        private IEnumerable<SnailNum> GetInput() => GetInputLines(false).Select(SnailNum.Parse);

        public override void Solve()
        {
            var sum = GetInput().Aggregate((a, b) => a + b);
            Console.WriteLine(sum);
            Console.WriteLine(sum.Magnitude);
        }

        public override void SolveMain()
        {
            var l = GetInput().ToList();
            var m = 0;
            foreach (var a in l)
            {
                foreach (var b in l)
                {
                    if (a != b)
                    {
                        var x = (a + b).Magnitude;
                        if (x > m)
                        {
                            m = x;
                        }
                    }
                }
            }
            Console.WriteLine(m);
        }
    }
}
