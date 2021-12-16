﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Grid<T> : IEnumerable<T>
    {
        private readonly T[,] grid;
        public int Width { get; }
        public int Height { get; }

        public Grid(int width, int height)
        {
            this.grid = new T[width, height];
            this.Width = width;
            this.Height = height;
        }

        public T this[int x, int y]
        {
            get => this.grid[x, y];
            set => this.grid[x, y] = value;
        }

        public void Apply(Action<int, int> operation)
        {
            foreach (var (x, y) in this.Indexes())
            {
                operation(x, y);
            }
        }

        public void Fill(T value)
        {
            Apply((x, y) => this.grid[x, y] = value);
        }

        public IEnumerable<(int X, int Y)> Indexes()
        {
            for (var x = 0; x < Width; ++x)
            {
                for (var y = 0; y < Height; ++y)
                {
                    yield return (x, y);
                }
            }
        }

        public IEnumerable<(int X, int Y)> Neighbors(int x, int y, bool diagonal)
        {
            if (x > 0)
            {
                yield return (x - 1, y);
                if (y > 0 && diagonal)
                {
                    yield return (x - 1, y - 1);
                }
                if (y < Height - 1 && diagonal)
                {
                    yield return (x - 1, y + 1);
                }
            }
            if (y > 0)
            {
                yield return (x, y - 1);
            }
            if (y < Height- 1)
            {
                yield return (x, y + 1);
            }
            if (x < Width - 1)
            {
                yield return (x + 1, y);
                if (y > 0 && diagonal)
                {
                    yield return (x + 1, y - 1);
                }
                if (y < Height - 1 && diagonal)
                {
                    yield return (x + 1, y + 1);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var x in this.grid)
            {
                yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.grid.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    sb.Append(this.grid[x, y]);
                    sb.Append(' ');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
