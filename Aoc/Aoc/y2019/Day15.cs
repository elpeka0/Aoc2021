using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoc.y2019
{
    internal class Day15 : DayBase
    {
        public Day15() : base(15)
        {
        }

        private void Visualize(Dictionary<Vector, char> field, HashSet<Vector> path)
        {
            var minx = field.Keys.Min(v => v.X);
            var miny = field.Keys.Min(v => v.Y);
            var maxx = field.Keys.Max(v => v.X);
            var maxy = field.Keys.Max(v => v.Y);
            
            for (var y = miny; y <= maxy; y++)
            {
                for (var x = minx; x <= maxx; x++)
                {
                    var old = Console.ForegroundColor;
                    if (path.Contains(new(x, y)))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    if (x == 0 && y == 0)
                    {
                        Console.Write("D");
                    }
                    else
                    {
                        field.TryGetValue(new Vector(x, y), out var v);
                        Console.Write(v == 0 ? ' ' : v);
                    }
                    Console.ForegroundColor = old;
                }
                Console.WriteLine();
            }
        }

        public override void Solve()
        {
            var a = new Channel();
            var b = new Channel();
            var i = new IntCodeInterpreter(GetInputLines(false).First(), a.Receive, b.Send);
            Task.Run(i.Run);

            var field = new Dictionary<Vector, char>();
            var pos = new Vector();
            var directions = new[]
            {
                (new Vector(1, 0), 4), 
                (new Vector(-1, 0), 3), 
                (new Vector(0, 1), 2),
                (new Vector(0, -1), 1)
            };
            field[pos] = '.';

            void Move(Vector p)
            {
                if (p == pos)
                {
                    return;
                }

                var path = Utils.Bfs(pos,
                    v => directions.Where(d => field.TryGetValue(v + d.Item1, out var f) && f != '#')
                        .Select(d => v + d.Item1),
                    v => v.X == p.X && v.Y == p.Y);
                foreach (var x in path)
                {
                    if (x != pos)
                    {
                        var dir = directions.First(d => d.Item1 == x - pos).Item2;
                        a.Send(dir);
                        b.Receive();
                        pos = x;
                    }
                }
            }

            void Explore()
            {
                foreach (var dir in directions)
                {
                    if (!field.ContainsKey(pos + dir.Item1))
                    {
                        a.Send(dir.Item2);
                        var r = b.Receive();
                        field[pos + dir.Item1] = r switch
                        {
                            0 => '#',
                            1 => '.',
                            2 => 'O'
                        };
                        if (r != 0)
                        {
                            a.Send(directions.First(d => d.Item1 == -dir.Item1).Item2);
                            b.Receive();
                        }
                    }
                }
            }
            
            var path = Utils.Bfs(new Vector(), v =>
            {
                Move(v);
                Explore();
                return directions.Select(d => v + d.Item1).Where(p => field[p] != '#');
            }, v => field.TryGetValue(v, out var f) && f == 'O');
            Visualize(field, path.ToHashSet());
            Console.WriteLine(path.Count - 1);
        }

        public override void SolveMain()
        {
            var a = new Channel();
            var b = new Channel();
            var i = new IntCodeInterpreter(GetInputLines(false).First(), a.Receive, b.Send);
            Task.Run(i.Run);

            var field = new Dictionary<Vector, char>();
            var pos = new Vector();
            var directions = new[]
            {
                (new Vector(1, 0), 4),
                (new Vector(-1, 0), 3),
                (new Vector(0, 1), 2),
                (new Vector(0, -1), 1)
            };
            field[pos] = '.';

            void Move(Vector p)
            {
                if (p == pos)
                {
                    return;
                }

                var path = Utils.Bfs(pos,
                    v => directions.Where(d => field.TryGetValue(v + d.Item1, out var f) && f != '#')
                        .Select(d => v + d.Item1),
                    v => v.X == p.X && v.Y == p.Y);
                foreach (var x in path)
                {
                    if (x != pos)
                    {
                        var dir = directions.First(d => d.Item1 == x - pos).Item2;
                        a.Send(dir);
                        b.Receive();
                        pos = x;
                    }
                }
            }

            void Explore()
            {
                foreach (var dir in directions)
                {
                    if (!field.ContainsKey(pos + dir.Item1))
                    {
                        a.Send(dir.Item2);
                        var r = b.Receive();
                        field[pos + dir.Item1] = r switch
                        {
                            0 => '#',
                            1 => '.',
                            2 => 'O'
                        };
                        if (r != 0)
                        {
                            a.Send(directions.First(d => d.Item1 == -dir.Item1).Item2);
                            b.Receive();
                        }
                    }
                }
            }

            while (true)
            {
                var next = field.FirstOrDefault(kv =>
                    kv.Value != '#' && directions.Any(d => !field.ContainsKey(kv.Key + d.Item1)));
                if (next.Value == 0)
                {
                    break;
                }

                Move(next.Key);
                Explore();
            }

            var minute = 0;
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(minute);
                //Visualize(field, new HashSet<Vector>());
                var original = field.ToDictionary(kv => kv.Key, kv => kv.Value);
                var effective = false;
                foreach (var kv in original)
                {
                    if (kv.Value == 'O')
                    {
                        foreach (var d in directions)
                        {
                            if (original[kv.Key + d.Item1] == '.' || original[kv.Key + d.Item1] == 'D')
                            {
                                field[kv.Key + d.Item1] = 'O';
                                effective = true;
                            }
                        }
                    }
                }

                if (effective)
                {
                    ++minute;
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine(minute);
        }
    }
}
