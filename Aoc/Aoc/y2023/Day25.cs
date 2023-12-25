using Aoc.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day25 : DayBase
    {
        public Day25() : base(25)
        {
        }

        private Dictionary<string, List<string>> Load()
        {
            var graph = new Dictionary<string, List<string>>();
            foreach (var t in Letter()
                .String()
                .ThenWs(":")
                .ThenWs(Letter().String().RepeatWithSeperator(Whitespace()))
                .Evaluate(GetInputLines(false).ToList()))
            {
                foreach (var i in t.Item2)
                {
                    Link(t.Item1, i);
                    Link(i, t.Item1);
                }
            }

            void Link(string a, string b)
            {
                if (!graph.TryGetValue(a, out var l))
                {
                    l = new List<string>();
                    graph[a] = l;
                }
                if (!l.Contains(b))
                {
                    l.Add(b);
                }
            }
            return graph;
        }

        private record Link(string A, string B);

        public override void Solve()
        {
            var graph = Load();
            var links = graph.SelectMany(kv => kv.Value.Select(v => new Link(kv.Key, v))).ToList();
            var set = FindCutoff();
            var h1 = Utils.FloodFill(set.First().A, (n, _) => Step(n, set));
            var h2 = Utils.FloodFill(set.First().B, (n, _) => Step(n, set));
            Console.WriteLine(h1.Count * h2.Count);

            HashSet<Link> FindCutoff()
            {
                for (var k = 0;k < links.Count;k++)
                {
                    var first = links[k];
                    var path1 = FindPath(first.A, first.B, new[] { first }.ToHashSet());
                    Debug.Assert(path1 != null);
                    for (var i = 0; i < path1.Count - 1; i++)
                    {
                        var second = new Link(path1[i], path1[i + 1]);
                        var path2 = FindPath(first.A, first.B, new[] { first, second }.ToHashSet());
                        Debug.Assert(path2 != null);
                        for (var j = 0; j < path2.Count - 1; j++)
                        {
                            var third = new Link(path2[j], path2[j + 1]);
                            var path3 = FindPath(first.A, first.B, new[] { first, second, third }.ToHashSet());
                            if (path3 == null)
                            {
                                return new[] { first, second, third }.ToHashSet();
                            }
                        }
                    }
                    Console.WriteLine($"{k + 1}/{links.Count}");
                }

                Debug.Assert(false);
                return null;
            }

            List<string> FindPath(string a, string b, HashSet<Link> forbidden)
            {
                return Utils.Bfs(
                    a, 
                    n => Step(n, forbidden), 
                    n => n == b);
            }

            IEnumerable<string> Step(string n, HashSet<Link> forbidden)
            {
                return graph.GetWithDefault(n, new List<string>()).Where(s => !forbidden.Contains(new(n, s)) && !forbidden.Contains(new(s, n)));
            }
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
