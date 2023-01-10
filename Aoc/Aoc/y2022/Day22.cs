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

        private Vector[] facing = new Vector[]
        {
            new Vector(1, 0),
            new Vector(0, 1),
            new Vector(-1, 0),
            new Vector(0, -1)
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

        private record Rule(Vector P, TransformMatrix M)
        {
            public override string ToString()
            {
                return $"{P}\r\n\r\n{M}\r\n";
            }
        }

        private IEnumerable<Rule> Sample(int len)
        {
            yield return new Rule(new Vector(2 * len, 0), new TransformMatrix(
                1, 0, 0, 0,
                0, 0, 0, len - 1,
                0, -1, 0, len - 1));

            yield return new Rule(new Vector(0, len), new TransformMatrix(
                -1, 0, 0, len - 1,
                0, -1, 0, len - 1,
                0, 0, 0, len - 1));

            yield return new Rule(new Vector(len, len), new TransformMatrix(
                0, 0, 0, 0,
                0, -1, 0, len - 1,
                -1, 0, 0, len - 1));

            yield return new Rule(new Vector(2 * len, len), new TransformMatrix(
                1, 0, 0, 0,
                0, -1, 0, len - 1,
                0, 0, 0, 0));

            yield return new Rule(new Vector(2 * len, 2 * len), new TransformMatrix(
                1, 0, 0, 0, 
                0, 0, 0, 0,
                0, 1, 0, 0));

            yield return new Rule(new Vector(3 * len, 2 * len), new TransformMatrix(
                0, 0, 0, len - 1,
                1, 0, 0, 0,
                0, 1, 0, 0));
        }

        private record CubeCell(char C, List<Vector> Coords)
        {
            private static Dictionary<Vector, int> lookup = new Dictionary<Vector, int>();

            public override string ToString()
            {
                const int padding = 5;
                if (Coords.Count == 0)
                {
                    return string.Empty.PadRight(padding);
                }

                if (!lookup.ContainsKey(Coords[0]))
                {
                    var n = lookup.Count + 1;
                    foreach (var c in Coords)
                    {
                        lookup[c] = n;
                    }
                }
                return lookup[Coords[0]].ToString().PadRight(padding);
            }
        }

        private TransformMatrix Root(int len) => new TransformMatrix(
            1, 0, 0, 0,
            0, -1, 0, len - 1,
            0, 0, 1, 0);

        private IEnumerable<Rule> ComputeMatrix(Grid<char> grid, int len)
        {
            int x;
            var y = len;
            for (x = 0; x < grid.Width; x += len)
            {
                if (grid[x, y] != ' ')
                {
                    break;
                }
            }

            var seen = new HashSet<Vector>();
            return NextFace(
                new Vector(x, y), 
                TransformMatrix.Identity, 
                new TransformMatrix(
                    0, 1, 0, 0,
                    1, 0, 0, 0,
                    0, 0, 1, 0
                    ),
                TransformMatrix.Identity).ToList();

            IEnumerable<Rule> NextFace(Vector p, TransformMatrix transform, TransformMatrix bend, TransformMatrix main)
            {
                if (seen.Contains(p)
                    || p.X < 0
                    || p.X >= grid.Width
                    || p.Y < 0
                    || p.Y >= grid.Height
                    || grid[p.X, p.Y] == ' ')
                {
                    yield break;
                }
                seen.Add(p);

                yield return new Rule(p, transform * Root(len));

                foreach (var f in facing)
                {
                    var actual = new Vector(f.X, -f.Y);
                    var fulcrum = actual switch
                    {
                        (-1, 0) => new Vector(0, 0),
                        (0, -1) => new Vector(0, 0),
                        (0, 1) => new Vector(0, len - 1),
                        (1, 0) => new Vector(len - 1, 0)
                    };
                    fulcrum = transform * fulcrum;
                    var offset = main * (actual * (len - 1));
                    var rot = TransformMatrix.Rotate(fulcrum, bend * actual, 1);
                    var dir = TransformMatrix.Rotate(new Vector(), bend * actual, 1);
                    var counter = TransformMatrix.Rotate(new Vector(), -bend * actual, 1);
                    var m = rot * TransformMatrix.Translate(offset);

                    foreach (var r in NextFace(p + f * len, m * transform, counter * bend, dir * main).ToList())
                    {
                        yield return r;
                    }
                }
            }
        }

        private StringBuilder VisualizeTransform(TransformMatrix m, int len)
        {
            var sb = new StringBuilder();
            for (int row = 0; row < len; ++row)
            {
                for (int column = 0; column < len; ++column)
                {
                    sb.Append(m * new Vector(column, row));
                    sb.Append("  ");
                }
                sb.AppendLine();
            }
            return sb;
        }

        private Grid<CubeCell> Cubify(Grid<char> grid)
        {
            var len = (int) Math.Sqrt(grid.Count(c => c != ' ') / 6);
            var res = new Grid<CubeCell>(grid.Width, grid.Height);
            res.Fill(new CubeCell(' ', new List<Vector>()));
            var rules = ComputeMatrix(grid, len).ToList();
            Console.WriteLine(string.Join(Environment.NewLine, rules.Select(r => 
                r.ToString() + Environment.NewLine + Environment.NewLine + VisualizeTransform(r.M, len))));

            var map = new Dictionary<Vector, List<Vector>>();

            foreach (var rule in rules)
            {
                for (var x = 0; x < len; x++)
                {
                    for (var y = 0; y < len; y++)
                    {
                        var v = new Vector(x, y);
                        var t = rule.M * v;
                        var op = rule.P + v;
                        var c = grid[op.X, op.Y];
                        if (!map.TryGetValue(t, out var l))
                        {
                            l = new List<Vector>();
                            map[t] = l;
                        }
                        l.Add(op);
                        res[op.X, op.Y] = new CubeCell(c, l);
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

                List<Vector> candidates = coords.ToList();
                if (coords.Count == 3)
                {
                    candidates.Remove(new Vector(pos.X, pos.Y));
                    //throw new InvalidOperationException("Sigh!");
                }

                var nc = candidates.First(x => x != new Vector(pos.X, pos.Y));
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
            var v = 1000 * (position.Y + 1) + 4 * (position.X + 1) + position.Facing;
            Console.WriteLine(v);
        }
    }
}
