using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public class ChoiceParser<T> : IParser<T>
    {
        private readonly IParser<T> _left;
        private readonly IParser<T> _right;

        public ChoiceParser(IParser<T> left, IParser<T> right)
        {
            _left = left;
            _right = right;
        }

        public Output<T> Parse(Input input)
        {
            if (_left.Parse(input) is (var res, true))
            {
                return new Output<T>(res, true);
            }

            return _right.Parse(input);
        }
    }
}
