using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public abstract class DayBase
    {
        private readonly int day;

        protected DayBase(int day)
        {
            this.day = day;
        }
        protected IEnumerable<string> GetInputLines(bool mainPuzzle = false)
        {
            using var reader = this.OpenInput(mainPuzzle);
            while (reader.ReadLine() is { } line)
            {
                yield return line;
            }
        }

        protected StreamReader OpenInput(bool mainPuzzle = false)
        {
            var postfix = mainPuzzle ? "-main" : "";
            return File.OpenText($"input\\input{postfix}{this.day:00}.txt");
        }

        protected IEnumerable<int> SplitInts(string line, params char[] seperator)
        {
            return line.Split(seperator).Where(s => !string.IsNullOrEmpty(s)).Select(s => int.Parse(s));
        }

        public abstract void Solve();
        public abstract void SolveMain();
    }
}
