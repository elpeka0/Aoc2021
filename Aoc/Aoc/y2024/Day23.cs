using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Aoc.y2024
{
    public class Day23 : DayBase
    {
        public Day23() : base(23)
        {
        }

        private record Grouping(string A, string B, string C);

        public override void Solve()
        {
            var links = BuildLinks();

            var res = new HashSet<Grouping>();
            foreach (var kv in links.Where(x => x.Key.StartsWith("t")))
            {
                foreach (var v in kv.Value)
                {
                    foreach (var o in links[v])
                    {
                        if (kv.Value.Contains(o))
                        {
                            var a = new[] { kv.Key, v, o }.OrderBy(x => x).ToList();
                            res.Add(new Grouping(a[0], a[1], a[2]));
                        }
                    }
                }
            }
            Console.WriteLine(res.Count);
        }

        private Dictionary<string, HashSet<string>> BuildLinks()
        {
            var input = GetInputLines().Select(l => l.Split('-')).Select(p => (A: p[0], B: p[1])).ToList();
            var links = new Dictionary<string, HashSet<string>>();
            foreach (var link in input)
            {
                if (!links.TryGetValue(link.A, out var l))
                {
                    l = new HashSet<string>();
                    links.Add(link.A, l);
                }
                l.Add(link.B);
                if (!links.TryGetValue(link.B, out l))
                {
                    l = new HashSet<string>();
                    links.Add(link.B, l);
                }
                l.Add(link.A);
            }

            return links;
        }

        public override void SolveMain()
        {
            var links = BuildLinks();
            var bestSets = new Dictionary<string, HashSet<string>>();
            bestSets.Add(string.Empty, new HashSet<string>());

            while (true)
            {
                var next = new Dictionary<string, HashSet<string>>();
                foreach (var key in links.Keys)
                {
                    foreach (var b in bestSets.Values)
                    {
                        if (b.All(links[key].Contains))
                        {
                            var c = b.ToHashSet();
                            c.Add(key);
                            next[KeyOf(c)] = c;
                        }
                    }
                }
                if (next.Count == 0)
                {
                    break;
                }
                bestSets = next;
            }

            Console.WriteLine(bestSets.Keys.First());
        }

        private string KeyOf(HashSet<string> set)
        {
            return string.Join(",", set.OrderBy(x => x));
        }
    }
}
