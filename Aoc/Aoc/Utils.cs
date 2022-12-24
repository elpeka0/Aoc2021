using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public static class Utils
    {
        public static int ExtractInt(string bullshit, bool signed)
        {
            return int.Parse(new string(bullshit.Where(c => char.IsDigit(c) || (c == '-' && signed)).ToArray()));
        }
    }
}
