using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day20 : DayBase
    {
        public Day20() : base(20)
        {
        }

        public override void Solve()
        {
            var indexed = IndexPath();

            var res = 0;
            foreach (var kv in indexed)
            {
                foreach (var a in kv.Key.Neighbors(false))
                {
                    foreach (var b in a.Neighbors(false))
                    {
                        if (indexed.TryGetValue(b, out var x) && x - kv.Value >= 102)
                        {
                            res++;
                        }
                    }
                }
            }
            Console.WriteLine(res);
        }

        private Dictionary<Vector, int> IndexPath()
        {
            var grid = Grid<char>.FromLines(GetInputLines().ToList(), c => c);
            var start = grid.Indexes().First(i => grid[i] == 'S');
            var end = grid.Indexes().First(i => grid[i] == 'E');
            grid[start] = grid[end] = '.';
            var path = Utils.Bfs(start, i => grid.Neighbors(i, false).Where(n => grid[n] == '.'), i => i == end);
            var indexed = path.Select((v, i) => (v, i)).ToDictionary(t => t.v, t => t.i);
            return indexed;
        }

        public override void SolveMain()
        {
            var indexed = IndexPath();

            var res = 0;
            var target = 100;
            var steps = 20;
            foreach (var kv in indexed)
            {
                foreach (var other in indexed)
                {
                    var m = Vector.Manhattan(other.Key, kv.Key);
                    if (m <= steps && other.Value - kv.Value >= (target + m))
                    {
                        res++;
                    }
                }
            }

            Console.WriteLine(res);
        }
    }
}
