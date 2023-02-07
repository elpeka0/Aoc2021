using System;
using System.Linq;
using Aoc.Geometry;

namespace Aoc.y2021
{
    public class Day25 : DayBase
    {
        public Day25() : base(25)
        {
        }

        private Grid<bool?> GetInput()
        {
            var lines = this.GetInputLines(false).ToList();
            var res = new Grid<bool?>(lines[0].Length, lines.Count);
            var y = 0;
            foreach(var line in lines)
            {
                var x = 0;
                foreach(var c in line)
                {
                    if(c != '.')
                    {
                        res[x, y] = c == '>';
                    }
                    ++x;
                }
                ++y;
            }
            return res;
        }

        private (Grid<bool?> Grid, bool Any) Right(Grid<bool?> grid)
        {
            var res = new Grid<bool?>(grid.Width, grid.Height);
            var any = false;
            foreach (var (x, y) in grid.Indexes())
            {
                if (grid[x, y] == true)
                {
                    var nx = (x + 1) % grid.Width;
                    if (grid[nx, y] == null)
                    {
                        res[nx, y] = true;
                        res[x, y] = null;
                        any = true;
                    }
                    else
                    {
                        res[x, y] = grid[x, y];
                    }
                }
                else if(grid[x, y] == false)
                {
                    res[x, y] = grid[x, y];
                }
            }
            return (res, any);
        }

        private (Grid<bool?> Grid, bool Any) Down(Grid<bool?> grid)
        {
            var res = new Grid<bool?>(grid.Width, grid.Height);
            var any = false;
            foreach (var (x, y) in grid.Indexes())
            {
                if (grid[x, y] == false)
                {
                    var ny = (y + 1) % grid.Height;
                    if (grid[x, ny] == null)
                    {
                        res[x, ny] = false;
                        res[x, y] = null;
                        any = true;
                    }
                    else
                    {
                        res[x, y] = grid[x, y];
                    }
                }
                else if(grid[x, y] == true)
                {
                    res[x, y] = grid[x, y];
                }
            }
            return (res, any);
        }

        public override void Solve()
        {
            var grid = this.GetInput();
            var turn = 0;
            var right = true;
            var down = true;
            while(right || down)
            {
                //Visualize(grid);
                //Console.WriteLine();
                ++turn;
                (grid, right) = this.Right(grid);
                (grid, down) = this.Down(grid);
            }
            Console.WriteLine(turn);
        }

        private void Visualize(Grid<bool?> grid)
        {
            for (var y = 0; y < grid.Height; ++y)
            {
                for (var x = 0; x < grid.Width; ++x)
                {
                    switch (grid[x, y])
                    {
                        case null:
                            Console.Write('.');
                            break;
                        case true:
                            Console.Write('>');
                            break;
                        case false:
                            Console.Write('v');
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
