using Aoc.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2023
{
    public class Day23 : DayBase
    {
        public Day23() : base(23)
        {
        }

        public override void Solve()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Row(0).Indexes().First(i => grid[i] == '.');
            var target = grid.Row(grid.Height - 1).Indexes().First(i => grid[i] == '.');

            var res = LongestPath(start, target, grid, CanMove);
            Console.WriteLine(res);

            IEnumerable<Vector> CanMove(Vector p)
            {
                return grid[p] switch
                {
                    '.' => grid.Neighbors(p, false),
                    '>' => new[] { p + new Vector(1, 0) },
                    '<' => new[] { p + new Vector(-1, 0) },
                    '^' => new[] { p + new Vector(0, -1) },
                    'v' => new[] { p + new Vector(0, 1) }
                };
            }
        }

        public override void SolveMain()
        {
            var grid = Grid<char>.FromLines(GetInputLines(false).ToList(), c => c);
            var start = grid.Row(0).Indexes().First(i => grid[i] == '.');
            var target = grid.Row(grid.Height - 1).Indexes().First(i => grid[i] == '.');

            var res = LongestPath(start, target, grid, p => grid.Neighbors(p, false));
            Console.WriteLine(res);
        }

        private record TreeLink(int Length, TreeNode Child)
        {
            public override string ToString()
            {
                return $"{Length} -> {Child}";
            }
        }

        private class TreeNode
        {
            public List<TreeLink> Links { get; } = new List<TreeLink>();

            public bool IsTarget { get; }

            public Vector Pos { get; }

            public TreeNode(Vector pos, bool isTarget)
            {
                Pos = pos;
                IsTarget = isTarget;
            }

            public override string ToString()
            {
                return $"[{(IsTarget ? "Target " : "")}{Pos}]";
            }

            public string Visualize()
            {
                var sb = new StringBuilder();
                Utils.FloodFill(this, (n, _) =>
                {
                    foreach (var link in n.Links)
                    {
                        sb.AppendLine($"{n}->{link.Child}({link.Length})");
                    }
                    return n.Links.Select(l => l.Child);
                });
                return sb.ToString();
            }
        }

        private TreeNode IntoTree(Vector start, Vector target, Grid<char> grid, Func<Vector, IEnumerable<Vector>> stepFunc)
        {
            var lookup = new Dictionary<Vector, TreeNode>();
            Utils.FloodFill(start, (v, _) =>
            {
                lookup[v] = new TreeNode(v, v == target);
                return stepFunc(v).Where(n => grid[n] != '#');
            });

            Utils.FloodFill(lookup[start], (n, _) =>
            {
                n.Links.AddRange(stepFunc(n.Pos).Where(i => grid[i] != '#').Select(i => new TreeLink(1, lookup[i])));
                return n.Links.Select(l => l.Child);
            });

            Utils.FloodFill(lookup[start], (n, _) =>
            {
                var r = n.Links.Select(l => Shorten(l, n))
                    .Where(n => n != null)
                    .GroupBy(n => n.Child)
                    .Select(g => g.OrderByDescending(n => n.Length).First())
                    .ToList();
                n.Links.Clear();
                n.Links.AddRange(r);
                return n.Links.Select(l => l.Child);
            });

            return lookup[start];
        }

        TreeLink Shorten(TreeLink link, TreeNode head)
        {
            var cost = 1;
            while (true)
            {
                var l = link.Child.Links.Where(n => n.Child != head).ToList();
                if (l.Count != 1 || l[0].Length > 1)
                {
                    return new TreeLink(cost, link.Child);
                }
                head = link.Child;
                link = l[0];
                cost++;
            }
        }

        private long LongestPath(Vector start, Vector target, Grid<char> grid, Func<Vector, IEnumerable<Vector>> stepFunc)
        {
            var root = IntoTree(start, target, grid, stepFunc);
            Console.WriteLine(root.Visualize());
            return Impl(root, new HashSet<TreeNode>());

            long Impl(TreeNode s, HashSet<TreeNode> visited)
            {
                if (s.IsTarget)
                {
                    return 0;
                }
                var best = -1L;
                foreach (var n in s.Links)
                {
                    if (!visited.Contains(n.Child))
                    {
                        visited.Add(s);
                        var b = Impl(n.Child, visited.ToHashSet());
                        if (b >= 0 && b + n.Length > best)
                        {
                            best = b + n.Length;
                        }
                    }
                }
                return best;
            }
        }
    }
}
