using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class IntCodeInterpreter
    {
        private const int Add = 1;
        private const int Mul = 2;
        private const int Halt = 99;

        private readonly List<long> memory;
        private readonly List<long> original;

        public IntCodeInterpreter(string input)
        {
            this.memory = input.Split(',').Select(i => long.Parse(i)).ToList();
            this.original = this.memory.ToList();
        }

        public long this[long index]
        {
            get => this.memory[(int)index];
            set => this.memory[(int)index] = value;
        }

        public void Run()
        {
            var ip = 0;
            while (this.memory[ip] != Halt)
            {
                switch (this.memory[ip])
                {
                    case Add:
                        this[this[ip + 3]] = this[this[ip + 1]] + this[this[ip + 2]];
                        ip += 4;
                        break;
                    case Mul:
                        this[this[ip + 3]] = this[this[ip + 1]] * this[this[ip + 2]];
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
