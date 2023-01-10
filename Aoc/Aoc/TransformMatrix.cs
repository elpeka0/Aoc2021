using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public struct TransformMatrix : IEquatable<TransformMatrix>
    {
        private readonly int[,] data;

        public TransformMatrix()
        {
            data = new int[4, 4];
        }

        private TransformMatrix(int[,] data)
        {
            this.data = data;
        }

        public TransformMatrix(
            int a00, int a01, int a02, int a03,
            int a10, int a11, int a12, int a13,
            int a20, int a21, int a22, int a23)
        {
            data = new int[4, 4];
            data[0, 0] = a00;
            data[0, 1] = a01;
            data[0, 2] = a02;
            data[0, 3] = a03;
            data[1, 0] = a10;
            data[1, 1] = a11;
            data[1, 2] = a12;
            data[1, 3] = a13;
            data[2, 0] = a20;
            data[2, 1] = a21;
            data[2, 2] = a22;
            data[2, 3] = a23;
            data[3, 0] = 0;
            data[3, 1] = 0;
            data[3, 2] = 0;
            data[3, 3] = 1;
        }

        public int this[int row, int column]
        {
            get => data[row, column];
        }

        public bool Equals(TransformMatrix other)
        {
            for (var r = 0; r < data.GetLength(0); ++r)
            {
                for (var c = 0; c < data.GetLength(1); ++c)
                {
                    if (this[r, c] != other[r, c])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is TransformMatrix m && Equals(m);
        }

        public override int GetHashCode()
        {
            var hc = 0;
            for (var r = 0; r < data.GetLength(0) - 1; ++r)
            {
                for (var c = 0; c < data.GetLength(1); ++c)
                {
                    if (r == 0 && c == 0)
                    {
                        hc = this[r, c];
                    }
                    else
                    {
                        hc = HashCode.Combine(hc, this[r, c]);
                    }
                }
            }
            return hc;
        }

        public static bool operator ==(TransformMatrix a, TransformMatrix b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TransformMatrix a, TransformMatrix b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var r = 0; r < data.GetLength(0); ++r)
            {
                for (var c = 0; c < data.GetLength(1); ++c)
                {
                    sb.Append(this[r, c]);
                    sb.Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static Vector operator *(TransformMatrix m, Vector a)
        {
            return new Vector
            (
                a.X * m[0, 0] + a.Y * m[0, 1] + a.Z * m[0, 2] + m[0, 3],
                a.X * m[1, 0] + a.Y * m[1, 1] + a.Z * m[1, 2] + m[1, 3],
                a.X * m[2, 0] + a.Y * m[2, 1] + a.Z * m[2, 2] + m[2, 3]
            );
        }

        public static TransformMatrix operator *(TransformMatrix a, TransformMatrix b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0); ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    for (var i = 0; i < res.GetLength(0); ++i)
                    {
                        res[r, c] += a[r, i] * b[i, c];
                    }
                }
            }
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator +(TransformMatrix a, TransformMatrix b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0) - 1; ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a[r, c] + b[r, c];
                }
            }
            res[3, 3] = 1;
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator -(TransformMatrix a, TransformMatrix b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0) - 1; ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a[r, c] - b[r, c];
                }
            }
            res[3, 3] = 1;
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator -(TransformMatrix a)
        {
            return -1 * a;
        }

        public static TransformMatrix operator *(TransformMatrix a, int b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0) - 1; ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a[r, c] * b;
                }
            }
            res[3, 3] = 1;
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator *(int a, TransformMatrix b)
        {
            return b * a;
        }

        public static TransformMatrix Identity => Scale(new Vector(1, 1, 1));

        public static TransformMatrix Translate(Vector v)
        {
            return new TransformMatrix
                (
                1, 0, 0, v.X,
                0, 1, 0, v.Y,
                0, 0, 1, v.Z
                );
        }

        public static TransformMatrix Scale(Vector v)
        {
            return new TransformMatrix(
                v.X, 0, 0, 0,
                0, v.Y, 0, 0,
                0, 0, v.Z, 0
                );
        }

        public static TransformMatrix Rotate(Vector pos, Vector v, int quarterCircles)
        {
            quarterCircles %= 4;
            if (quarterCircles < 0)
            {
                quarterCircles += 4;
            }
            if (quarterCircles == 3)
            {
                v = -v;
                quarterCircles = 1;
            }
            else if (quarterCircles == 0)
            {
                return Identity;
            }
            
            var m = v switch
            {
                (1, 0, 0) => new TransformMatrix
                (
                    1, 0, 0, 0,
                    0, 0, -1, 0,
                    0, 1, 0, 0
                ),
                (0, 1, 0) => new TransformMatrix
                (
                    0, 0, -1, 0,
                    0, 1, 0, 0,
                    1, 0, 0, 0
                ),
                (0, 0, 1) => new TransformMatrix
                (
                    0, -1, 0, 0,
                    1, 0, 0, 0,
                    0, 0, 1, 0
                ),
                (-1, 0, 0) => new TransformMatrix
                (
                    1, 0, 0, 0,
                    0, 0, 1, 0,
                    0, -1, 0, 0
                ),
                (0, -1, 0) => new TransformMatrix
                (
                    0, 0, 1, 0,
                    0, 1, 0, 0,
                    -1, 0, 0, 0
                ),
                (0, 0, -1) => new TransformMatrix
                (
                    0, 1, 0, 0,
                    -1, 0, 0, 0,
                    0, 0, 1, 0
                ),
                _ => throw new NotImplementedException("Guess I have to implement that")
            };
            if (quarterCircles == 2)
            {
                return m * m;
            }
            return Translate(pos) * m * Translate(-pos);
        }

        public bool Effective(int row)
        {
            return this[row, 0] + this[row, 1] + this[row, 2] != 0;
        }
    }
}
