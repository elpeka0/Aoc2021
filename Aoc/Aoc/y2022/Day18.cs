using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day18 : DayBase
    {
        private IEnumerable<char> GetInput() => GetInputLines(false).First();

        public Day18() : base(18)
        {
        }

        private Grid<char>[] GetTetris()
        {
            var tetris = new Grid<char>[5];
            tetris[0] = new Grid<char>(4, 1);
            tetris[0].Fill('#');
            tetris[1] = new Grid<char>(3, 3);
            tetris[1].Fill('#');
            tetris[1][0, 0] = '.';
            tetris[1][0, 2] = '.';
            tetris[1][2, 0] = '.';
            tetris[1][2, 2] = '.';
            tetris[2] = new Grid<char>(3, 3);
            tetris[2].Fill('#');
            tetris[2][0, 0] = '.';
            tetris[2][0, 1] = '.';
            tetris[2][1, 0] = '.';
            tetris[2][1, 1] = '.';
            tetris[3] = new Grid<char>(1, 4);
            tetris[3].Fill('#');
            tetris[4] = new Grid<char>(2, 2);
            tetris[4].Fill('#');
            return tetris;
        }

        private string Visualize(List<char[]> field, Grid<char> shape, int x, int y)
        {
            var copy = field.Select(l => l.ToArray()).ToList();

            if (shape != null)
            {
                foreach (var p in shape.Indexes())
                {
                    copy[y - p.Y][x + p.X] = shape[p.X, p.Y] == '#' ? '@' : '.';
                }
            }
            
            return string.Join(Environment.NewLine, copy.Select(l => new string(l)).Reverse());
        }

        public override void Solve()
        {
            var field = new List<char[]>();
            field.Add(new char[9]);
            Array.Fill(field[0], '#');
            var tetris = GetTetris();
            var wind = GetInput().ToList();
            var widx = 0;
            var top = 1;

            for (var i = 0; i < 2022; ++i)
            {
                var shape = tetris[i % tetris.Length];

                while (top + 10 > field.Count)
                {
                    field.Add(new char[9]);
                    Array.Fill(field[field.Count - 1], '.');
                    field[field.Count - 1][0] = '#';
                    field[field.Count - 1][8] = '#';
                }

                var x = 3;
                var y = top + 3 + shape.Height - 1;

                while (true)
                {
                    var gust = wind[widx % wind.Count];
                    ++widx;
                    var conflict = false;
                    var offx = gust == '<' ? -1 : 1;
                    foreach (var p in shape.Column(0).Indexes())
                    {
                        var cx = x + p.X + offx;
                        var cy = y - p.Y;
                        if (field[cy][cx] == '#')
                        {
                            conflict = true;
                        }
                    }
                    foreach (var p in shape.Column(shape.Width - 1).Indexes())
                    {
                        var cx = x + p.X + offx;
                        var cy = y - p.Y;
                        if (field[cy][cx] == '#')
                        {
                            conflict = true;
                        }
                    }
                    if (!conflict)
                    {
                        x += offx;
                    }
                    conflict = false;
                    foreach (var p in shape.Row(shape.Height - 1).Indexes())
                    {
                        var cx = x + p.X;
                        var cy = y - p.Y - 1;
                        if (field[cy][cx] == '#')
                        {
                            conflict = true;
                        }
                    }
                    if (conflict)
                    {
                        break;
                    }
                    --y;
                }
                foreach (var p in shape.Indexes())
                {
                    field[y - p.Y][x + p.X] = shape[p.X, p.Y];
                }
                top = Math.Max(top, y + 1);
            }
            Console.WriteLine(top - 1);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
