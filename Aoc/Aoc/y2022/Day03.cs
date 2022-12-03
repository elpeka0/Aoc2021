using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day03 : DayBase
    {
        public Day03() : base(3)
        {
        }

        private List<(string, string)> GetInput()
        {
            return GetInputLines(false).Select(l => (l.Substring(0, l.Length/2), l.Substring(l.Length/2))).ToList();
        }
        private List<List<HashSet<char>>> GetInputMain()
        {
            var lines = GetInputLines(false).ToList();
            var res = new List<List<HashSet<char>>>();
            for (var i = 0; i < lines.Count; i += 3)
            {
                res.Add(new List<HashSet<char>>
                {
                    lines[i].ToHashSet(),
                    lines[i+1].ToHashSet(),
                    lines[i+2].ToHashSet()
                });
            }
            return res;
        }

        public override void Solve()
        {
            var res = 0;
            foreach (var (a, b) in GetInput())
            {
                var ha = a.ToHashSet();
                var hb = b.ToHashSet();
                var c = ha.Single(x => hb.Contains(x));
                res += Score(c);
            }
            Console.WriteLine(res);
        }

        private int Score(char c)
        {
            if (char.IsLower(c))
            {
                return 1 + c - 'a';
            }
            else
            {
                return 27 + c - 'A';
            }
        }

        public override void SolveMain()
        {
            var res = 0;
            foreach (var group in GetInputMain())
            {
                foreach (var c in group[0])
                {
                    if (group[1].Contains(c) && group[2].Contains(c))
                    {
                        res += Score(c);
                    }
                }
            }
            Console.WriteLine(res);
        }
    }
}
