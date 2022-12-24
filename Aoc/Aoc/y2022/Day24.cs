using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day24 : DayBase
    {
        public Day24() : base(24)
        {
        }

        private record Blizzard(int X, int Y, int DX, int DY)
        {
            public (int X, int Y) PositionAt(int width, int height, int time)
            {
                int FancyMod(int n, int t, int d, int max)
                {
                    var res = (n - 1 + t * d) % (max - 2);
                    if (res < 0)
                    {
                        res += max - 2;
                    }
                    return res + 1;
                }

                return (FancyMod(X, time, DX, width), FancyMod(Y, time, DY, height));
            }
        }

        private record Board(int Width, int Height, List<Blizzard> Blizzards)
        {
            public bool HasBlizzard(int x, int y, int time)
            {
                return Blizzards.Any(b => b.PositionAt(Width, Height, time) == (x, y));
            }
        }

        private Board GetInput()
        {
            var g = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var b = new List<Blizzard>();

            foreach (var idx in g.Indexes())
            {
                switch (g[idx.X, idx.Y])
                {
                    case '^':
                        b.Add(new Blizzard(idx.X, idx.Y, 0, -1));
                        break;
                    case 'v':
                        b.Add(new Blizzard(idx.X, idx.Y, 0, 1));
                        break;
                    case '>':
                        b.Add(new Blizzard(idx.X, idx.Y, 1, 0));
                        break;
                    case '<':
                        b.Add(new Blizzard(idx.X, idx.Y, -1, 0));
                        break;
                }
            }

            return new Board(g.Width, g.Height, b);
        }

        private void RenderPath(List<(int X, int Y, int T)> path)
        {
            for (int i = 1; i < path.Count; ++i)
            {
                Console.WriteLine((path[i].X - path[i - 1].X, path[i].Y - path[i - 1].Y) switch
                {
                    (0, 0) => "Wait",
                    (0, 1) => "Down",
                    (0, -1) => "Up",
                    (1, 0) => "Right",
                    (-1, 0) => "Left",
                    _ => "?"
                });
            }
        }

        private record Pos(int X, int Y, int T);

        private Pos Traverse(Board board, Pos pos, int targetX, int targetY)
        {
            var byX = board.Blizzards.GroupBy(b => b.X).ToDictionary(g => g.Key, g => g.Where(b => b.DX == 0).ToList());
            var byY = board.Blizzards.GroupBy(b => b.Y).ToDictionary(g => g.Key, g => g.Where(b => b.DY == 0).ToList());

            var path = Utils.Bfs(pos,
                p =>
                {
                    var l = new List<Pos>();
                    foreach (var (ox, oy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1), (0, 0) })
                    {
                        var tx = p.X + ox;
                        var ty = p.Y + oy;
                        var tt = p.T + 1;

                        if (((tx == 1 && ty == 0) ||
                           (tx == board.Width - 2 && ty == board.Height - 1) ||
                           (tx > 0 && tx <= board.Width - 2 && ty > 0 && ty <= board.Height - 2))
                           && (!byX.ContainsKey(tx) || !byX[tx].Any(b => b.PositionAt(board.Width, board.Height, tt) == (tx, ty)))
                           && (!byY.ContainsKey(ty) || !byY[ty].Any(b => b.PositionAt(board.Width, board.Height, tt) == (tx, ty))))
                        {
                            l.Add(new Pos(tx, ty, tt));
                        }
                    }
                    return l;
                },
                p => p.X == targetX && p.Y == targetY);
            return path.Last();
        }

        public override void Solve()
        {
            var board = GetInput();
            Console.WriteLine(Traverse(board, new Pos(1, 0, 0), board.Width - 2, board.Height - 1).T);
        }

        public override void SolveMain()
        {
            var board = GetInput();
            var pos = new Pos(1, 0, 0);
            pos = Traverse(board, pos, board.Width - 2, board.Height - 1);
            pos = Traverse(board, pos, 1, 0);
            pos = Traverse(board, pos, board.Width - 2, board.Height - 1);
            Console.WriteLine(pos.T);
        }
    }
}
