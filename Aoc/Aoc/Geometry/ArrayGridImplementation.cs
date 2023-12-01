using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Geometry
{
    public class ArrayGridImplementation<T> : IGridImplementation<T>
    {
        private readonly T[,] grid;
        public int MaxX { get; }
        public int MaxY { get; }
        public int MinX => 0;
        public int MinY => 0;

        public IEnumerable<Vector> Indexes()
        {
            for (var x = MinX; x <= MaxX; x++)
            {
                for (var y = MinY; y <= MaxY; y++)
                {
                    yield return new Vector(x, y);
                }
            }
        }

        public T this[Vector v]
        {
            get => this.grid[v.X, v.Y];
            set => this.grid[v.X, v.Y] = value;
        }

        public ArrayGridImplementation(int width, int height)
        {
            this.grid = new T[width, height];
            this.MaxX = width - 1;
            this.MaxY = height - 1;
        }
    }
}
