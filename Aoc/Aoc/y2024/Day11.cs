using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day11 : DayBase
    {
        public Day11() : base(11)
        {
        }

        private long BruteForce(long k, int n)
        {
            if (n == 0)
            {
                return 1;
            }
            else if (k == 0)
            {
                return BruteForce(1, n - 1);
            }
            else
            {
                var s = k.ToString();
                if (s.Length % 2 == 0)
                {
                    return BruteForce(long.Parse(s.Substring(0, s.Length / 2)), n - 1)
                           + BruteForce(long.Parse(s.Substring(s.Length / 2, s.Length / 2)), n - 1);
                }
                else
                {
                    return BruteForce(2024 * k, n - 1);
                }
            }
        }

        public override void Solve()
        {
            var parts = SplitInts(this.GetInputLines().First(), ' ').Select(i => (long)i).ToList();
            var sum = parts.Select(p => BruteForce(p, 25)).Sum();
            Console.WriteLine(sum);
        }

        public override void SolveMain()
        {
            var parts = SplitInts(this.GetInputLines().First(), ' ').Select(i => (long)i).ToList();

            var cache = new Dictionary<int, Dictionary<char, long>>();

            for (var i = 0; i <= 75; i++)
            {
                var d = new Dictionary<char, long>();
                cache[i] = d;
                if (i < 5)
                {
                    for (var c = '0'; c <= '9'; c++)
                    {
                        d[c] = BruteForce(c - '0', i);
                    }
                }
                else
                {
                    d['0'] = cache[i - 1]['1'];
                    d['1'] = FromCache(1 * 2024, i - 3);
                    d['2'] = FromCache(2 * 2024, i - 3);
                    d['3'] = FromCache(3 * 2024, i - 3);
                    d['4'] = FromCache(4 * 2024, i - 3);
                    d['5'] = FromCache(5 * 2024 * 2024, i - 5);
                    d['6'] = FromCache(6 * 2024 * 2024, i - 5);
                    d['7'] = FromCache(7 * 2024 * 2024, i - 5);
                    d['8'] = cache[i-5]['3'] + cache[i - 5]['2'] + cache[i - 5]['7'] + cache[i - 5]['7'] + cache[i - 5]['2'] + cache[i - 5]['6'] + cache[i - 4]['8'];
                    d['9'] = FromCache(9 * 2024 * 2024, i - 5);
                }
            }

            var sum = parts.Select(p => CachedBruteForce(p, 75)).Sum();
            Console.WriteLine(sum);

            long CachedBruteForce(long k, int n)
            {
                if (n == 0)
                {
                    return 1;
                }

                if (k > 9)
                {
                    var s = k.ToString();
                    if (s.Length % 2 == 0)
                    {
                        return CachedBruteForce(long.Parse(s.Substring(0, s.Length / 2)), n - 1)
                               + CachedBruteForce(long.Parse(s.Substring(s.Length / 2, s.Length / 2)), n - 1);
                    }
                    else
                    {
                        return CachedBruteForce(2024 * k, n - 1);
                    }
                }
                else
                {
                    return cache[n][(char)('0' + k)];
                }
            }

            long FromCache(long k, int n)
            {
                return k.ToString().Sum(c => cache[n][c]);
            }
        }
    }
}
