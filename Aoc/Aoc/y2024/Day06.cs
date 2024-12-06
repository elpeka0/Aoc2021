using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2024
{
    public class Day06 : DayBase
    {
        public Day06() : base(6)
        {
        }

        private HashSet<Vector> FindPath(Grid<char> grid, Vector? extraObstacle)
        {
            var pos = grid.Indexes().First(i => grid[i] == '^');
            var facing = new Vector(0, -1);
            var seen = new HashSet<(Vector, Vector)>();
            var path = new HashSet<Vector>();

            while (true)
            {
                path.Add(pos);
                if (!seen.Add((pos, facing)))
                {
                    return null;
                }

                var next = pos + facing;
                if (!grid.IsInBounds(next))
                {
                    return path;
                }

                if (grid[next] == '#' || next == extraObstacle)
                {
                    facing = new Vector(-facing.Y, facing.X);
                    continue;
                }

                pos = next;
            }
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
            Console.WriteLine(this.FindPath(grid, null).Count);
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(this.GetInputLines().ToList(), c => c);
            var path = FindPath(grid, null);

            var res = path.Count(p => this.FindPath(grid, p) == null && grid[p] != '^');
            Console.WriteLine(res);
        }
    }
}
