using System;
using System.Collections.Generic;

namespace Aoc.Geometry
{
    public readonly struct DoubleVector : IEquatable<DoubleVector>
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public DoubleVector(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public DoubleVector(double x, double y)
            : this(x, y, 0)
        {
        }

        public DoubleVector Reorient(double permutation)
        {
            switch (permutation)
            {
                case 0: return new DoubleVector(this.X, this.Y, this.Z);
                case 1: return new DoubleVector(this.X, this.Z, this.Y);
                case 2: return new DoubleVector(this.Y, this.X, this.Z);
                case 3: return new DoubleVector(this.Y, this.Z, this.X);
                case 4: return new DoubleVector(this.Z, this.X, this.Y);
                case 5: return new DoubleVector(this.Z, this.Y, this.X);
                default: throw new InvalidOperationException();
            }
        }

        public IEnumerable<DoubleVector> Neighbors(bool diagonal)
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

        public DoubleVector Scale(DoubleVector scaling)
        {
            return new DoubleVector(this.X * scaling.X, this.Y * scaling.Y, this.Z * scaling.Z);
        }

        public bool Equals(DoubleVector other)
        {
            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is DoubleVector v && this.Equals(v);
        }

        public static bool operator ==(DoubleVector a, DoubleVector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DoubleVector a, DoubleVector b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Z);
        }

        public static DoubleVector operator +(DoubleVector a, DoubleVector b)
        {
            return new DoubleVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static DoubleVector operator -(DoubleVector a, DoubleVector b)
        {
            return new DoubleVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static DoubleVector operator -(DoubleVector a)
        {
            return new DoubleVector(-a.X, -a.Y, -a.Z);
        }

        public static DoubleVector operator *(DoubleVector a, double scalar)
        {
            return a.Scale(new DoubleVector(scalar, scalar, scalar));
        }

        public static DoubleVector operator *(double scalar, DoubleVector a)
        {
            return a.Scale(new DoubleVector(scalar, scalar, scalar));
        }

        public override string ToString()
        {
            return $"{this.X}, {this.Y}, {this.Z}";
        }

        public static IEnumerable<DoubleVector> Directions()
        {
            yield return new DoubleVector(1, 1, 1);
            yield return new DoubleVector(1, 1, -1);
            yield return new DoubleVector(1, -1, 1);
            yield return new DoubleVector(1, -1, -1);
            yield return new DoubleVector(-1, 1, 1);
            yield return new DoubleVector(-1, 1, -1);
            yield return new DoubleVector(-1, -1, 1);
            yield return new DoubleVector(-1, -1, -1);
        }

        public static readonly DoubleVector Origin = new DoubleVector();

        public void Deconstruct(out double x, out double y)
        {
            x = this.X;
            y = this.Y;
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = this.X;
            y = this.Y;
            z = this.Z;
        }

        public static explicit operator DoubleVector(LongVector v)
        {
            return new DoubleVector(v.X, v.Y, v.Z);
        }

        public DoubleVector Cross(DoubleVector other)
        {
            return new DoubleVector(
                Y*other.Z - Z*other.Y,
                Z*other.X - X*other.Z,
                X*other.Y - Y*other.Z
            );
        }
    }
}
