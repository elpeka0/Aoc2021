using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day19 : DayBase
    {
        public Day19() : base(19)
        {
        }

        private record Part(IReadOnlyList<long> Fields)
        {
            public long Rating => Fields.Sum();

            public override string ToString()
            {
                return "{" + string.Join(",", Fields) + "}";
            }
        }

        private record Rule(int Field, long Min, long Max, string Target)
        {
            public string Apply(Part part)
            {
                if (Field == -1)
                {
                    return Target;
                }

                if (part.Fields[Field] > Min && part.Fields[Field] < Max)
                {
                    return Target;
                }

                return null;
            }
        }

        private record Workflow(IReadOnlyList<Rule> Rules)
        {
            public string Apply(Part part)
            {
                return Rules.Select(r => r.Apply(part)).First(x => x != null);
            }

            public override string ToString()
            {
                return string.Join(",", Rules);
            }
        }

        private record Factory(IReadOnlyDictionary<string, Workflow> Rules, IReadOnlyList<Part> Parts)
        {
            public bool Apply(Part part)
            {
                var next = "in";
                while (next != "A" && next != "R")
                {
                    next = Rules[next].Apply(part);
                }

                return next == "A";
            }

            public IEnumerable<Part> Filter()
                => Parts.Where(Apply);
        }

        private Factory Load()
        {
            var top = GetInputLines(false).TakeWhile(l => !string.IsNullOrEmpty(l)).ToList();
            var bottom = GetInputLines(false).SkipWhile(i => !string.IsNullOrEmpty(i)).Skip(1).ToList();

            var rule = Letter()
                .String()
                .Map(ParseField)
                .ThenWs(Literal(">").Map(_ => true).Or(Literal("<").Map(_ => false)))
                .ThenWs(Integer())
                .ThenWs(Literal(":").Discard())
                .ThenWs(Letter().String())
                .Map(t =>
                {
                    var field = t.Item1.Item1.Item1;
                    var isMin = t.Item1.Item1.Item2;
                    var value = t.Item1.Item2;
                    var target = t.Item2;
                    return new Rule(field, isMin ? value : long.MinValue, isMin ? long.MaxValue : value, target);
                });
            var workflows = Letter()
                .String()
                .ThenWs("{")
                .ThenWs(rule
                    .ThenWs(",")
                    .Repeat()
                    .ThenWs(Letter().String().Map(s => new Rule(-1, long.MinValue, long.MaxValue, s)))
                    .Map(t => t.Item1.Concat(new[] { t.Item2 })))
                .ThenWs("}")
                .Map(t => (Name: t.Item1, Rules: t.Item2))
                .Evaluate(top)
                .ToDictionary(t => t.Name, t => new Workflow(t.Rules.ToList()));

            var field = Letter()
                .String()
                .Map(ParseField)
                .ThenWs("=")
                .ThenWs(Integer())
                .Map(t => (Field: t.Item1, Value: t.Item2));
            var parts = "{"
                .ThenWs(field.RepeatWithSeperatorWs(","))
                .ThenWs("}")
                .Map(e => new Part(e.Select(t => t.Value).ToArray()))
                .Evaluate(bottom)
                .ToList();

            return new Factory(workflows, parts);

            int ParseField(string s) => s switch
            {
                "x" => 0,
                "m" => 1,
                "a" => 2,
                _ => 3
            };
        }

        public override void Solve()
        {
            var fac = this.Load();
            var res = fac.Filter().Sum(p => p.Rating);
            Console.WriteLine(res);
        }

        public override void SolveMain()
        {
            var fac = this.Load();
            var res = CountWays(fac, "in", new Dictionary<string, IReadOnlyList<HashSet<long>[]>>());
            var full = res.Sum(s => s.Aggregate(1L, (a, b) => a * b.Count));
            Console.WriteLine(full);
        }

        private IReadOnlyList<HashSet<long>[]> CountWays(Factory fac, string target, Dictionary<string, IReadOnlyList<HashSet<long>[]>> visited)
        {
            if (target == "A")
            {
                return new[] { Full() };
            }

            if (target == "R")
            {
                return Array.Empty<HashSet<long>[]>();
            }

            if (visited.TryGetValue(target, out var l))
            {
                return l;
            }

            visited[target] = Array.Empty<HashSet<long>[]>();

            var workflow = fac.Rules[target];

            var res = new List<HashSet<long>[]>();
            for(var i = 0;i < workflow.Rules.Count;i++)
            {
                var rule = workflow.Rules[i];
                var next = Full();
                for (var j = 0; j < i; j++)
                {
                    next[workflow.Rules[j].Field]
                        .RemoveWhere(v => v > workflow.Rules[j].Min && v < workflow.Rules[j].Max);
                }

                if (rule.Field != -1)
                {
                    next[rule.Field].IntersectWith(AllValues().Where(v => v > rule.Min && v < rule.Max));
                }

                foreach (var forSub in CountWays(fac, rule.Target, visited))
                {
                    var n = next.Select(h => h.ToHashSet()).ToArray();
                    Intersect(n, forSub);
                    res.Add(n);
                }
            }

            visited[target] = res;
            return res;

            IEnumerable<long> AllValues() => Enumerable.Range(1, 4000).Select(n => (long)n);

            void Intersect(HashSet<long>[] a, HashSet<long>[] b)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    a[i].IntersectWith(b[i]);
                }
            }

            HashSet<long>[] Full() =>
                Enumerable.Range(0, 4)
                    .Select(_ => AllValues().ToHashSet())
                    .ToArray();
        }
    }
}
