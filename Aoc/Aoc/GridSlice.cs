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
        private readonly int incrementX;
        private readonly int incrementY;
        private readonly int count;
        private readonly int startX;
        private readonly int startY;

        public GridSlice(Grid<T> grid, int incrementX, int incrementY, int count, int startX, int startY)
        {
            this.grid = grid;
            this.incrementX = incrementX;
            this.incrementY = incrementY;
            this.count = count;
            this.startX = startX;
            this.startY = startY;
        }
        public void Apply(Action<int, int> operation)
        {
            foreach (var (x, y) in this.Indexes())
            {
                operation(x, y);
            }
        }

        public IEnumerable<(int X, int Y)> Indexes()
        {
            for (var i = 0; i < count; ++i)
            {
                yield return (startX + i*incrementX, startY + i*incrementY);
            }
        }

        public GridSlice<T> Invert() => new GridSlice<T>(grid, -incrementX, -incrementY, count, startX + incrementX*(count - 1), startY + incrementY*(count - 1));

        public T this[int index]
        {
            get => grid[startX + index * incrementX, startY + index * incrementY];
            set => grid[startX + index * incrementX, startY + index * incrementY] = value;
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
