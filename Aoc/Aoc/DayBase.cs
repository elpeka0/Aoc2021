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
        protected IEnumerable<string> GetInputLines(bool mainPuzzle)
        {
            var postfix = mainPuzzle ? "-main" : "";
            using var reader = File.OpenText($"input\\input{postfix}{this.day}.txt");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        protected IEnumerable<int> SplitInts(string line, params char[] seperator)
        {
            return line.Split(seperator).Where(s => !string.IsNullOrEmpty(s)).Select(s => int.Parse(s));
        }

        public abstract void Solve();
        public abstract void SolveMain();
    }
}
