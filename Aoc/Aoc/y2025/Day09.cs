using System;
using System.Collections.Generic;
using System.Linq;
using Aoc.Geometry;

namespace Aoc.y2025;

public class Day09 : DayBase
{
    public Day09() : base(9)
    {
    }

    public override void Solve()
    {
        var points = this.GetInput();
        var area = 0L;
        for (var i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var n = Area(points[i], points[j]);
                if (n > area)
                {
                    area = n;
                }
            }
        }
        Console.WriteLine(area);
    }

    private List<Vector> GetInput()
    {
        return this.GetInputLines()
            .Select(l => this.SplitInts(l, ',').ToArray())
            .Select(a => new Vector(a[0], a[1]))
            .ToList();
    }

    private long Area(Vector a, Vector b)
    {
        var l = Math.Abs(a.X - b.X) + 1;
        var w = Math.Abs(a.Y - b.Y) + 1;
        return (long)l * w;
    }

    private readonly record struct Segment(Vector A, Vector B)
    {
        public bool IsVertical => A.X == B.X;
        
        
        public Vector TopLeft => new Vector(Math.Min(A.X, B.X), Math.Min(A.Y, B.Y));
        public Vector BottomRight => new Vector(Math.Max(A.X, B.X), Math.Max(A.Y, B.Y));
    }

    public override void SolveMain()
    {
        var points = this.GetInput();

        var segments = points.Select((p, i) => i switch
        {
            0 => new Segment(points.Last(), points.First()),
            _ => new Segment(points[i - 1], points[i])
        }).ToList();
        
        var areas = new List<(Vector, Vector, long)>();
        for (var i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var a = points[i];
                var b = points[j];
                areas.Add((a, b, Area(a, b)));
            }
        }

        foreach (var (a, b, area) in areas.OrderByDescending(a => a.Item3))
        {
            if (IsValidRect(a, b, segments))
            {
                Console.WriteLine(area);
                return;
            }
        }

        Console.WriteLine("NOT FOUND!");
    }

    private bool IsValidRect(Vector a, Vector b, List<Segment> segments)
    {
        var topLeft = new Vector(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        var bottomRight = new Vector(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        
        foreach (var s in segments)
        {
            if (s.TopLeft.X < bottomRight.X
                && s.BottomRight.X > topLeft.X
                && s.TopLeft.Y < bottomRight.Y
                && s.BottomRight.Y > topLeft.Y)
            {
                return false;
            }
        }

        return IsInPerimeter(new DoubleVector(bottomRight.X + 0.5, bottomRight.Y + 0.5), segments);
    }

    private bool IsInPerimeter(DoubleVector v, List<Segment> segments)
    {
        var cnt = segments.Count(s => s.IsVertical && s.A.X < v.X && s.TopLeft.Y < v.Y && s.BottomRight.Y > v.Y);
        return cnt % 2 == 1;
    }
}
