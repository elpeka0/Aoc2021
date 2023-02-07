using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoc.Geometry
{
    public class Grid<T> : IEnumerable<T>
    {
        private readonly IGridImplementation<T> impl;

        public Grid(IGridImplementation<T> impl)
        {
            this.impl = impl;
        }

        public Grid(int width, int height) : this(new ArrayGridImplementation<T>(width, height))
        {
        }

        public T this[int x, int y]
        {
            get => this.impl[new (x, y)];
            set => this.impl[new (x, y)] = value;
        }

        public T this[Vector v] 
        { 
            get => this.impl[v];
            set => this.impl[v] = value;
        }

        public int Width => this.impl.Width;
        public int Height => this.impl.Height;

        public void Apply(Action<Vector> operation)
        {
            foreach (var v in this.Indexes())
            {
                operation(v);
            }
        }

        public void Fill(T value)
        {
            this.Apply(v => this.impl[v] = value);
        }

        public IEnumerable<Vector> Indexes()
        {
            for (var x = 0; x < this.Width; ++x)
            {
                for (var y = 0; y < this.Height; ++y)
                {
                    yield return new(x, y);
                }
            }
        }

        public IEnumerable<Vector> Neighbors(Vector v, bool diagonal) => this.Neighbors(v.X, v.Y, diagonal);

        public IEnumerable<Vector> Neighbors(int x, int y, bool diagonal)
        {
            if (x > 0)
            {
                yield return new(x - 1, y);
                if (y > 0 && diagonal)
                {
                    yield return new(x - 1, y - 1);
                }
                if (y < this.Height - 1 && diagonal)
                {
                    yield return new(x - 1, y + 1);
                }
            }
            if (y > 0)
            {
                yield return new(x, y - 1);
            }
            if (y < this.Height- 1)
            {
                yield return new(x, y + 1);
            }
            if (x < this.Width - 1)
            {
                yield return new(x + 1, y);
                if (y > 0 && diagonal)
                {
                    yield return new(x + 1, y - 1);
                }
                if (y < this.Height - 1 && diagonal)
                {
                    yield return new(x + 1, y + 1);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var v in this.Indexes())
            {
                yield return this[v];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < this.Height; ++y)
            {
                for (int x = 0; x < this.Width; ++x)
                {
                    sb.Append(this[x, y]);
                    sb.Append(' ');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public GridSlice<T> Row(int y) => new(this, new(1, 0), this.Width, new(0, y));
        public GridSlice<T> Column(int x) => new(this, new(0, 1), this.Height, new(x, 0));

        public IEnumerable<GridSlice<T>> Rows()
        {
            for (int y = 0; y < this.Height; ++y)
            {
                yield return this.Row(y);
            }
        }
        public IEnumerable<GridSlice<T>> Columns()
        {
            for (int x = 0; x < this.Width; ++x)
            {
                yield return this.Column(x);
            }
        }

        public static Grid<T> FromLines(List<string> lines, Func<char, T> selector)
        {
            var grid = new Grid<T>(lines.Max(l => l.Length), lines.Count);
            var y = 0;
            foreach (var line in lines)
            {
                var x = 0;
                foreach (var c in line)
                {
                    grid[x, y] = selector(c);
                    ++x;
                }
                ++y;
            }
            return grid;
        }
    }
}
