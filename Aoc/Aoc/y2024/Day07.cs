using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day07 : DayBase
    {
        public Day07() : base(7)
        {
        }

        public override void Solve()
        {
            var total = 0L;
            foreach (var line in GetInputLines())
            {
                var parts = line.Split(':');
                var head = long.Parse(parts[0]);
                var rest = SplitInts(parts[1], ' ').ToList();

                var n = 1 << rest.Count;
                for (var mask = 0; mask < n; mask++)
                {
                    var check = (long) rest[0];
                    for (var i = 1; i < rest.Count; i++)
                    {
                        var bit = (mask & (1 << i)) > 0 ? true : false;

                        if (bit)
                        {
                            check *= rest[i];
                        }
                        else
                        {
                            check += rest[i];
                        }
                    }
                    if (check == head)
                    {
                        total += head;
                        break;
                    }
                }
            }
            Console.WriteLine(total);
        }

        public override void SolveMain()
        {
            var total = 0L;
            foreach (var line in GetInputLines())
            {
                var parts = line.Split(':');
                var head = long.Parse(parts[0]);
                var rest = SplitInts(parts[1], ' ').ToList();

                var n = 1;
                for (var e = 0; e < rest.Count; e++)
                {
                    n *= 3;
                }

                for (var mask = 0; mask < n; mask++)
                {
                    var check = (long)rest[0];
                    var test = mask;
                    for (var i = 1; i < rest.Count; i++)
                    {
                        var rem = test % 3;

                        if (rem == 0)
                        {
                            check *= rest[i];
                        }
                        else if (rem == 1)
                        {
                            check += rest[i];
                        }
                        else
                        {
                            check = long.Parse(check.ToString() + rest[i]);
                        }
                        test /= 3;
                    }
                    if (check == head)
                    {
                        total += head;
                        break;
                    }
                }
            }
            Console.WriteLine(total);
        }
    }
}
