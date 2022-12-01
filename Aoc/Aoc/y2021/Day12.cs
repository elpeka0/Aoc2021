using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.y2021
{
    public class Day12 : DayBase
    {
        private class Node
        {
            public string Name { get; }
            public bool Large => char.IsUpper(this.Name[0]);

            public List<Edge> Edges { get; } = new List<Edge>();

            public Node(string name)
            {
                this.Name = name;
            }

            public override string ToString() => this.Name;
        }

        private class Edge
        {
            public Node From { get; }
            public Node To { get; }

            public Edge(Node from, Node to)
            {
                this.From = from;
                this.To = to;
                this.From.Edges.Add(this);
            }

            public override string ToString() => $"{this.From}->{this.To}";
        }

        private class Path
        {
            public List<Node> PathNodes { get; }
            public HashSet<Node> Visited { get; }
            public bool SecondSmall { get; set; }

            public Path()
            {
                this.PathNodes = new List<Node>();
                this.Visited = new HashSet<Node>();
            }

            private Path(Path parent)
            {
                this.PathNodes = parent.PathNodes.ToList();
                this.Visited = parent.Visited.ToHashSet();
                this.SecondSmall = parent.SecondSmall;
            }

            public Path Copy() => new Path(this);

            public override string ToString() => string.Join("->", this.PathNodes);

            public bool Visit(Node next, bool part2)
            {
                if (this.CanVisit(next, part2))
                {
                    this.PathNodes.Add(next);
                    this.Visited.Add(next);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool CanVisit(Node next, bool part2)
            {
                if (!next.Large && this.Visited.Contains(next))
                {
                    if (part2 && !this.SecondSmall)
                    {
                        this.SecondSmall = true;
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        public Day12() : base(12)
        {
        }

        private IEnumerable<Path> FindPaths(Dictionary<string, Node> nodes, Path active, Node current, bool part2)
        {
            if (!active.Visit(current, part2))
            {
                yield break;
            }
            if (current.Name == "end")
            {
                yield return active;
            }
            else
            {
                foreach (var edge in current.Edges)
                {
                    foreach (var child in this.FindPaths(nodes, active.Copy(), edge.To, part2))
                    {
                        yield return child;
                    }
                }
            }
        }

        private Dictionary<string, Node> LoadGraph()
        {
            var nodes = new Dictionary<string, Node>();
            foreach (var (f, t) in this.GetInputLines(false).Select(s => s.Split('-')).Select(p => (p[0], p[1])))
            {
                if (!nodes.TryGetValue(f, out var from))
                {
                    from = new Node(f);
                    nodes.Add(f, from);
                }
                if (!nodes.TryGetValue(t, out var to))
                {
                    to = new Node(t);
                    nodes.Add(t, to);
                }
                if (to.Name != "start")
                {
                    new Edge(from, to);
                }
                if (from.Name != "start")
                {
                    new Edge(to, from);
                }
            }
            return nodes;
        }

        public override void Solve()
        {
            var nodes = this.LoadGraph();
            var all = this.FindPaths(nodes, new Path(), nodes["start"], false).ToList();
            Console.WriteLine(all.Count);
        }

        public override void SolveMain()
        {
            var nodes = this.LoadGraph();
            var all = this.FindPaths(nodes, new Path(), nodes["start"], true).ToList();
            Console.WriteLine(all.Count);
        }
    }
}
