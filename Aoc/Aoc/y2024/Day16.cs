using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day16 : DayBase
    {
        public Day16() : base(16)
        {
        }

        private class State
        {
            public Vector Pos { get; }

            public Vector Facing { get; }

            public State(Vector pos, Vector facing)
            {
                Pos = pos;
                Facing = facing;
            }

            public override int GetHashCode()
            {
                return Pos.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is State s && s.Pos == Pos;
            }
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines().ToList(), c => c);
            var pos = grid.Indexes().First(i => grid[i] == 'S');
            var target = grid.Indexes().First(i => grid[i] == 'E');

            Console.WriteLine(Path(grid, pos, target, new Vector(1, 0)));
        }

        private long Path(Grid<char> grid, Vector from, Vector to, Vector facing)
        {
            var cost = Utils.Dijkstra((from, facing), i => Step(grid, i));
            return cost.Where(kv => kv.Key.from == to).Min(kv => kv.Value);
        }

        private IEnumerable<((Vector, Vector), long)> Step(Grid<char> grid, (Vector Pos, Vector Facing) value)
        {
            return grid.Neighbors(value.Pos, false).Where(i => grid[i] != '#').Select(n => ((n, n - value.Pos), (n - value.Pos) == value.Facing
            ? 1L
            : 1001));
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines().ToList(), c => c);
            var pos = grid.Indexes().First(i => grid[i] == 'S');
            var target = grid.Indexes().First(i => grid[i] == 'E');
            var cost = Utils.Dijkstra((Pos: pos, Facing: new Vector(1, 0)), i => Step(grid, i));
            var best = cost.Where(kv => kv.Key.Pos == target).Min(kv => kv.Value);

            var good = new HashSet<Vector>();
            var n = 0;
            var l = cost.Where(x => x.Value <= best).ToList();
            foreach (var kv in l)
            {
                if (n % 100 == 0)
                {
                    Console.WriteLine($"Path: {n}/{l.Count}");
                }
                var rest = Path(grid, kv.Key.Pos, target, kv.Key.Facing);
                if (kv.Value + rest == best)
                {
                    good.Add(kv.Key.Pos);
                }
                n++;
            }

            Console.WriteLine(good.Count + 2);
        }
    }
}
