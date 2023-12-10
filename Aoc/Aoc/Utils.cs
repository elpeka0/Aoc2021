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
