using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day20 : DayBase
    {
        private class Node
        {
            public Node Prev { get; set; }
            public Node Next { get; set; }
            public long Value { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();
                var p = this;
                do
                {
                    sb.Append(p.Value);
                    sb.Append(" ");
                    p = p.Next;
                } while (p != this);
                return sb.ToString();
            }
        }

        private Node Remove(Node node)
        {
            (node.Prev.Next, node.Next.Prev) = (node.Next, node.Prev);
            return node.Prev;
        }

        private Node AddAfter(Node prev, long value)
        {
            if (prev == null)
            {
                var node = new Node
                {
                    Value = value
                };
                node.Prev = node.Next = node;
                return node;
            }
            else
            {
                var node = new Node
                {
                    Prev = prev,
                    Next = prev.Next,
                    Value = value
                };
                (prev.Next, prev.Next.Prev) = (node, node);
                return node;
            }
        }

        private Node AddAfter(Node prev, Node node)
        {
            (node.Next, node.Prev) = (prev.Next, prev);
            (prev.Next, prev.Next.Prev) = (node, node);
            return node;
        }

        private Node Advance(Node p, long n)
        {
            for (var i = 0; i != n; i += Math.Sign(n))
            {
                p = n > 0 ? p.Next : p.Prev;
            }
            return p;
        }

        private Node Find(Node p, int value)
        {
            while (p.Value != value)
            {
                p = p.Next;
            }
            return p;
        }

        public Day20() : base(20)
        {
        }

        private List<Node> GetInput()
        {
            Node p = null;
            var l = new List<Node>();
            foreach (var n in GetInputLines(false).Select(int.Parse))
            {
                p = AddAfter(p, n);
                l.Add(p);
            }
            return l;
        }

        private void Shuffle(List<Node> order)
        {
            foreach (var n in order)
            {
                var cnt = n.Value % (order.Count - 1);
                var p = Remove(n);
                p = Advance(p, cnt);
                AddAfter(p, n);
            }
        }

        private long FindAnswer(Node p)
        {
            p = Find(p, 0);
            var res = 0L;
            p = Advance(p, 1000);
            res += p.Value;
            p = Advance(p, 1000);
            res += p.Value;
            p = Advance(p, 1000);
            res += p.Value;
            return res;
        }

        public override void Solve()
        {
            var l = GetInput();
            Shuffle(l);
            Console.WriteLine(FindAnswer(l[0]));
        }

        public override void SolveMain()
        {
            var l = GetInput();
            foreach (var n in l)
            {
                n.Value *= 811589153;
            }
            for (int i = 0; i < 10; i++)
            {
                Shuffle(l);
            }
            Console.WriteLine(FindAnswer(l[0]));
        }
    }
}
