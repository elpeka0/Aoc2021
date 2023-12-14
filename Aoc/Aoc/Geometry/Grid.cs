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

        public int Width => this.impl.MaxX -  this.impl.MinX + 1;
        public int Height => this.impl.MaxY - this.impl.MinY + 1;
        public int MaxX => this.impl.MaxX;
        public int MaxY => this.impl.MaxY;
        public int MinX => this.impl.MinX;
        public int MinY => this.impl.MinY;

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

        public IEnumerable<Vector> Indexes() => this.impl.Indexes();

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
            for (var y = MinY; y <= this.MaxY; ++y)
            {
                for (var x = MinX; x <= this.MaxX; ++x)
                {
                    sb.Append(this[x, y]);
                    sb.Append(' ');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public GridSlice<T> Row(int y) => new(this, new(1, 0), this.Width, new(MinX, y));
        public GridSlice<T> Column(int x) => new(this, new(0, 1), this.Height, new(x, MinY));

        public IEnumerable<GridSlice<T>> Rows()
        {
            for (var y = MinY; y <= this.MaxY; ++y)
            {
                yield return this.Row(y);
            }
        }
        public IEnumerable<GridSlice<T>> Columns()
        {
            for (var x = MinX; x <= this.MaxX; ++x)
            {
                yield return this.Column(x);
            }
        }

        public static Grid<T> WithSize(int width, int height)
        {
            return new Grid<T>(new ArrayGridImplementation<T>(width, height));
        }

        public static Grid<T> Dynamic()
        {
            return new Grid<T>(new DictionaryGridImplementation<T>());
        }

        public bool IsInBounds(Vector p)
        {
            return p.X >= MinX && p.X <= MaxX && p.Y >= MinY && p.Y <= MaxY;
        }

        public static Grid<T> FromLines(List<string> lines, Func<char, T> selector)
        {
            var grid = WithSize(lines.Max(l => l.Length), lines.Count);
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

        public override bool Equals(object obj)
        {
            var other = obj as Grid<T>;
            if (other == null)
            {
                return false;
            }

            if (other.Width != Width || other.Height != Height)
            {
                return false;
            }

            return this.Indexes().All(i => this[i].Equals(other[i]));
        }

        public override int GetHashCode()
        {
            const int largePrime = 111317;
            return this
                .Indexes()
                .Select(i => this[i].GetHashCode())
                .Aggregate((a, b) => unchecked(largePrime * a + b));
        }
    }
}
