using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2025
{
    public class Day11 : DayBase
    {
        public Day11() : base(11)
        {
        }

        private record Link(string Key, List<string> Children);

        public override void Solve()
        {
            var dict = GetInput();
            Console.WriteLine(CountPaths(dict, "out", "you", new HashSet<string>()));
        }

        private Dictionary<string, Link> GetInput()
        {
            var dict = new Dictionary<string, Link>();

            foreach (var line in GetInputLines())
            {
                var idx = line.IndexOf(':');
                var key = line.Substring(0, idx);
                var children = line.Substring(idx + 2).Split(' ').ToList();
                dict[key] = new Link(key, children);
            }
            return dict;
        }

        private long CountPaths(Dictionary<string, Link> dict, string from, string to, HashSet<string> exclude)
        {
            var known = exclude.ToDictionary(k => k, k => 0L);
            known[from] = 1L;
            return CountPathsImpl(to);

            long CountPathsImpl(string to)
            {
                if (known.TryGetValue(to, out var res))
                {
                    return res;
                }

                foreach (var l in dict.GetWithDefault(to, new Link("", new List<string>())).Children)
                {
                    res += CountPathsImpl(l);
                }
                known[to] = res;
                return res;
            }
        }

        public override void SolveMain()
        {
            var dict = GetInput();
            var f2d = CountPaths(dict,"dac",  "fft", Exclude("svr", "out"));
            var d2f = CountPaths(dict,"fft",  "dac", Exclude("svr", "out"));
            var s2f = CountPaths(dict,"fft",  "svr", Exclude("out", "dac"));
            var s2d = CountPaths(dict,"dac",  "svr", Exclude("fft", "out"));
            var d2o = CountPaths(dict,"out",  "dac", Exclude("svr", "fft"));
            var f2o = CountPaths(dict,"out",  "fft", Exclude("svr", "dac"));

            Console.WriteLine(s2f * f2d * d2o + s2d * d2f * f2o);

            HashSet<string> Exclude(params string[] exclude) => exclude.ToHashSet();
        }
    }
}
