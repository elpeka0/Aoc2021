using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Aoc
{
    public class Day23 : DayBase
    {
        private class Spot
        {
            public int X { get; }
            public int Y { get; }
            public int? AmphiPod { get; }
            public bool IsMatch => Y > 0 && AmphiPod != null && GetScore(X) == AmphiPod.Value;

            public static int GetScore(int i)
            {
                var res = 1;
                while (i > 2)
                {
                    res *= 10;
                    i -= 2;
                }

                return res;
            }

            public static int GetIndex(int score)
            {
                var res = 2;
                while (score > 1)
                {
                    res += 2;
                    score /= 10;
                }

                return res;
            }

            public Spot(int x, int y, int? amphipod)
            {
                this.X = x;
                this.Y = y;
                this.AmphiPod = amphipod;
            }

            public override string ToString()
            {
                if (AmphiPod != null)
                {
                    return $"{X}, {Y}: {AmphiPod.Value}";
                }
                else
                {
                    return $"{X}, {Y}";
                }
            }
        }

        private class Board
        {
            public Spot[,] Targets { get; }
            public Spot[] Buffer { get; }
            public List<Spot> Misplaced { get; }

            public Board()
            {
                this.Targets = new Spot[4, 2];
                this.Buffer = new Spot[7];
                this.Misplaced = new List<Spot>();
            }

            private Board(Board other)
            {
                this.Buffer = other.Buffer.ToArray();
                this.Misplaced = other.Misplaced.ToList();
                this.Targets = new Spot[4, 2];
                for (var x = 0; x < 4; ++x)
                {
                    for (var y = 0; y < 2; ++y)
                    {
                        this.Targets[x, y] = other.Targets[x, y];
                    }
                }
            }

            public Spot GetSpot(int x, int y)
            {
                if (y > 0) return Targets[x / 2 - 1, y - 1];
                if (x == 10) x = 6;
                if (x == 2 || x == 4 || x == 6 || x == 8)
                {
                    return null;
                }
                if (x > 1) x = (x + 1) / 2;
                return Buffer[x];
            }

            public void SetSpot(int x, int y, Spot spot)
            {
                if (y > 0)
                {
                    Targets[x / 2 - 1, y - 1] = spot;
                    return;
                }
                if (x == 10) x = 6;
                if (x > 1) x = (x + 1) / 2;
                Buffer[x] = spot;
            }

            public int? MovementCost(int fx, int fy, int tx, int ty)
            {
                var steps = 0;
                bool Check(int x, int y)
                {
                    var s = GetSpot(x, y);
                    ++steps;
                    return s?.AmphiPod == null;
                }

                if (fy == 2)
                {
                    if (!Check(fx, 1)) return null;
                }

                if (fy > 0)
                {
                    if (!Check(fx, 0)) return null;
                }

                if (tx < fx)
                {
                    for (var x = fx - 1; x >= tx; --x)
                    {
                        if (!Check(x, 0)) return null;
                    }
                }
                else
                {
                    for (var x = fx + 1; x <= tx; ++x)
                    {
                        if (!Check(x, 0)) return null;
                    }
                }

                if (ty > 0)
                {
                    if (!Check(tx, 1)) return null;
                }

                if (ty == 2)
                {
                    if (!Check(tx, 2)) return null;
                }

                return steps;
            }

            public (int?, List<Move>) Solve(string indent)
            {
                for (var i = Misplaced.Count - 1; i >= 0; --i)
                {
                    var s = Misplaced[i];
                    if (s.IsMatch && (s.Y == 2 || GetSpot(s.X, s.Y + 1).IsMatch))
                    {
                        Misplaced.RemoveAt(i);
                    }
                }

                if (Misplaced.Count == 0)
                {
                    return (0, new List<Move>());
                }
                
                (int?, List<Move>) Move(Spot s, int x, int y, int idx)
                {
                    var cost = MovementCost(s.X, s.Y, x, y);
                    if (cost != null)
                    {
                        var clone = new Board(this);
                        clone.SetSpot(s.X, s.Y, new Spot(s.X, s.Y, null));
                        var next = new Spot(x, y, s.AmphiPod);
                        clone.SetSpot(x, y, next);
                        clone.Misplaced[idx] = next;
                        // Console.WriteLine($"{indent}Move from {s} to {next}");
                        var (c, l) = clone.Solve(indent + "    ");
                        l?.Insert(0, new Move(s, next));
                        return (cost * s.AmphiPod + c, l);
                    }

                    return (null, null);
                }

                for(var i = 0;i < Misplaced.Count;++i)
                {
                    var s = Misplaced[i];
                    var x = Spot.GetIndex(s.AmphiPod.Value);
                    var b = GetSpot(x, 2);
                    if (b.AmphiPod == null)
                    {
                        var (cost, l) = Move(s, x, 2, i);
                        if (cost != null)
                        {
                            // Console.WriteLine($"{indent}>> {cost}");
                            return (cost, l);
                        }
                    }
                    else if (b.IsMatch)
                    {
                        var (cost, l) = Move(s, x, 1, i);
                        if (cost != null)
                        {
                            // Console.WriteLine($"{indent}>> {cost}");
                            return (cost, l);
                        }
                    }
                }

                int? total = null;
                List<Move> retl = null;
                for (var i = 0; i < Misplaced.Count; ++i)
                {
                    var s = Misplaced[i];
                    if (s.Y > 0)
                    {
                        for (var x = 0; x < 11; ++x)
                        {
                            if (x % 2 != 0 || x == 0 || x == 10)
                            {
                                var (cost, l) = Move(s, x, 0, i);
                                if (cost != null && (total == null || cost < total))
                                {
                                    total = cost;
                                    retl = l;
                                }
                            }
                        }
                    }
                }

                // Console.WriteLine($"{indent}>> {total}");
                return (total, retl);
            }
        }

        private class Move
        {
            public Spot From { get; }
            public Spot To { get; }

            public Move(Spot @from, Spot to)
            {
                this.From = @from;
                this.To = to;
            }

            public override string ToString()
            {
                return $"{From}->{To}";
            }
        }

        //private int[] Seed => new[] { 10, 1, 100, 1000, 10, 100, 1000, 1 };
        //private int[] Seed => new[] { 1000, 1, 10, 10, 100, 100, 1000, 1 };
        private int[] Seed => new[] { 100, 10, 10, 100, 1, 1000, 1000, 1 };

        private Board GetInput()
        {
            var res = new Board();
            for (var x = 0; x < 4; ++x)
            {
                for (var y = 0; y < 2; ++y)
                {
                    var s = new Spot(2 + 2 * x, y + 1, Seed[2 * x + y]);
                    res.Targets[x, y] = s;
                    res.Misplaced.Add(s);
                }
            }

            for (var i = 0; i < res.Buffer.Length; ++i)
            {
                var offset = 0;
                if (i > 1) offset = i - 1;
                if (i == 6) offset = 4;
                res.Buffer[i] = new Spot(i + offset, 0, null);
            }
            return res;
        }

        public Day23() : base(23)
        {
        }

        public override void Solve()
        {
            var input = this.GetInput();
            var (res, l) = input.Solve(string.Empty);
            Console.WriteLine(res);
            foreach (var m in l)
            {
                Console.WriteLine(m);
            }
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
