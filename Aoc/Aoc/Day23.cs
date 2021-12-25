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
            public bool IsLocked { get; }

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

            public Spot(int x, int y, int? amphipod, bool isLocked)
            {
                this.X = x;
                this.Y = y;
                this.AmphiPod = amphipod;
                this.IsLocked = isLocked;
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
            public Dictionary<(int, int), Spot> Spots { get; }
            public int Depth { get; }

            public Board(int depth)
            {
                this.Spots = new Dictionary<(int, int), Spot>();
                this.Depth = depth;
            }

            private Board(Board other)
            {
                this.Spots = other.Spots.ToDictionary(kv => kv.Key, kv => kv.Value);
                this.Depth = other.Depth;
            }

            public int? MovementCost(int fx, int fy, int tx, int ty)
            {
                var steps = 0;
                bool Check(int x, int y)
                {
                    Spots.TryGetValue((x, y), out var s);
                    ++steps;
                    return s?.AmphiPod == null;
                }

                for (var cy = fy - 1; cy >= 0; --cy)
                {
                    if (!Check(fx, cy)) return null;
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

                for (var cy = 1; cy <= ty; ++cy)
                {
                    if (!Check(tx, cy)) return null;
                }

                return steps;
            }

            private IEnumerable<Spot> Below(int x, int y)
            {
                for (var cy = y + 1; cy <= Depth; ++cy)
                {
                    Spots.TryGetValue((x, cy), out var spot);
                    yield return spot;
                }
            }

            public (int?, List<Move>) Solve(string indent)
            {
                var misplaced = new List<Spot>();
                foreach(var s in Spots.Values)
                {
                    if (!s.IsLocked)
                    {
                        if (s.IsMatch && Below(s.X, s.Y).All(s => s != null && s.IsMatch))
                        {
                            Spots[(s.X, s.Y)] = new Spot(s.X, s.Y, s.AmphiPod, true);
                        }
                        else if(s.Y < 2 || !Spots.ContainsKey((s.X, s.Y - 1)))
                        {
                            misplaced.Add(s);
                        }
                    }
                }

                if (misplaced.Count == 0)
                {
                    return (0, new List<Move>());
                }
                
                (int?, List<Move>) Move(Spot s, int x, int y)
                {
                    var cost = MovementCost(s.X, s.Y, x, y);
                    if (cost != null)
                    {
                        var clone = new Board(this);
                        clone.Spots.Remove((s.X, s.Y));
                        var next = new Spot(x, y, s.AmphiPod, false);
                        clone.Spots[(x, y)] = next;
                        // Console.WriteLine($"{indent}Move from {s} to {next}");
                        var (c, l) = clone.Solve(indent + "    ");
                        l?.Insert(0, new Move(s, next));
                        return (cost * s.AmphiPod + c, l);
                    }

                    return (null, null);
                }

                foreach (var s in misplaced)
                {
                    var x = Spot.GetIndex(s.AmphiPod.Value);
                    for (var cy = Depth; cy >= 1; --cy)
                    {
                        if (!Spots.TryGetValue((x, cy), out var target))
                        {
                            var (cost, l) = Move(s, x, cy);
                            if (cost != null)
                            {
                                // Console.WriteLine($"{indent}>> {cost}");
                                return (cost, l);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (!target.IsMatch)
                        {
                            break;
                        }
                    }
                }

                int? total = null;
                List<Move> retl = null;
                foreach (var s in misplaced)
                {
                    if (s.Y > 0)
                    {
                        for (var x = 0; x < 11; ++x)
                        {
                            if (x % 2 != 0 || x == 0 || x == 10)
                            {
                                var (cost, l) = Move(s, x, 0);
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

        private Board GetInput(int depth)
        {
            var res = new Board(depth);
            for (var x = 0; x < 4; ++x)
            {
                for (var y = 0; y < 2; ++y)
                {
                    var ax = 2 + 2 * x;
                    var ay = y == 0 ? 1 : depth;
                    var s = new Spot(ax, ay, Seed[2 * x + y], false);
                    res.Spots[(ax, ay)] = s;
                }
            }

            if(depth == 4)
            {
                void Put(int x, int y, int value)
                {
                    var s = new Spot(x, y, value, false);
                    res.Spots[(x, y)] = s;
                }
                Put(2, 2, 1000);
                Put(2, 3, 1000);
                Put(4, 2, 100);
                Put(4, 3, 10);
                Put(6, 2, 10);
                Put(6, 3, 1);
                Put(8, 2, 1);
                Put(8, 3, 100);
            }

            return res;
        }

        public Day23() : base(23)
        {
        }

        public override void Solve()
        {
            var sw = new Stopwatch();
            sw.Start();
            var input = this.GetInput(2);
            var (res, l) = input.Solve(string.Empty);
            Console.WriteLine(res);
            foreach (var m in l)
            {
                Console.WriteLine(m);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public override void SolveMain()
        {
            var input = this.GetInput(4);
            var (res, l) = input.Solve(string.Empty);
            Console.WriteLine(res);
            foreach (var m in l)
            {
                Console.WriteLine(m);
            }
        }
    }
}
