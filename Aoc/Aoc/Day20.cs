using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day20 : DayBase
    {
        public Day20() : base(20)
        {
        }

        private class InputData
        {
            public List<bool> PixelIndex { get; } = new List<bool>();
            public Dictionary<(int X, int Y), bool> Grid { get; private set; } = new Dictionary<(int, int), bool>();
            public Dictionary<(int X, int Y), bool> NextGrid { get; private set; } = new Dictionary<(int, int), bool>();

            public int MinX { get; set; } = int.MaxValue;
            public int MinY { get; set; } = int.MaxValue;
            public int MaxX { get; set; } = int.MinValue;
            public int MaxY { get; set; } = int.MinValue;
            public int NextMinX { get; set; } = int.MaxValue;
            public int NextMinY { get; set; } = int.MaxValue;
            public int NextMaxX { get; set; } = int.MinValue;
            public int NextMaxY { get; set; } = int.MinValue;

            public bool Default { get; private set; }

            public void Set(int x, int y, bool b)
            {
                if (x < NextMinX) NextMinX = x;
                if (x > NextMaxX) NextMaxX = x;
                if (y < NextMinY) NextMinY = y;
                if (y > NextMaxY) NextMaxY = y;
                NextGrid[(x, y)] = b;
            }

            public bool Get(int x, int y)
            {
                if (Grid.TryGetValue((x, y), out var v))
                {
                    return v;
                }
                else
                {
                    return Default;
                }
            }

            public void Flip()
            {
                MaxX = NextMaxX;
                MaxY = NextMaxY;
                MinX = NextMinX;
                MinY = NextMinY;
                Grid = NextGrid;
                NextGrid = new Dictionary<(int X, int Y), bool>();
            }

            public void Advance()
            {
                for (var x = MinX - 1; x <= MaxX + 1; ++x)
                {
                    for (var y = MinY - 1; y <= MaxY + 1; ++y)
                    {
                        var idx = 0;
                        for (var iy = -1; iy <= 1; ++iy)
                        {
                            for (var ix = -1; ix <= 1; ++ix)
                            {
                                idx <<= 1;
                                if (Get(x + ix, y + iy))
                                {
                                    ++idx;
                                }
                            }
                        }
                        
                        Set(x, y, PixelIndex[idx]);
                    }
                }

                this.Default = PixelIndex[this.Default ? 511 : 0];
                this.Flip();
            }
        }

        private InputData GetInput()
        {
            var res = new InputData();
            var lines = GetInputLines(false).ToList();
            var head = lines[0];
            foreach (var c in head)
            {
                res.PixelIndex.Add(c == '#');
            }
            
            for (var y = 2;y < lines.Count;++y)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; ++x)
                {
                    res.Set(x, y, line[x] == '#');
                }
            }

            res.Flip();

            return res;
        }

        public override void Solve()
        {
            var input = this.GetInput();
            input.Advance();
            input.Advance();
            Console.WriteLine(input.Grid.Count(kv => kv.Value));
        }

        public override void SolveMain()
        {
            var input = this.GetInput();
            for (var i = 0; i < 50; ++i)
            {
                input.Advance();
            }

            Console.WriteLine(input.Grid.Count(kv => kv.Value));
        }
    }
}
