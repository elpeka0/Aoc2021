using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
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
            for (var i = 0; i < count; ++i)
            {
                yield return start + i*increment;
            }
        }

        public GridSlice<T> Invert() => new GridSlice<T>(grid, -this.increment, count, start + increment*(count - 1));

        public T this[int index]
        {
            get => grid[start + index * increment];
            set => grid[start + index * increment] = value;
        }

        public int Count => count;

        public IEnumerator<T> GetEnumerator()
        {
            return Indexes().Select(i => grid[i.X, i.Y]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
