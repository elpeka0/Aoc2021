using System;
using System.Collections.Generic;

namespace Aoc
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
    }
}
