﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public interface IParser<T>
    {
        Output<T> Parse(Input input);
    }
}
