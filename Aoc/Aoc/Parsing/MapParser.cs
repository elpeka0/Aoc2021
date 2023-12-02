using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public class MapParser<TFrom, TTo> : IParser<TTo>
    {
        private readonly IParser<TFrom> _parser;
        private readonly Func<TFrom, TTo> _converter;

        public MapParser(IParser<TFrom> parser, Func<TFrom, TTo> converter)
        {
            _parser = parser;
            _converter = converter;
        }

        public Output<TTo> Parse(Input input)
        {
            if (_parser.Parse(input) is (var res, true))
            {
                return new Output<TTo>(_converter(res), true);
            }

            return new Output<TTo>(default, false);
        }
    }
}
