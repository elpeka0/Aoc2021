using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day07 : DayBase
    {
        private class File
        {
            public string Name { get; }
            public Dictionary<string, File> Children { get; } = new Dictionary<string, File>();
            public long Size { get; private set; }
            public File Parent { get; }

            public File(File parent, string name, long size)
            {
                this.Parent = parent;
                this.Name = name;
                this.Size = size;
            }

            public void Update(List<File> directories)
            {
                if (Children.Count > 0)
                {
                    directories.Add(this);
                    Size = 0;
                    foreach (var child in Children.Values)
                    {
                        child.Update(directories);
                        Size += child.Size;
                    }
                }
            }

            public override string ToString()
            {
                return $"{Name} ({Size}) => {Children.Count}";
            }
        }

        private class Terminal
        {
            public File Root { get; } = new (null, string.Empty, 0);
            private File current;

            public Terminal()
            {
                this.current = this.Root;
            }

            public void Ls(List<string> output)
            {
                foreach (var line in output)
                {
                    var parts = line.Split();
                    var name = parts[1];
                    if (!this.current.Children.ContainsKey(name))
                    {
                        if (parts[0] == "dir")
                        {
                            this.current.Children.Add(name, new File(this.current, name, 0));
                        }
                        else
                        {
                            this.current.Children.Add(name, new File(this.current, name, long.Parse(parts[0])));
                        }
                    }
                }
            }

            public void Cd(string path)
            {
                if (path == "/")
                {
                    this.current = this.Root;
                }
                else if (path == "..")
                {
                    this.current = this.current.Parent;
                }
                else
                {
                    if (!this.current.Children.TryGetValue(path, out var d))
                    {
                        d = new File(this.current, path, 0);
                        this.current.Children.Add(path, d);
                    }

                    this.current = d;
                }
            }
        }

        public Day07() : base(7)
        {
        }

        public override void Solve()
        {
            var cmd = string.Empty;
            var args = new List<string>();
            var terminal = new Terminal();

            void Exec()
            {
                if (cmd.StartsWith("$ cd "))
                {
                    terminal.Cd(cmd.Substring("$ cd ".Length));
                }
                else if (cmd != string.Empty)
                {
                    terminal.Ls(args);
                }

                args.Clear();
            }

            foreach (var line in GetInputLines(false))
            {
                if (line.StartsWith("$"))
                {
                    Exec();
                    cmd = line;
                }
                else
                {
                    args.Add(line);
                }
            }
            Exec();

            var dirs = new List<File>();
            terminal.Root.Update(dirs);

            var fullSize = 70000000;
            var target = 30000000;
            var used = terminal.Root.Size;
            var unused = fullSize - used;
            var delSize = target - unused;
            Console.WriteLine(dirs.OrderBy(d => d.Size).First(d => d.Size >= delSize));
        }

        public override void SolveMain()
        {
        }
    }
}
