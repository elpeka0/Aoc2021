using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Geometry
{
    public interface IGridImplementation<T>
    {
        int MaxX { get; }
        int MaxY { get; }
        int MinX { get; }
        int MinY { get; }

        IEnumerable<Vector> Indexes();

        T this[Vector v]
        {
            get;
            set;
        }
    }
}
