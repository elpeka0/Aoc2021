using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoc
{
    public class IntCodeInterpreter
    {
        private const int Add = 1;
        private const int Mul = 2;
        private const int In = 3;
        private const int Out = 4;
        private const int JTrue = 5;
        private const int JFalse = 6;
        private const int Lt = 7;
        private const int Eq = 8;
        private const int Rel = 9;
        private const int Halt = 99;

        private long rbase;
        private readonly Dictionary<long, long> memory;
        private readonly Dictionary<long, long> original;
        private readonly Func<long> input;
        private readonly Action<long> output;

        public IntCodeInterpreter(string line, Func<long> input, Action<long> output)
        {
            this.input = input;
            this.output = output;
            this.memory = line.Split(',').Select((i, l) => (V: long.Parse(i), K: (long)l)).ToDictionary(t => t.K, t => t.V);
            this.original = this.memory.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public long this[long index]
        {
            get => this.memory.TryGetValue(index, out var v) ? v : 0;
            set => this.memory[index] = value;
        }

        public long Get(long op, int c, int ip)
        {
            var v = 100;
            for (int i = 0; i < c; i++)
            {
                v *= 10;
            }

            var m = (op / v) % 10;

            var f = this[ip + c + 1];
            return m switch
            {
                0 => this[f],
                1 => f,
                _ => this[this.rbase + f]
            };
        }
        public void Set(long op, int c, int ip, long value)
        {
            var v = 100;
            for (int i = 0; i < c; i++)
            {
                v *= 10;
            }

            var m = (op / v) % 10;

            var f = this[ip + c + 1];
            switch(m)
            {
                case 0: 
                    this[f] = value; 
                    break;
                default:
                    this[this.rbase + f] = value;
                    break;
            };
        }

        public static Func<long> MakeInput(params long[] e)
        {
            var n = e.Cast<long>().GetEnumerator();
            return () => 
            { 
                n.MoveNext();
                return n.Current;
            };
        }

        public static Action<long> MakeBatch(int n, Action<List<long>> callback)
        {
            var l = new List<long>();
            return v => 
            { 
                l.Add(v);
                if (l.Count == n)
                {
                    callback(l);
                    l.Clear();
                }
            };
        }

        public void Run()
        {
            var ip = 0;
            while (this.memory[ip] != Halt)
            {
                var i = this.memory[ip];
                switch (i % 100)
                {
                    case Add:
                        Set(i, 2, ip, Get(i, 0, ip) + Get(i, 1, ip));
                        ip += 4;
                        break;
                    case Mul:
                        Set(i, 2, ip, Get(i, 0, ip) * Get(i, 1, ip));
                        ip += 4;
                        break;
                    case In:
                        Set(i, 0, ip, this.input());
                        ip += 2;
                        break;
                    case Out:
                        this.output(Get(i, 0, ip));
                        ip += 2;
                        break;
                    case JTrue:
                        if (Get(i, 0, ip) != 0)
                        {
                            ip = (int) Get(i, 1, ip);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case JFalse:
                        if (Get(i, 0, ip) == 0)
                        {
                            ip = (int)Get(i, 1, ip);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case Lt:
                        Set(i, 2, ip, Get(i, 0, ip) < Get(i, 1, ip) ? 1 : 0);
                        ip += 4;
                        break;
                    case Eq:
                        Set(i, 2, ip, Get(i, 0, ip) == Get(i, 1, ip) ? 1 : 0);
                        ip += 4;
                        break;
                    case Rel:
                        this.rbase += Get(i, 0, ip);
                        ip += 2;
                        break;
                    case Halt:
                        break;
                    default:
                        throw new InvalidOperationException("Unknown op code");
                }
            }
        }

        public void Reset()
        {
            this.memory.Clear();
            foreach (var kv in this.original)
            {
                this.memory[kv.Key] = kv.Value;
            }
        }
    }
}
