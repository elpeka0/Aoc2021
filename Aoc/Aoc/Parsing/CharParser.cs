using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public class CharParser : IParser<char>
    {
        private readonly Func<char, bool> _decider;

        public CharParser(Func<char, bool> decider) 
        {
            _decider = decider;
        }

        public Output<char> Parse(Input input)
        {
            var pos = input.Index;
            if (input.Next() is (var c, true)
                && _decider(c))
            {
                return new Output<char>(c, true);
            }

            input.Index = pos;
            return new Output<char>(' ', false);
        }
    }
}
