using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public class Input
    {
        public string Line { get; }

        public int Index { get; set; }

        public Input(string line)
        {
            Line = line;
            Index = 0;
        }

        public (char C, bool Ok) Next()
        {
            return Index < Line.Length
                ? (Line[Index++], true)
                : (' ', false);
        }
    }
}
