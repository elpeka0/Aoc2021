using System;
using System.Collections.Generic;

namespace Aoc.Geometry
{
    public readonly struct Vector : IEquatable<Vector>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public Vector(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector(int x, int y)
            : this(x, y, 0)
        {
        }

        public Vector Reorient(int permutation)
        {
            switch (permutation)
            {
                case 0: return new Vector(this.X, this.Y, this.Z);
                case 1: return new Vector(this.X, this.Z, this.Y);
                case 2: return new Vector(this.Y, this.X, this.Z);
                case 3: return new Vector(this.Y, this.Z, this.X);
                case 4: return new Vector(this.Z, this.X, this.Y);
                case 5: return new Vector(this.Z, this.Y, this.X);
                default: throw new InvalidOperationException();
            }
        }

        public IEnumerable<Vector> Neighbors(bool diagonal)
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

        public Vector Scale(Vector scaling)
        {
            return new Vector(this.X * scaling.X, this.Y * scaling.Y, this.Z * scaling.Z);
        }

        public bool Equals(Vector other)
        {
            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector v && this.Equals(v);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Z);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y, -a.Z);
        }

        public static Vector operator *(Vector a, int scalar)
        {
            return a.Scale(new Vector(scalar, scalar, scalar));
        }

        public static Vector operator *(int scalar, Vector a)
        {
            return a.Scale(new Vector(scalar, scalar, scalar));
        }

        public override string ToString()
        {
            return $"{this.X}, {this.Y}, {this.Z}";
        }

        public static IEnumerable<Vector> Directions()
        {
            yield return new Vector(1, 1, 1);
            yield return new Vector(1, 1, -1);
            yield return new Vector(1, -1, 1);
            yield return new Vector(1, -1, -1);
            yield return new Vector(-1, 1, 1);
            yield return new Vector(-1, 1, -1);
            yield return new Vector(-1, -1, 1);
            yield return new Vector(-1, -1, -1);
        }

        public static readonly Vector Origin = new Vector();

        public void Deconstruct(out int x, out int y)
        {
            x = this.X;
            y = this.Y;
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = this.X;
            y = this.Y;
            z = this.Z;
        }
    }
}
