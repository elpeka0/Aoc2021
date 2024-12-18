using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day18 : DayBase
    {
        public Day18() : base(18)
        {
        }

        private List<Vector> GetPath(Grid<char> grid)
        {
            return Utils.Bfs(new Vector(0, 0), Step, v => v == new Vector(70, 70));

            IEnumerable<Vector> Step(Vector v)
            {
                foreach (var n in grid.Neighbors(v, false).Where(i => grid[i] != '#'))
                {
                    yield return n;
                }
            }
        }

        private List<Vector> GetInput()
        {
            return GetInputLines().Select(l => SplitInts(l, ',').ToList()).Select(p => new Vector(p[0], p[1])).ToList();
        }

        private void BlockTo(Grid<char> grid, List<Vector> input, int n)
        {
            foreach (var v in input.Take(n))
            {
                grid[v] = '#';
            }
        }


        public override void Solve()
        {
            var input = GetInput();
            var grid = Grid<char>.WithSize(71, 71);
            BlockTo(grid, input, 1024);
            Console.WriteLine(GetPath(grid).Count - 1);
        }

        public override void SolveMain()
        {
            var input = GetInput();
            var lookup = input.Select((v, i) => (v, i)).ToDictionary(t => t.v, t => t.i);
            var grid = Grid<char>.WithSize(71, 71);
            
            var index = 1023;
            while (true)
            {
                BlockTo(grid, input, index + 1);
                var path = GetPath(grid);
                if (path == null)
                {
                    break;
                }
                index = path.Min(p => lookup.TryGetValue(p, out var v)
                    ? v
                    : int.MaxValue);
            }
            var res = input[index];
            Console.WriteLine(res);
        }
    }
}
