using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day17 : DayBase
    {
        private IEnumerable<char> GetInput() => GetInputLines(false).First();

        private class Field
        {
            private readonly List<char[]> data = new ();

            public Field()
            {
                data.Add(new char[9]);
                Array.Fill(data[0], '#');
            }

            public int Top { get; set; } = 1;

            public char this[Vector p]
            {
                get => this.data[-p.Y][p.X];
                set => this.data[-p.Y][p.X] = value;
            }

            public void Extend(int count)
            {
                while (Top + count > this.data.Count)
                {
                    this.data.Add(new char[9]);
                    Array.Fill(this.data[^1], '.');
                    this.data[^1][0] = '#';
                    this.data[^1][8] = '#';
                }
            }

            public string Visualize(Grid<char> shape, Vector pos)
            {
                var copy = this.data.Select(l => l.ToArray()).ToList();

                if (shape != null)
                {
                    foreach (var p in shape.Indexes())
                    {
                        copy[pos.Y - p.Y][pos.X + p.X] = shape[p] == '#' ? '@' : '.';
                    }
                }

                return string.Join(Environment.NewLine, copy.Select(l => new string(l)).Reverse());
            }
        }

        public Day17() : base(17)
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

        private bool Overlaps(Field field, Grid<char> shape, Vector offset)
        {
            foreach (var p in shape.Indexes())
            {
                if (field[p + offset] == '#' && shape[p] == '#')
                {
                    return true;
                }
            }

            return false;
        }

        public override void Solve()
        {
            var tetris = GetTetris();
            var wind = GetInput().ToList();
            var widx = 0;
            var field = new Field();

            for (var i = 0; i < 2022; ++i)
            {
                var shape = tetris[i % tetris.Length];
                
                field.Extend(10);
                var pos = new Vector(3, -field.Top - 3 - shape.Height + 1);

                while (true)
                {
                    var gust = wind[widx % wind.Count];
                    ++widx;
                    var offx = gust == '<' ? -1 : 1;

                    var movep = pos + new Vector(offx, 0);
                    if (!Overlaps(field, shape, movep))
                    {
                        pos = movep;
                    }
                    
                    movep = pos + new Vector(0, 1);
                    if (Overlaps(field, shape, movep))
                    {
                        break;
                    }
                    pos = movep;
                }
                foreach (var p in shape.Indexes())
                {
                    if (shape[p] != '.')
                    {
                        field[pos + p] = shape[p];
                    }
                }
                field.Top = Math.Max(field.Top, -pos.Y + 1);
            }

            Console.WriteLine(field.Visualize(null, new Vector()));
            Console.WriteLine(field.Top - 1);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
