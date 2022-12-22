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

        private record Position(int X, int Y, int Facing, int Face);

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
            var instructions = ParseInstructions(lines);

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
            return (grid, instructions, new Position(x, y, 0, 0));
        }

        private static List<Instruction> ParseInstructions(IEnumerable<string> lines)
        {
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
            return instructions;
        }

        private (Dictionary<int, Grid<char>> Cube, List<Instruction> Instruction) GetInput2()
        {
            var lines = GetInputLines(false);
            var grid = Grid<char>.FromLines(
                lines
                .TakeWhile(l => l != string.Empty)
                .ToList(), c => c);

            var sl = 50;
            var cube = new Dictionary<int, Grid<char>>();
            var indexLoc = new (int X, int Y)[] { (sl, 0), (2*sl, 0), (sl, sl), (0, 3*sl), (0, 2*sl), (sl, 2*sl) };

            for (int i = 1; i <= 6; ++i)
            {
                var sg = new Grid<char>(sl, sl);
                foreach (var idx in sg.Indexes())
                {
                    sg[idx.X, idx.Y] = grid[idx.X + indexLoc[i - 1].X, idx.Y + indexLoc[i - 1].Y];
                }
                cube[i] = sg;
            }

            var instructions = ParseInstructions(lines);


            return (cube, instructions);
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

        private (Position New, bool DidMove) MoveOne2(Position pos, Dictionary<int, Grid<char>> cube)
        {
            var f = facing[pos.Facing];
            var grid = cube[pos.Face];

            var newPos = pos with { X = pos.X + f.X, Y = pos.Y + f.Y };

            if (newPos.X < 0)
            {
                var (face, facing) = pos.Face switch
                {
                    1 => (4, 3),
                    2 => (4, 2),
                    3 => (1, 3),
                    4 => (6, 3),
                    5 => (5, 0),
                    6 => (3, 3)
                };
            }


            var c = grid[newPos.X, newPos.Y];
            if (c == '#')
            {
                return (pos, false);
            }
            if (c == '.')
            {
                return (pos with { X = pos.X + f.X, Y = pos.Y + f.Y }, true);
            }

            throw new InvalidOperationException("???");
        }

        public override void SolveMain()
        {
            var (cube, instructions) = GetInput2();
            var position = new Position(0, 0, 0, 1);

            foreach (var face in cube)
            {
                Console.WriteLine("FACE " + face.Key);
                Console.WriteLine(face.Value);
                Console.WriteLine();
            }
        }
    }
}
