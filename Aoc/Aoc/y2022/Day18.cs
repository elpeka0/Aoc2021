using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoc.Geometry;

namespace Aoc.y2022
{
    public class Day18 : DayBase
    {
        public Day18() : base(18)
        {
        }

        private IEnumerable<Vector> GetInput()
        {
            foreach (var line in GetInputLines(false))
            {
                var coords = line.Split(',');
                yield return new Vector(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
            }
        }

        private Vector[] unity = new[]
        {
                new Vector(-1, 0, 0),
                new Vector(1, 0, 0),
                new Vector(0, 1, 0),
                new Vector(0, -1, 0),
                new Vector(0, 0, 1),
                new Vector(0, 0, -1)
        };

        public override void Solve()
        {
            var cubes = GetInput().ToHashSet();

            var res = 0;
            foreach (var cube in cubes)
            {
                res += unity.Count(u => !cubes.Contains(cube + u));
            }
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var cubes = GetInput().ToDictionary(c => c, c => 1000);
            var xmin = cubes.Keys.Min(c => c.X);
            var xmax = cubes.Keys.Max(c => c.X);
            var ymin = cubes.Keys.Min(c => c.Y);
            var ymax = cubes.Keys.Max(c => c.Y);
            var zmin = cubes.Keys.Min(c => c.Z);
            var zmax = cubes.Keys.Max(c => c.Z);

            for (var x = xmin; x <= xmax; x++)
            {
                for (var y = ymin; y <= ymax; y++)
                {
                    for (var z = zmin; z <= zmax; z++)
                    {
                        if (!cubes.ContainsKey(new Vector(x, y, z)))
                        {
                            cubes[new Vector(x, y, z)] = 0;
                        }
                    }
                }
            }

            var effective = true;
            while (effective)
            {
                effective = false;
                foreach (var c in cubes.ToDictionary(kv => kv.Key, kv => kv.Value))
                {
                    foreach (var u in unity)
                    {
                        var t = c.Key + u;
                        if (c.Value == 0)
                        {
                            if (t.X < xmin || t.X > xmax || t.Y < ymin || t.Y > ymax || t.Z < zmin || t.Z > zmax)
                            {
                                cubes[c.Key] = 1;
                                effective = true;
                            }
                            else if (cubes[t] == 1)
                            {
                                cubes[c.Key] = 1;
                                effective = true;
                            }
                        }
                    }
                }
            }

            var res = 0;
            foreach (var cube in cubes)
            {
                res += unity.Count(u => cube.Value == 1000 && (!cubes.TryGetValue(cube.Key + u, out var v) || v == 1));
            }
            Console.WriteLine(res);
        }
    }
}
