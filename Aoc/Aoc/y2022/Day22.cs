using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day22 : DayBase
    {
        private record Instruction(int Move, int Turn);

        private (int X, int Y)[] facing = new (int, int)[]
        {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1)
        };

        private record Position(int X, int Y, int Facing);

        private (Grid<char> Grid, List<Instruction> Instruction, Position Position) GetInput()
        {
            var lines = GetInputLines(false);
            var grid = Grid<char>.FromLines(
                new[] { string.Empty }.Concat(
                lines
                .TakeWhile(l => l != string.Empty)
                .Select(l => $" {l} ")
                ).Concat(new[] { string.Empty })
                .ToList(), c => c);
            foreach (var idx in grid.Indexes())
            {
                if (grid[idx.X, idx.Y] == 0)
                {
                    grid[idx.X, idx.Y] = ' ';
                }
            }
            var instructions = new List<Instruction>();
            var i = 0;
            var p = 0;
            var s = lines.Last();
            while (i < s.Length)
            {
                if (!char.IsDigit(s[i]))
                {
                    instructions.Add(new Instruction(int.Parse(s.Substring(p, i - p)), s[i] == 'L' ? -1 : 1));
                    p = i + 1;
                }
                ++i;
            }
            instructions.Add(new Instruction(int.Parse(s.Substring(p, i - p)), 0));

            var x = 0;
            var y = 1;

            foreach (var idx in grid.Row(y).Indexes())
            {
                if (grid[idx.X, idx.Y] == '.')
                {
                    x = idx.X;
                    break;
                }
            }
            return (grid, instructions, new Position(x, y, 0));
        }

        public Day22() : base(22)
        {
        }

        private (Position New, bool DidMove) MoveOne(Position pos, Grid<char> grid)
        {
            var f = facing[pos.Facing];
            var c = grid[pos.X + f.X, pos.Y + f.Y];

            int n = 1;
            if (c == ' ')
            {
                for (n = 0; grid[pos.X - n * f.X, pos.Y - n * f.Y] != ' '; ++n)
                {
                }
                n = -n + 1;
                c = grid[pos.X + n * f.X, pos.Y + n * f.Y];
            }

            if (c == '#')
            {
                return (pos, false);
            }
            if (c == '.')
            {
                return (pos with { X = pos.X + n * f.X, Y = pos.Y + n * f.Y }, true);
            }

            throw new InvalidOperationException("???");
        }

        private Position Move(Position pos, Instruction instruction, Grid<char> grid)
        {
            var didMove = true;
            for (int i = 0; i < instruction.Move && didMove; ++i)
            {
                (pos, didMove) = MoveOne(pos, grid);
            }

            var f = (pos.Facing + instruction.Turn) % facing.Length;
            if (f < 0)
            {
                f += facing.Length;
            }

            return pos with { Facing = f };
        }

        public override void Solve()
        {
            var (grid, instructions, position) = GetInput();

            foreach (var instruction in instructions)
            {
                position = Move(position, instruction, grid);
            }
            var v = 1000 * position.Y + 4 * position.X + position.Facing;
            Console.WriteLine(v);
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
