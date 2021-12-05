﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc
{
    public class Day4 : DayBase
    {
        private class Board
        {
            public int[,] Elements { get; set; }

            public bool[,] Drawn { get; set; }

            public int SideLength { get; set; }

            public void Draw(int i)
            {
                for (int x = 0; x < SideLength; ++x)
                {
                    for (int y = 0; y < SideLength; ++y)
                    {
                        if (Elements[x, y] == i)
                        {
                            Drawn[x, y] = true;
                        }
                    }
                }
            }

            public bool Check()
            {
                for (int x = 0; x < SideLength; ++x)
                {
                    int y;
                    for (y = 0; y < SideLength && Drawn[x, y]; ++y)
                    {
                    }
                    if (y == SideLength)
                    {
                        return true;
                    }
                }

                for (int y = 0; y < SideLength; ++y)
                {
                    int x;
                    for (x = 0; x < SideLength && Drawn[x, y]; ++x)
                    {
                    }
                    if (x == SideLength)
                    {
                        return true;
                    }
                }

                return false;
            }

            public int SumUndrawn()
            {
                int sum = 0;
                for (int x = 0; x < SideLength; ++x)
                {
                    for (int y = 0; y < SideLength; ++y)
                    {
                        sum += Drawn[x, y] ? 0 : Elements[x, y];
                    }
                }
                return sum;
            }
        }

        public Day4() : base(4)
        {
        }

        public override void Solve()
        {
            var l = GetInputLines(false).ToList();
            var draw = SplitInts(l[0], ',').ToList();
            int i = 2;
            var boards = new List<Board>();
            while (i < l.Count)
            {
                var head = SplitInts(l[i]).ToList();
                int n = head.Count;
                var arr = new int[n, n];
                for (int j = i; j < i + n; ++j)
                {
                    var line = SplitInts(l[j]).ToList();
                    for (int x = 0; x < n; ++x)
                    {
                        arr[j - i, x] = line[x];
                    }
                }
                boards.Add(new Board { Elements = arr, Drawn = new bool[n, n], SideLength = n });
                i += n + 1;
            }

            foreach (var n in draw)
            {
                foreach (var board in boards.ToList())
                {
                    board.Draw(n);
                    if (board.Check())
                    {
                        if (boards.Count == 1)
                        {
                            Console.WriteLine(board.SumUndrawn() * n);
                            return;
                        }
                        else
                        {
                            boards.Remove(board);
                        }
                    }
                }
            }
        }

        public override void SolveMain()
        {
            throw new NotImplementedException();
        }
    }
}
