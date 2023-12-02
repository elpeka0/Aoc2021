using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public class RepeatParser<T> : IParser<IEnumerable<T>>
    {
        private readonly IParser<T> _parser;
        private readonly int _min;
        private readonly int _max;

        public RepeatParser(IParser<T> parser, int min, int max)
        {
            _parser = parser;
            _min = min;
            _max = max;
        }

        public RepeatParser(IParser<T> parser)
            : this(parser, 0, int.MaxValue)
        {
        }

        public Output<IEnumerable<T>> Parse(Input input)
        {
            var pos = input.Index;
            var res = new List<T>();
            var n = 0;
            while (n < _max && _parser.Parse(input) is (var value, true))
            {
                res.Add(value);
                n++;
            }
            if (res.Count >= _min)
            {
                return new Output<IEnumerable<T>>(res, true);
            }

            input.Index = pos;
            return new Output<IEnumerable<T>>(null, false);
        }
    }
}
