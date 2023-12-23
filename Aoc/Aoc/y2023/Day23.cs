using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day23 : DayBase
    {
        public Day23() : base(23)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Row(0).Indexes().First(i => grid[i] == '.');
            var target = grid.Row(grid.Width - 1).Indexes().First(i => grid[i] == '.');

            var res = LongestPath(start, target, grid, CanMove);
            Console.WriteLine(res);

            IEnumerable<Vector> CanMove(Vector p)
            {
                return grid[p] switch
                {
                    '.' => grid.Neighbors(p, false),
                    '>' => new[] { p + new Vector(1, 0) },
                    '<' => new[] { p + new Vector(-1, 0) },
                    '^' => new[] { p + new Vector(0, -1) },
                    'v' => new[] { p + new Vector(0, 1) }
                };
            }
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Row(0).Indexes().First(i => grid[i] == '.');
            var target = grid.Row(grid.Width - 1).Indexes().First(i => grid[i] == '.');

            var res = LongestPath(start, target, grid, p => grid.Neighbors(p, false));
            Console.WriteLine(res);
        }

        private class State
        {
            public Vector s;
            public HashSet<Vector> visited;
            public IEnumerator<Vector> e;
            public long best = -1L;
            public Action<long> continuation;
        }

        private long LongestPath(Vector start, Vector target, Grid<char> grid, Func<Vector, IEnumerable<Vector>> stepFunc)
        {
            var stack = new Stack<Action>();
            long res = 0L;
            var initial = new State
            {
                s = start,
                visited = new HashSet<Vector>(),
                continuation = l => res = l
            };
            stack.Push(() => Head(initial));
            while (stack.Count > 0)
            {
                var next = stack.Pop();
                next();
            }
            return res;

            void Head(State state)
            {
                state.visited.Add(state.s);
                if (state.s == target)
                {
                    state.continuation(0L);
                    return;
                }

                state.e = stepFunc(state.s).Where(i => grid[i] != '#').GetEnumerator();

                stack.Push(() => Tail(state));
                stack.Push(() => Body(state));
            }

            void Body(State state)
            {
                if (state.e.MoveNext())
                {
                    stack.Push(() => Body(state));
                    if (!state.visited.Contains(state.e.Current))
                    {
                        var ns = new State
                        {
                            s = state.e.Current,
                            visited = state.visited.ToHashSet(),
                            continuation = l =>
                            {
                                if (l > state.best)
                                {
                                    state.best = l;
                                }
                            }
                        };
                        stack.Push(() => Head(ns));
                    }
                }
            }

            void Tail(State state)
            {
                state.continuation(state.best + 1);
            }
        }
    }
}
