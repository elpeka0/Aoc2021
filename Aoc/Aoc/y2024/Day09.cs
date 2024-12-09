using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2024
{
    public class Day09 : DayBase
    {
        public Day09() : base(9)
        {
        }

        private record Chunk(long Id, long Len, bool Free);

        public override void Solve()
        {
            var l = this.BuildChunks();

            var free = l.First!.Next;
            var todo = l.Last!;

            while (free != null)
            {
                var left = todo.Value.Len;
                while (free != null && free.Value.Free && left >= free.Value.Len)
                {
                    free.Value = free.Value with { Id = todo.Value.Id, Free = false };
                    left -= free.Value.Len;
                    if (free.Next == todo)
                    {
                        free = null;
                    }
                    else
                    {
                        free = free.Next!.Next;
                    }
                }

                if (left > 0 && free != null)
                {
                    var old = free.Value.Len;
                    free.Value = new(todo.Value.Id, left, false);
                    if (old > left)
                    {
                        free = l.AddAfter(free, new Chunk(0, old - left, true));
                    }

                    left = 0;
                }

                todo.Value = todo.Value with { Len = left };
                if (todo.Previous == free)
                {
                    break;
                }

                todo = todo.Previous!.Previous!;
            }

            var sum = ComputeSum(l);
            Console.WriteLine(sum);
        }

        private LinkedList<Chunk> BuildChunks()
        {
            var line = this.GetInputLines().First();
            var l = new LinkedList<Chunk>();

            for (var i = 0; i < line.Length; i++)
            {
                l.AddLast(new Chunk(i / 2, line[i] - '0', i % 2 != 0));
            }

            return l;
        }

        private static long ComputeSum(LinkedList<Chunk> l)
        {
            var sum = 0L;
            var pos = 0L;
            for (var p = l.First; p != null; p = p.Next)
            {
                var id = p.Value.Id;
                if (!p.Value.Free)
                {
                    sum += id * (pos * p.Value.Len + p.Value.Len * (p.Value.Len - 1) / 2);
                }

                pos += p.Value.Len;
            }

            return sum;
        }

        public override void SolveMain()
        {
            var l = this.BuildChunks();

            var todo = l.Last;
            var seen = new HashSet<long>();

            while (todo != null)
            {
                var available = l
                    .EnumerateNodes()
                    .TakeWhile(f => f != todo)
                    .FirstOrDefault(f => f.Value.Free && f.Value.Len >= todo.Value.Len);
                if (available != null)
                {
                    var old = available.Value.Len;
                    available.Value = available.Value with { Id = todo.Value.Id, Free = false, Len = todo.Value.Len };
                    l.AddAfter(available, new Chunk(available.Value.Id, old - todo.Value.Len, true));
                    todo.Value = todo.Value with { Free = true };
                }

                seen.Add(todo.Value.Id);
                while (todo != null && (todo.Value.Free || seen.Contains(todo.Value.Id)))
                {
                    todo = todo.Previous;
                }
            }

            //foreach (var n in l)
            //{
            //    for (var i = 0; i < n.Len; i++)
            //    {
            //        if (n.Free)
            //        {
            //            Console.Write('.');
            //        }
            //        else
            //        {
            //            Console.Write(n.Id);
            //        }
            //    }
            //}
            //Console.WriteLine();

            var sum = ComputeSum(l);
            Console.WriteLine(sum);
        }
    }
}
