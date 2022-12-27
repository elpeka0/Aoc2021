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
                for (var c = 0; c < data.GetLength(1); ++r)
                {
                    if (this[r, c] != other[r, c])
                    {
                        return false;
                    }
                }
            }
            return true;
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

        public static Vector operator *(Vector a, TransformMatrix m)
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
            for (var r = 0; r < res.GetLength(0); ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a[r, c] + b[r, c];
                }
            }
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator -(TransformMatrix a, TransformMatrix b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0); ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a[r, c] - b[r, c];
                }
            }
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator *(TransformMatrix a, int b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0); ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a[r, c] * b;
                }
            }
            return new TransformMatrix(res);
        }

        public static TransformMatrix operator *(int a, TransformMatrix b)
        {
            var res = new int[4, 4];
            for (var r = 0; r < res.GetLength(0); ++r)
            {
                for (var c = 0; c < res.GetLength(1); ++c)
                {
                    res[r, c] = a * b[r, c];
                }
            }
            return new TransformMatrix(res);
        }

        public static TransformMatrix Identity => Scale(new Vector(1, 1, 1));

        public static TransformMatrix Translate(Vector v)
        {
            return new TransformMatrix
                (
                0, 0, 0, v.X,
                0, 0, 0, v.Y,
                0, 0, 0, v.Z
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
    }
}
