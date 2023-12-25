using System;
using System.Collections.Generic;

namespace Aoc.Geometry
{
    public readonly struct LongVector : IEquatable<LongVector>
    {
        public long X { get; }
        public long Y { get; }
        public long Z { get; }

        public LongVector(long x, long y, long z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public LongVector(long x, long y)
            : this(x, y, 0)
        {
        }

        public LongVector Reorient(long permutation)
        {
            switch (permutation)
            {
                case 0: return new LongVector(this.X, this.Y, this.Z);
                case 1: return new LongVector(this.X, this.Z, this.Y);
                case 2: return new LongVector(this.Y, this.X, this.Z);
                case 3: return new LongVector(this.Y, this.Z, this.X);
                case 4: return new LongVector(this.Z, this.X, this.Y);
                case 5: return new LongVector(this.Z, this.Y, this.X);
                default: throw new InvalidOperationException();
            }
        }

        public IEnumerable<LongVector> Neighbors(bool diagonal)
        {
            yield return new(X - 1, Y);
            if (diagonal)
            {
                yield return new(X - 1, Y - 1);
            }
            if (diagonal)
            {
                yield return new(X - 1, Y + 1);
            }
            yield return new(X, Y - 1);
            yield return new(X, Y + 1);
            yield return new(X + 1, Y);
            if (diagonal)
            {
                yield return new(X + 1, Y - 1);
            }
            if (diagonal)
            {
                yield return new(X + 1, Y + 1);
            }
        }

        public LongVector Scale(LongVector scaling)
        {
            return new LongVector(this.X * scaling.X, this.Y * scaling.Y, this.Z * scaling.Z);
        }

        public bool Equals(LongVector other)
        {
            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is LongVector v && this.Equals(v);
        }

        public static bool operator ==(LongVector a, LongVector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LongVector a, LongVector b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Z);
        }

        public static LongVector operator +(LongVector a, LongVector b)
        {
            return new LongVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static LongVector operator -(LongVector a, LongVector b)
        {
            return new LongVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static LongVector operator -(LongVector a)
        {
            return new LongVector(-a.X, -a.Y, -a.Z);
        }

        public static LongVector operator *(LongVector a, long scalar)
        {
            return a.Scale(new LongVector(scalar, scalar, scalar));
        }

        public static LongVector operator *(long scalar, LongVector a)
        {
            return a.Scale(new LongVector(scalar, scalar, scalar));
        }

        public override string ToString()
        {
            return $"{this.X}, {this.Y}, {this.Z}";
        }

        public static IEnumerable<LongVector> Directions()
        {
            yield return new LongVector(1, 1, 1);
            yield return new LongVector(1, 1, -1);
            yield return new LongVector(1, -1, 1);
            yield return new LongVector(1, -1, -1);
            yield return new LongVector(-1, 1, 1);
            yield return new LongVector(-1, 1, -1);
            yield return new LongVector(-1, -1, 1);
            yield return new LongVector(-1, -1, -1);
        }

        public static readonly LongVector Origin = new LongVector();

        public void Deconstruct(out long x, out long y)
        {
            x = this.X;
            y = this.Y;
        }

        public void Deconstruct(out long x, out long y, out long z)
        {
            x = this.X;
            y = this.Y;
            z = this.Z;
        }

        public static explicit operator LongVector(DoubleVector v)
        {
            return new LongVector((long)v.X, (long)v.Y, (long)v.Z);
        }

        public LongVector Cross(LongVector other)
        {
            return new LongVector(
                Y * other.Z - Z * other.Y,
                Z * other.X - X * other.Z,
                X * other.Y - Y * other.Z
            );
        }
    }
}
