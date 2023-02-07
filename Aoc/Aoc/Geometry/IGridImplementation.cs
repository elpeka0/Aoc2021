using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Geometry
{
    public interface IGridImplementation<T>
    {
        int Width { get; }
        int Height { get; }

        T this[Vector v]
        {
            get;
            set;
        }
    }
}
