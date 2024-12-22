using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day22 : DayBase
    {
        public Day22() : base(22)
        {
        }

        private IReadOnlyList<long> Read() => GetInputLines().Select(long.Parse).ToList();

        private long Next(long value)
        {
            var m = 16777216;
            value = (value ^ (value << 6)) % m;
            value = (value ^ (value >> 5)) % m;
            value = (value ^ (value << 11)) % m;
            return value;
        }

        private long Apply(long value, int count)
        {
            for (var i = 0; i < count; i++)
            {
                value = Next(value);
            }
            return value;
        }

        private record Sequence(long A, long B, long C, long D)
        {
            public Sequence Shift(long v) => new Sequence(B, C, D, v);
        }

        private Dictionary<Sequence, int> BuildMap(long value, int count)
        {
            var raw = new Sequence(0, 0, 0, value);
            var diff = new Sequence(0, 0, 0, 0);
            var d = new Dictionary<Sequence, int>();
            for (var i = 0; i < count; ++i)
            {
                value = Next(value);
                raw = raw.Shift(value);
                diff = diff.Shift((raw.D % 10) - (raw.C % 10));
                if (i >= 3)
                {
                    if (!d.ContainsKey(diff))
                    {
                        d[diff] = (int)(raw.D % 10);
                    }
                }
            }
            return d;
        }

        public override void Solve()
        {
            var sum = Read().Select(v => Apply(v, 2000)).Sum();
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var map = new Dictionary<Sequence, int>();
            foreach (var i in Read())
            {
                var d = BuildMap(i, 2000);
                foreach (var kv in d)
                {
                    map.TryGetValue(kv.Key, out var v);
                    v += kv.Value;
                    map[kv.Key] = v;
                }
            }
            Console.WriteLine(map.Values.Max());
        }
    }
}
