using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Geometry
{
    public class DictionaryGridImplementation<T> : IGridImplementation<T>
    {
        private readonly Dictionary<Vector, T> grid = new Dictionary<Vector, T>();
        private readonly T defaultValue;

        public int MaxX { get; private set; } = int.MinValue;
        public int MaxY { get; private set; } = int.MinValue;
        public int MinX { get; private set; } = int.MaxValue;
        public int MinY { get; private set; } = int.MaxValue;

        public IEnumerable<Vector> Indexes() => grid.Keys;

        public T this[Vector v]
        {
            get 
            {
                if (this.grid.TryGetValue(v, out var res))
                {
                    return res;
                }
                else
                {
                    return defaultValue;
                }
            }
            set 
            {
                this.grid[v] = value;
                if (v.X > MaxX)
                {
                    MaxX = v.X;
                }

                if (v.Y > MaxY)
                {
                    MaxY = v.Y;
                }

                if (v.X < MinX)
                {
                    MinX = v.X;
                }

                if (v.Y < MinY)
                {
                    MinY = v.Y;
                }
            }
        }
    }
}
