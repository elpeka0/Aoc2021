using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.Geometry
{
    public class GridSlice<T> : IEnumerable<T>
    {
        private readonly Grid<T> grid;
        private readonly Vector increment;
        private readonly int count;
        private readonly Vector start;

        public GridSlice(Grid<T> grid, Vector increment, int count, Vector start)
        {
            this.grid = grid;
            this.increment = increment;
            this.count = count;
            this.start = start;
        }
        public void Apply(Action<int, int> operation)
        {
            foreach (var (x, y) in this.Indexes())
            {
                operation(x, y);
            }
        }

        public IEnumerable<Vector> Indexes()
        {
            for (var i = 0; i < this.count; ++i)
            {
                yield return this.start + i*this.increment;
            }
        }

        public GridSlice<T> Invert() => new GridSlice<T>(this.grid, -this.increment, this.count, this.start + this.increment*(this.count - 1));

        public T this[int index]
        {
            get => this.grid[this.start + index * this.increment];
            set => this.grid[this.start + index * this.increment] = value;
        }

        public int Count => this.count;

        public IEnumerator<T> GetEnumerator()
        {
            return this.Indexes().Select(i => this.grid[i.X, i.Y]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
