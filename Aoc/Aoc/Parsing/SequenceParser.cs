using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public class SequenceParser<TFirst, TSecond> : IParser<(TFirst, TSecond)>
    {
        private readonly IParser<TFirst> _first;
        private readonly IParser<TSecond> _second;

        public SequenceParser(IParser<TFirst> first, IParser<TSecond> second)
        {
            _first = first;
            _second = second;
        }

        public Output<(TFirst, TSecond)> Parse(Input input)
        {
            var pos = input.Index;

            if (_first.Parse(input) is (var first, true)
                && _second.Parse(input) is (var second, true))
            {
                return new Output<(TFirst, TSecond)>((first, second), true);
            }

            input.Index = pos;
            return new Output<(TFirst, TSecond)>((default, default), false);
        }
    }
}
