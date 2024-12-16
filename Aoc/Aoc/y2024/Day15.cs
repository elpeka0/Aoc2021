using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day15 : DayBase
    {
        public Day15() : base(15)
        {
        }

        private class Input
        {
            public Grid<char> Grid { get; private set; }

            public IReadOnlyList<Vector> Instructions { get; }

            public Vector Pos { get; private set; }

            public Input(Grid<char> grid, IReadOnlyList<char> instructions)
            {
                Grid = grid;
                Instructions = instructions.Select(c => c switch
                {
                    '^' => new Vector(0, -1),
                    'v' => new Vector(0, 1),
                    '<' => new Vector(-1, 0),
                    '>' => new Vector(1, 0),
                    _ => throw new InvalidOperationException()
                }).ToImmutableList();
                Pos = Grid.Indexes().Where(x => Grid[x] == '@').First();
            }

            public void Move(Vector next)
            {
                var start = Pos;
                var end = start;
                while (Grid[end] == '@' || Grid[end] == 'O')
                {
                    end += next;
                }

                if (Grid[end] == '.')
                {
                    for (var p = end; p != start; p -= next)
                    {
                        Grid[p] = Grid[p - next];
                    }
                    Grid[start] = '.';

                    Pos = Pos + next;
                }
            }

            public void Widen()
            {
                var newGrid = Grid<char>.WithSize(Grid.Width * 2, Grid.Height);
                foreach (var index in Grid.Indexes())
                {
                    switch (Grid[index])
                    {
                        case '.':
                        case '#':
                            newGrid[2 * index.X, index.Y] = Grid[index];
                            newGrid[2 * index.X + 1, index.Y] = Grid[index];
                            break;
                        case '@':
                            newGrid[2 * index.X, index.Y] = '@';
                            newGrid[2 * index.X + 1, index.Y] = '.';
                            break;
                        case 'O':
                            newGrid[2 * index.X, index.Y] = '[';
                            newGrid[2 * index.X + 1, index.Y] = ']';
                            break;
                    }
                }
                Grid = newGrid;
                Pos = new(2*Pos.X, Pos.Y);
            }

            public void MoveWide(Vector next)
            {
                if (TryMoveWide(Pos, next, false))
                {
                    Pos += next;
                }
            }

            public bool TryMoveWide(Vector p, Vector next, bool tryOnly)
            {
                if (Grid[p] == '#')
                {
                    return false;
                }

                if (Grid[p] == '.')
                {
                    return true;
                }

                var follow = p + next;
                if (Grid[p] == '@')
                {
                    if (TryMoveWide(follow, next, tryOnly))
                    {
                        if (!tryOnly)
                        {
                            Grid[follow] = '@';
                            Grid[p] = '.';
                        }
                        return true;
                    }

                    return false;
                }

                if (next.Y == 0)
                {
                    if (TryMoveWide(follow, next, tryOnly))
                    {
                        if (!tryOnly)
                        {
                            Grid[follow] = Grid[p];
                        }
                        return true;
                    }

                    return false;
                }

                var orig = Grid[p];
                if (!TryMoveWide(follow, next, true))
                {
                    return false;
                }

                var other = orig switch
                {
                    '[' => p + new Vector(1, 0),
                    _ => p + new Vector(-1, 0)
                };

                if (!TryMoveWide(other + next, next, tryOnly))
                {
                    return false;
                }

                if (!tryOnly)
                {
                    TryMoveWide(follow, next, false);
                    Grid[follow] = Grid[p];
                    Grid[other + next] = Grid[other];
                    Grid[p] = '.';
                    Grid[other] = '.';
                }
                return true;
            }

            public void MoveWide(char c)
            {
                var next = c switch
                {
                    '^' => new Vector(0, -1),
                    'v' => new Vector(0, 1),
                    '<' => new Vector(-1, 0),
                    '>' => new Vector(1, 0),
                    _ => throw new InvalidOperationException()
                };

                var start = Pos;
                var end = start;
                while (Grid[end] == '@' || Grid[end] == '[' || Grid[end] == ']')
                {
                    end += next;
                }

                if (Grid[end] == '.')
                {
                    for (var p = end; p != start; p -= next)
                    {
                        Grid[p] = Grid[p - next];
                    }
                    Grid[start] = '.';

                    Pos = Pos + next;
                }
            }

            public int Sum() => Grid.Indexes().Where(i => Grid[i] == 'O' || Grid[i] == '[').Sum(i => 100 * i.Y + i.X);
        }

        private Input Read()
        {
            var lines = GetInputLines().ToList();
            var top = lines.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).ToList();
            var bottom = lines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1).ToList();

            var grid = Grid<char>.FromLines(top, c => c);
            var instructions = string.Join(string.Empty, bottom);

            return new(grid, instructions.ToList());
        }

        public override void Solve()
        {
            var i = Read();

            foreach (var c in i.Instructions)
            {
                i.Move(c);
            }

            Console.WriteLine(i.Sum());
        }

        public override void SolveMain()
        {
            var i = Read();
            i.Widen();

            foreach (var c in i.Instructions)
            {
                i.MoveWide(c);
                //Console.WriteLine(i.Grid);
                //Console.WriteLine();
            }

            Console.WriteLine(i.Sum());
        }
    }
}
