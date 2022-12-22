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

        private (Grid<char> Grid, List<Instruction> Instruction, Position Position) GetInput(bool pad)
        {
            var padding = pad ? new[] { string.Empty } : new string[0];
            var lines = GetInputLines(false);
            var grid = Grid<char>.FromLines(
                padding.Concat(
                lines
                .TakeWhile(l => l != string.Empty)
                .Select(l => pad ? $" {l} " : l)
                ).Concat(padding)
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
            var y = pad ? 1 : 0;

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
            var (grid, instructions, position) = GetInput(true);

            foreach (var instruction in instructions)
            {
                position = Move(position, instruction, grid);
            }
            var v = 1000 * position.Y + 4 * position.X + position.Facing;
            Console.WriteLine(v);
        }

        private record Rule(int X, int Y, Vector XM, Vector YM, Vector ZM, Vector A);

        private IEnumerable<Rule> Sample()
        {
            yield return new Rule(2, 0, new Vector(1, 0, 0), new Vector(0, -1, 0), new Vector(0, 0, 0), new Vector(0, 1, 0));
            yield return new Rule(0, 1, new Vector(-1, 0, 0), new Vector(0, 0, 0), new Vector(0, 1, 0), new Vector(1, 1, 0));
            yield return new Rule(1, 1, new Vector(0, 0, 0), new Vector(-1, 0, 0), new Vector(0, 1, 0), new Vector(0, 1, 0));
            yield return new Rule(2, 1, new Vector(1, 0, 0), new Vector(0, 0, 0), new Vector(0, 1, 0), new Vector(0, 0, 0));
            yield return new Rule(2, 2, new Vector(1, 0, 0), new Vector(0, 1, 0), new Vector(0, 0, 0), new Vector(0, 1, 1));
            yield return new Rule(3, 2, new Vector(0, 0, 0), new Vector(0, 1, 0), new Vector(-1, 0, 0), new Vector(1, 0, 1));
        }
        private IEnumerable<Rule> Actual()
        {
            yield break;
        }

        private record CubeCell(char C, List<(int X, int Y)> Coords)
        {
            public override string ToString()
            {
                var c = string.Join(':', Coords.Select(c => c.X.ToString().PadRight(2) + "," + c.Y.ToString().PadRight(2)));
                c = c.PadRight(17);
                return $"[{C} {c}]";
            }
        }

        private Grid<CubeCell> Cubify(Grid<char> grid)
        {
            var len = (int) Math.Sqrt(grid.Count(c => c != ' ') / 6);
            var res = new Grid<CubeCell>(grid.Width, grid.Height);
            res.Fill(new CubeCell(' ', new List<(int X, int Y)>()));
            var rules = len == 4 ? Sample() : Actual();

            var map = new Dictionary<(int, int, int), List<(int, int)>>();

            foreach (var rule in rules)
            {
                var px = rule.X * len;
                var py = rule.Y * len;
                for (var x = 0; x < len; x++)
                {
                    for (var y = 0; y < len; y++)
                    {
                        int Mul(Vector m, int a) => x * m.X + y * m.Y + a * (len - 1);
                        var tx = Mul(rule.XM, rule.A.X);
                        var ty = Mul(rule.YM, rule.A.Y);
                        var tz = Mul(rule.ZM, rule.A.Z);

                        var c = grid[px + x, py + y];
                        if (!map.TryGetValue((tx, ty, tz), out var l))
                        {
                            l = new List<(int, int)>();
                            map[(tx, ty, tz)] = l;
                        }
                        l.Add((px + x, py + y));
                        res[px + x, py + y] = new CubeCell(c, l);
                    }
                }
            }

            return res;
        }

        private bool IsEmpty(int x, int y, Grid<CubeCell> grid) =>
            x < 0 || x >= grid.Width || y < 0 || y >= grid.Height || grid[x, y].C == ' ';

        private (Position New, bool DidMove) MoveOne2(Position pos, Grid<CubeCell> grid)
        {
            var f = facing[pos.Facing];
            var newPos = pos with { X = pos.X + f.X, Y = pos.Y + f.Y };

            if (IsEmpty(newPos.X, newPos.Y, grid))
            {
                var coords = grid[pos.X, pos.Y].Coords;
                if (coords.Count == 1)
                {
                    throw new InvalidOperationException("Fuck!");
                }

                if (coords.Count == 3)
                {
                    throw new InvalidOperationException("Sigh!");
                }

                var nc = coords.First(x => x != (pos.X, pos.Y));
                newPos = pos with { X = nc.X, Y = nc.Y };

                int t;
                for (t = 0; t < facing.Length; ++t)
                {
                    if (IsEmpty(newPos.X + facing[t].X, newPos.Y + facing[t].Y, grid))
                    {
                        newPos = newPos with { Facing = t switch
                        {
                            0 => 2,
                            1 => 3,
                            2 => 0,
                            3 => 1
                        }};
                        break;
                    }
                }
            }

            var c = grid[newPos.X, newPos.Y];

            if (c.C == '#')
            {
                return (pos, false);
            }
            if (c.C == '.')
            {
                return (newPos, true);
            }

            throw new InvalidOperationException("???");
        }

        private Position Move2(Position pos, Instruction instruction, Grid<CubeCell> grid)
        {
            var didMove = true;
            for (int i = 0; i < instruction.Move && didMove; ++i)
            {
                (pos, didMove) = MoveOne2(pos, grid);
            }

            var f = (pos.Facing + instruction.Turn) % facing.Length;
            if (f < 0)
            {
                f += facing.Length;
            }

            return pos with { Facing = f };
        }

        public override void SolveMain()
        {
            var (grid, instructions, position) = GetInput(false);
            var cube = Cubify(grid);

            foreach (var instruction in instructions)
            {
                position = Move2(position, instruction, cube);
            }
            var v = 1000 * position.Y + 4 * position.X + position.Facing;
            Console.WriteLine(v);
        }
    }
}
