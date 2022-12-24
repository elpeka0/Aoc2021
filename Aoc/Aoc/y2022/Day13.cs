using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day13 : DayBase
    {

        public Day13() : base(13)
        {
        }

        private object Parse(IEnumerator<char> p)
        {
            if (p.Current == '[')
            {
                p.MoveNext();
                var l = new List<object>();
                if (p.Current == ']')
                {
                    p.MoveNext();
                    return l;
                }
                l.Add(Parse(p));

                while (p.Current == ',')
                {
                    p.MoveNext();
                    l.Add(Parse(p));
                }
                p.MoveNext();
                return l;
            }

            var sb = new StringBuilder();
            while (char.IsDigit(p.Current))
            {
                sb.Append(p.Current);
                p.MoveNext();
            }

            return int.Parse(sb.ToString());
        }

        private static int Compare(object a, object b)
        {
            var ai = a as int?;
            var bi = b as int?;

            if (ai != null && bi != null)
            {
                return Math.Sign(ai.Value - bi.Value);
            }
            
            if (ai == null && bi == null)
            {
                var la = (List<object>)a;
                var lb = (List<object>)b;

                for (var i = 0; i < Math.Min(la.Count, lb.Count); i++)
                {
                    var c = Compare(la[i], lb[i]);
                    if (c != 0)
                    {
                        return c;
                    }
                }

                return Math.Sign(la.Count - lb.Count);
            }

            if (ai != null)
            {
                return Compare(new List<object> { ai.Value }, b);
            }

            return Compare(a, new List<object> { bi.Value });
        }

        private IEnumerable<(object A, object B, int Index)> GetInput()
        {
            var lines = GetInputLines(false).ToList();
            for (var i = 0; i < lines.Count; i += 3)
            {
                var a = lines[i].GetEnumerator();
                var b = lines[i + 1].GetEnumerator();
                a.MoveNext();
                b.MoveNext();
                yield return (Parse(a), Parse(b), i/3 + 1);
            }
        }

        private object ParseString(string s)
        {
            var e = s.GetEnumerator();
            e.MoveNext();
            return Parse(e);
        }

        private class PacketComparer : IComparer<object>
        {
            public int Compare(object x, object y)
            {
                return Day13.Compare(x, y);
            }
        }

        public override void Solve()
        {
            var res = this.GetInput().Where(t => Compare(t.A, t.B) == -1).Sum(t => t.Index);
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var all = new SortedList<object, object>(new PacketComparer());
            foreach (var x in this.GetInput().SelectMany(t => new[] { t.A, t.B }))
            {
                all[x] = null;
            }

            var two = ParseString("[[2]]");
            var six = ParseString("[[6]]");
            all.Add(two, null);
            all.Add(six, null);

            var a = all.IndexOfKey(two) + 1;
            var b = all.IndexOfKey(six) + 1;
            Console.WriteLine(a * b);
        }
    }
}
