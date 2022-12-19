using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day16 : DayBase
    {
        private record Link(Valve A, Valve B, int Cost)
        {
            public override string ToString()
            {
                return $"{A.Name} -> {B.Name} ({Cost})";
            }
        }

        private record Valve(string Name, int Rate, List<Link> Links)
        {
            public override string ToString()
            {
                return $"{Name} ({Rate}) => {string.Join(", ", Links.Select(n => n.ToString()))}";
            }
        }

        public Day16() : base(16)
        {
        }

        private Dictionary<string, Valve> GetInput()
        {
            var res = new Dictionary<string, Valve>();
            var lookup = new Dictionary<string, List<string>>();
            foreach (var line in GetInputLines(false))
            {
                var parts = line.Split();
                var name = parts[1];
                var rate = int.Parse(parts[4].Replace("rate=", string.Empty).Replace(";", string.Empty));
                var neighbors = new List<string>();
                for (var i = 9; i < parts.Length; i++)
                {
                    var n = parts[i].Replace(",", string.Empty);
                    neighbors.Add(n);
                }
                res[name] = new Valve(name, rate, new List<Link>());
                lookup[name] = neighbors;
            }
            foreach (var key in res.Keys)
            {
                res[key].Links.AddRange(lookup[key].Select(n => new Link(res[key], res[n], 1)));
            }
            return res;
        }

        public override void Solve()
        {
            var all = GetInput();
            var start = all["AA"];
            Optimize(all);

            var shortest = all.Values.ToDictionary(v => v, v => ShortestPath(v, all.Values));
            shortest[start] = ShortestPath(start, all.Values);
            var best = Best(30, 0, start, shortest, new HashSet<Valve>());
            Console.WriteLine(best);
        }

        private int Best(int time, int value, Valve valve, Dictionary<Valve, Dictionary<Valve, int>> shortest, HashSet<Valve> opened)
        {
            if (time == 0 || opened.Count == shortest.Count)
            {
                return value;
            }

            opened.Add(valve);
            if (valve.Rate > 0)
            {
                --time;
                value += time * valve.Rate;
            }

            var best = value;
            foreach (var key in shortest.Keys.Where(k => !opened.Contains(k)))
            {
                var c = shortest[valve][key];
                if (c < time)
                {
                    var o = opened.ToHashSet();
                    var next = Best(time - c, value, key, shortest, o);
                    if (next > best)
                    {
                        best = next;
                    }
                }
            }
            return best;
        }

        private record Step(Valve Valve, int Distance, string Who, bool Done);

        private (int, string) Best2(int time, int value, Step alpha, Step beta, Dictionary<Valve, Dictionary<Valve, int>> shortest, HashSet<Valve> opened)
        {
            if (time == 0 || opened.Count == shortest.Count)
            {
                return (value, string.Empty);
            }

            opened.Add(alpha.Valve);
            time -= alpha.Distance;
            beta = beta with { Distance = beta.Distance - alpha.Distance };
            if (alpha.Valve.Rate > 0)
            {
                value += time * alpha.Valve.Rate;
            }

            var best = value;
            var bestOp = string.Empty;
            var found = false;
            foreach (var key in shortest.Keys.Where(k => !opened.Contains(k) && beta.Valve != k))
            {
                var c = shortest[alpha.Valve][key] + 1;
                if (c < time)
                {
                    found = true;
                    var o = opened.ToHashSet();
                    var newAlpha = new Step(key, c, alpha.Who, false);
                    var newBeta = beta;
                    if (newBeta.Distance < newAlpha.Distance)
                    {
                        (newAlpha, newBeta) = (newBeta, newAlpha);
                    }

                    var (next, op) = Best2(time, value, newAlpha, newBeta, shortest, o);
                    if (next > best)
                    {
                        best = next;
                        //bestOp = $"{alpha.Who} to {alpha.Valve.Name} @ {26 - time}. {op}";
                    }
                }
            }

            if (!found && !beta.Done)
            {
                var o = opened.ToHashSet();
                var (next, op) = Best2(time, value, beta, alpha with { Distance = 10000, Done = true }, shortest, o);
                if (next > best)
                {
                    best = next;
                    //bestOp = $"{alpha.Who} to {alpha.Valve.Name} @ {time}. {op}";
                }
            }

            return (best, bestOp);
        }

        private Dictionary<Valve, int> ShortestPath(Valve start, IEnumerable<Valve> valves)
        {
            var unvisited = valves.ToHashSet();
            if (!unvisited.Contains(start))
            {
                unvisited.Add(start);
            }
            var known = new Dictionary<Valve, int>();
            known.Add(start, 0);
            while (unvisited.Count > 0)
            {
                var next = known.Where(kv => unvisited.Contains(kv.Key)).OrderBy(kv => kv.Value).First();
                unvisited.Remove(next.Key);
                foreach (var link in next.Key.Links)
                {
                    var cost = next.Value + link.Cost;
                    if (!known.ContainsKey(link.B) || known[link.B] > cost)
                    {
                        known[link.B] = cost;
                    }
                }
            }
            return known;
        }

        private void Optimize(Dictionary<string, Valve> all)
        {
            var effective = true;
            while (effective)
            {
                effective = false;
                foreach (var valve in all.Values)
                {
                    foreach (var link in valve.Links.ToList())
                    {
                        if (link.B.Rate == 0)
                        {
                            foreach (var sl in link.B.Links)
                            {
                                var newLink = new Link(valve, sl.B, link.Cost + sl.Cost);
                                var existing = valve.Links.FirstOrDefault(l => l.B == newLink.B);
                                if (existing == null)
                                {
                                    valve.Links.Add(newLink);
                                    effective = true;
                                }
                                else if (existing.Cost > newLink.Cost)
                                {
                                    valve.Links.Remove(existing);
                                    valve.Links.Add(newLink);
                                    effective = true;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var kv in all.ToList())
            {
                if (kv.Value.Rate == 0)
                {
                    all.Remove(kv.Key);
                }
            }
        }

        public override void SolveMain()
        {
            var all = GetInput();
            var start = all["AA"];
            Optimize(all);

            var shortest = all.Values.ToDictionary(v => v, v => ShortestPath(v, all.Values));
            shortest[start] = ShortestPath(start, all.Values);
            var (best, op) = Best2(26, 0, new Step(start, 0, "Human", false), new Step(start, 0, "Elephant", false), shortest, new HashSet<Valve>());
            Console.WriteLine(op);
            Console.WriteLine(best);
        }
    }
}
