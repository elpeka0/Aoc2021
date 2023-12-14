using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2023
{
    public class Day14 : DayBase
    {
        public Day14() : base(14)
        {
        }

        private void Tilt(Grid<char> grid, Vector direction)
        {
            var indexes = grid.Indexes();
            if (direction.X > 0 || direction.Y > 0)
            {
                indexes = indexes.Reverse();
            }

            foreach (var p in indexes.Where(i => grid[i] == 'O'))
            {
                Vector pos;
                for (pos = p + direction; grid.IsInBounds(pos); pos += direction)
                {
                    if (grid[pos] != '.')
                    {
                        grid[p] = '.';
                        grid[pos - direction] = 'O';
                        break;
                    }
                }

                if (!grid.IsInBounds(pos) && grid[pos - direction] == '.')
                {
                    grid[p] = '.';
                    grid[pos - direction] = 'O';
                }
            }
        }

        private void Cycle(Grid<char> grid)
        {
            Tilt(grid, new Vector(0, -1));
            Tilt(grid, new Vector(-1, 0));
            Tilt(grid, new Vector(0, 1));
            Tilt(grid, new Vector(1, 0));
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            Tilt(grid, new Vector(0, -1));

            var res = grid.Indexes().Where(p => grid[p] == 'O').Sum(i => grid.Height - i.Y);
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);

            var cnt = 0;
            var seen = new Dictionary<Grid<char>, int>();
            int cycleStart;
            int cycleEnd;
            
            while (true)
            {
                seen[grid] = cnt;
                Cycle(grid);

                ++cnt;
                if (seen.TryGetValue(grid, out var from))
                {
                    cycleStart = from;
                    cycleEnd = cnt;
                    break;
                }
            }

            var target = 1000000000;
            var len = cycleEnd - cycleStart;
            var finalMoves = (target - cycleStart) % len;
            for (var i = 0; i < finalMoves; ++i)
            {
                Cycle(grid);
            }


            var res = grid.Indexes().Where(p => grid[p] == 'O').Sum(i => grid.Height - i.Y);
            Console.WriteLine(res);
        }
    }
}
