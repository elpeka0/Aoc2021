using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.y2022
{
    public class Day19 : DayBase
    {
        private const int Ore = 0;
        private const int Clay = 1;
        private const int Obsidian = 2;
        private const int Geode = 3;

        int ParseResource(string n) => n switch
        {
            "ore" => Ore,
            "clay" => Clay,
            "obsidian" => Obsidian,
            "geode" => Geode
        };

        private record Robot(int[] Cost, int Production);

        private record BluePrint(int Id, List<Robot> Robots, int[] MaxCosts);

        public Day19() : base(19)
        {
        }

        private IEnumerable<BluePrint> GetInput()
        {
            foreach (var line in GetInputLines(false))
            {
                var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                parts = parts[1].Split('.', StringSplitOptions.RemoveEmptyEntries);
                var robots = new List<Robot>();
                foreach (var p in parts)
                {
                    var sp = p.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var cd = new int[4];
                    for (var i = 5; i < sp.Length; i += 3)
                    {
                        cd[this.ParseResource(sp[i])] = int.Parse(sp[i - 1]);
                    }
                    robots.Add(new Robot(cd, this.ParseResource(sp[1])));
                }

                yield return new BluePrint(id, robots, Enumerable.Range(0, 4).Select(k => robots.Max(r => r.Cost[k])).ToArray());
            }
        }

        public override void Solve()
        {
            var bluePrints = GetInput().ToList();
            var res = 0;
            foreach (var bluePrint in bluePrints)
            {
                Console.WriteLine($"BP: {bluePrint.Id}");
                var production = new int[4];
                production[Ore] = 1;
                var prod = BestProduction(bluePrint, 24, new int[4], production);
                res += bluePrint.Id * prod;
            }
            Console.WriteLine(res);
        }

        private int BestProduction(BluePrint bluePrint, int tick, int[] resources, int[] production)
        {
            if (tick == 0)
            {
                return resources[Geode];
            }

            var best = 0;
            foreach (var template in bluePrint.Robots.Where(t => t.Production == Geode || production[t.Production] < bluePrint.MaxCosts[t.Production]))
            {
                var time = 0;
                for (var key = 0; key < 4; ++key)
                {
                    var p = production[key];
                    int t;
                    var missing = template.Cost[key] - resources[key];
                    if (missing <= 0)
                    {
                        t = 0;
                    }
                    else if (p == 0)
                    {
                        t = 10000;
                    }
                    else if (missing % p == 0)
                    {
                        t = missing / p;
                    }
                    else
                    {
                        t = missing / p + 1;
                    }

                    if (t > time)
                    {
                        time = t;
                    }
                }

                ++time;

                if (time <= tick)
                {
                    var newResources = resources.ToArray();
                    var newProduction = production.ToArray();
                    for (var key = 0; key < 4; ++key)
                    {
                        newResources[key] += production[key] * time - template.Cost[key];
                    }

                    newProduction[template.Production]++;

                    var next = BestProduction(bluePrint, tick - time, newResources, newProduction);
                    if (next > best)
                    {
                        best = next;
                    }
                }
                else
                {
                    var next = resources[Geode] + production[Geode] * tick;
                    if (next > best)
                    {
                        best = next;
                    }
                }
            }
            return best;
        }

        public override void SolveMain()
        {
            var bluePrints = GetInput().ToList();
            var res = 1;
            foreach (var bluePrint in bluePrints.Take(3))
            {
                Console.WriteLine($"BP: {bluePrint.Id}");
                var production = new int[4];
                production[Ore] = 1;
                var prod = BestProduction(bluePrint, 32, new int[4], production);
                res *= prod;
            }
            Console.WriteLine(res);
        }
    }
}
