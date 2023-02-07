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
        public int Width { get; }
        public int Height { get; }

        public T this[Vector v]
        {
            get => this.grid[v.X, v.Y];
            set => this.grid[v.X, v.Y] = value;
        }

        public ArrayGridImplementation(int width, int height)
        {
            this.grid = new T[width, height];
            this.Width = width;
            this.Height = height;
        }
    }
}
