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
        private const int Halt = 99;

        private readonly List<long> memory;
        private readonly List<long> original;
        private readonly Func<long> input;
        private readonly Action<long> output;

        public IntCodeInterpreter(string line, Func<long> input, Action<long> output)
        {
            this.input = input;
            this.output = output;
            this.memory = line.Split(',').Select(i => long.Parse(i)).ToList();
            this.original = this.memory.ToList();
        }

        public long this[long index]
        {
            get => this.memory[(int)index];
            set => this.memory[(int)index] = value;
        }

        public long Get(long op, int c, int ip)
        {
            var v = 100;
            for (int i = 0; i < c; i++)
            {
                v *= 10;
            }

            var m = (op / v) % 10;
            if (m == 0)
            {
                return this[this[ip + c + 1]];
            }
            else
            {
                return this[ip + c + 1];
            }
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

        public record Channel(Func<long> Receive, Action<long> Send);

        public static Channel MakeChannel(int nr, bool debug)
        {
            var signal = new object();
            var values = new Queue<long>();

            return new(() =>
                {
                    lock (signal)
                    {
                        while (!values.Any())
                        {
                            Monitor.Wait(signal);
                        }

                        return values.Dequeue();
                    }
                },
                v =>
                {
                    lock (signal)
                    {
                        values.Enqueue(v);
                        if (debug)
                        {
                            Console.WriteLine($"C{nr} -> {v}");
                        }
                        Monitor.Pulse(signal);
                    }
                });
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
                        this[this[ip + 3]] = Get(i, 0, ip) + Get(i, 1, ip);
                        ip += 4;
                        break;
                    case Mul:
                        this[this[ip + 3]] = Get(i, 0, ip) * Get(i, 1, ip);
                        ip += 4;
                        break;
                    case In:
                        this[this[ip + 1]] = this.input();
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
                        this[this[ip + 3]] = Get(i, 0, ip) < Get(i, 1, ip) ? 1 : 0;
                        ip += 4;
                        break;
                    case Eq:
                        this[this[ip + 3]] = Get(i, 0, ip) == Get(i, 1, ip) ? 1 : 0;
                        ip += 4;
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
            this.memory.AddRange(this.original);
        }
    }
}
