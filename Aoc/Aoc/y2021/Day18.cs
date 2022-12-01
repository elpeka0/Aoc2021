using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
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
                if (this.Parent == null)
                {
                    return null;
                }
                else if (this.Parent.Left != this)
                {
                    return this.Parent.Left is SnailNumPair p ? p.GetRightMostChild() : (SnailNumTerminus)this.Parent.Left;
                }
                else
                {
                    return this.Parent.GetLeftNeighbor();
                }
            }

            public SnailNumTerminus GetRightNeighbor()
            {
                if (this.Parent == null)
                {
                    return null;
                }
                else if (this.Parent.Right != this)
                {
                    return this.Parent.Right is SnailNumPair p ? p.GetLeftMostChild() : (SnailNumTerminus)this.Parent.Right;
                }
                else
                {
                    return this.Parent.GetRightNeighbor();
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
                    while (this.Explode(0))
                    {
                    }
                    s = this.Split();
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
                return this.Left is SnailNumPair p ? p.GetLeftMostChild() : (SnailNumTerminus)this.Left;
            }
            public SnailNumTerminus GetRightMostChild()
            {
                return this.Right is SnailNumPair p ? p.GetRightMostChild() : (SnailNumTerminus)this.Right;
            }

            public SnailNumPair(SnailNum left, SnailNum right)
            {
                this.Left = left;
                this.Right = right;
                left.Parent = this;
                right.Parent = this;
            }

            public override string ToString()
            {
                return $"[{this.Left}, {this.Right}]";
            }

            public override int Magnitude => 3*this.Left.Magnitude + 2*this.Right.Magnitude;

            public void Replace(SnailNum old, SnailNum @new)
            {
                if (this.Left == old) this.Left = @new;
                if (this.Right == old) this.Right = @new;
                @new.Parent = this;
            }

            public override bool Explode(int depth)
            {
                if (depth >= 4 && this.Left is SnailNumTerminus lt && this.Right is SnailNumTerminus rt)
                {
                    var ln = this.GetLeftNeighbor();
                    var rn = this.GetRightNeighbor();
                    if (ln != null)
                    {
                        ln.Value += lt.Value;
                    }
                    if (rn != null)
                    {
                        rn.Value += rt.Value;
                    }
                    this.Parent.Replace(this, new SnailNumTerminus(0));
                    return true;
                }
                else
                {
                    return this.Left.Explode(depth + 1) || this.Right.Explode(depth + 1);
                }
            }

            public override bool Split()
            {
                return this.Left.Split() || this.Right.Split();
            }

            public override SnailNum Clone()
            {
                return new SnailNumPair(this.Left.Clone(), this.Right.Clone());
            }
        }

        private class SnailNumTerminus : SnailNum
        {
            public int Value { get; set; }

            public SnailNumTerminus(int value)
            {
                this.Value = value;
            }

            public override string ToString()
            {
                return this.Value.ToString();
            }

            public override int Magnitude => this.Value;

            public override bool Split()
            {
                if (this.Value >= 10)
                {
                    var l = this.Value / 2;
                    var r = 2 * l == this.Value ? l : l + 1;
                    this.Parent.Replace(this, new SnailNumPair(new SnailNumTerminus(l), new SnailNumTerminus(r)));
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
                return new SnailNumTerminus(this.Value);
            }
        }

        private IEnumerable<SnailNum> GetInput() => this.GetInputLines(false).Select(SnailNum.Parse);

        public override void Solve()
        {
            var sum = this.GetInput().Aggregate((a, b) => a + b);
            Console.WriteLine(sum);
            Console.WriteLine(sum.Magnitude);
        }

        public override void SolveMain()
        {
            var l = this.GetInput().ToList();
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
