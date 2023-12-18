using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Aoc
{
    public static class Utils
    {
        public static int ExtractInt(string bullshit, bool signed)
        {
            return int.Parse(new string(bullshit.Where(c => char.IsDigit(c) || (c == '-' && signed)).ToArray()));
        }

        private record Link<T>(T Current, Link<T> Previous);

        public static List<T> Bfs<T>(T initial, Func<T, IEnumerable<T>> step, Func<T, bool> done)
        {
            var queue = new Queue<Link<T>>();
            queue.Enqueue(new Link<T>(initial, null));
            var visited = new HashSet<T>();
            while (queue.Any())
            {
                var next = queue.Dequeue();
                if (done(next.Current))
                {
                    var res = new List<T>();
                    for (var p = next; p != null; p = p.Previous)
                    {
                        res.Add(p.Current);
                    }
                    res.Reverse();
                    return res;
                }
                foreach (var e in step(next.Current))
                {
                    if (!visited.Contains(e))
                    {
                        visited.Add(e);
                        queue.Enqueue(new Link<T>(e, next));
                    }
                }
            }
            return null;
        }

        public static HashSet<T> FloodFill<T>(T initial, Func<T, IEnumerable<T>> step)
        {
            var queue = new Queue<Link<T>>();
            queue.Enqueue(new Link<T>(initial, null));
            var visited = new HashSet<T>();
            while (queue.Any())
            {
                var next = queue.Dequeue();
                visited.Add(next.Current);
                foreach (var e in step(next.Current))
                {
                    if (!visited.Contains(e))
                    {
                        queue.Enqueue(new Link<T>(e, next));
                    }
                }
            }
            return visited;
        }

        public static Dictionary<T, long> Dijkstra<T>(
            T initial, 
            Func<T, IEnumerable<(T Value, long Cost)>> step)
        {
            var q = new SortedDictionary<long, List<T>>();
            var tentative = new Dictionary<T, long>();
            q[0] = new List<T> { initial };

            while (q.Any())
            {
                var l = q.First();
                var next = l.Value.Last();
                l.Value.RemoveAt(l.Value.Count - 1);
                if (l.Value.Count == 0)
                {
                    q.Remove(l.Key);
                }
                
                foreach (var n in step(next))
                {
                    var oldWeight = tentative.GetWithDefault(n.Value, long.MaxValue);
                    var newWeight = l.Key + n.Cost;

                    if (newWeight < oldWeight)
                    {
                        tentative[n.Value] = newWeight;
                        if (q.TryGetValue(oldWeight, out var s) && s.Contains(n.Value))
                        {
                            s.Remove(n.Value);
                            if (s.Count == 0)
                            {
                                q.Remove(oldWeight);
                            }
                        }

                        if (!q.TryGetValue(newWeight, out s))
                        {
                            s = new List<T>();
                            q[newWeight] = s;
                        }
                        s.Add(n.Value);
                    }
                }
            }

            return tentative;
        }

        public static List<T> Dfs<T>(T initial, Func<T, IEnumerable<T>> step, Func<T, bool> done)
        {
            var stack = new Stack<Link<T>>();
            stack.Push(new Link<T>(initial, null));
            var visited = new HashSet<T>();
            while (stack.Count > 0)
            {
                var next = stack.Pop();
                if (done(next.Current))
                {
                    var res = new List<T>();
                    for (var p = next; p != null; p = p.Previous)
                    {
                        res.Add(p.Current);
                    }
                    res.Reverse();
                    return res;
                }
                foreach (var e in step(next.Current))
                {
                    if (!visited.Contains(e))
                    {
                        visited.Add(e);
                        stack.Push(new Link<T>(e, next));
                    }
                }
            }

            return null;
        }

        public static TValue GetWithDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                return defaultValue;
            }

            return value;
        }
    }
}
