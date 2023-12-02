using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc.Parsing
{
    public static class Parser
    {
        public static IParser<char> Char(Func<char, bool> decider) => new CharParser(decider);

        public static IParser<int> Digit() => Char(char.IsDigit).Map(c => c - '0');

        public static IParser<char> Letter() => Char(char.IsLetter);

        public static IParser<char> WhitespaceChar() => Char(char.IsWhiteSpace);

        public static IParser<char> Literal(char e) => Char(c => c == e);

        public static IParser<TTo> Map<TFrom, TTo>(this IParser<TFrom> from, Func<TFrom, TTo> converter) => new MapParser<TFrom, TTo>(from, converter);

        public static IParser<Discard> Discard<T>(this IParser<T> from) => from.Map(_ => new Discard());

        public static IParser<(TFirst, TSecond)> ThenOther<TFirst, TSecond>(this IParser<TFirst> first, IParser<TSecond> second) => new SequenceParser<TFirst, TSecond>(first, second);

        public static IParser<T> ThenOther<T>(this IParser<T> first, IParser<Discard> second) => (new SequenceParser<T, Discard>(first, second)).Map(x => x.Item1);

        public static IParser<T> ThenOther<T>(this IParser<Discard> first, IParser<T> second) => (new SequenceParser<Discard, T>(first, second)).Map(x => x.Item2);

        public static IParser<Discard> ThenOther(this IParser<Discard> first, IParser<Discard> second) => (new SequenceParser<Discard, Discard>(first, second)).Discard();

        public static IParser<IEnumerable<T>> Repeat<T>(this IParser<T> parser, int min, int max) => new RepeatParser<T>(parser, min, max);

        public static IParser<IEnumerable<T>> Repeat<T>(this IParser<T> parser) => new RepeatParser<T>(parser);

        public static IParser<Discard> Repeat(this IParser<Discard> parser) => parser.Repeat().Discard();

        public static IParser<T> OptionalC<T>(this IParser<T> parser) where T : class
        {
            return parser.Repeat(0, 1).Map(e => e.FirstOrDefault());
        }

        public static IParser<T?> OptionalS<T>(this IParser<T> parser) where T : struct
        {
            return parser.Repeat(0, 1).Map(e => e.Select(x => (T?)x).FirstOrDefault());
        }

        public static IParser<IEnumerable<T>> Then<T>(this IParser<T> first, IParser<T> second)
        {
            return first.ThenOther(second).Map(MakeSequence);

            IEnumerable<T> MakeSequence((T, T) t)
            {
                yield return t.Item1;
                yield return t.Item2;
            }
        }

        public static IParser<IEnumerable<T>> Then<T>(this IParser<IEnumerable<T>> first, IParser<T> second)
        {
            return first.ThenOther(second).Map(MakeSequence);

            IEnumerable<T> MakeSequence((IEnumerable<T>, T) t)
            {
                foreach (var x in t.Item1)
                {
                    yield return x;
                }
                yield return t.Item2;
            }
        }

        public static IParser<IEnumerable<T>> Then<T>(this IParser<T> first, IParser<IEnumerable<T>> second)
        {
            return first.ThenOther(second).Map(MakeSequence);

            IEnumerable<T> MakeSequence((T, IEnumerable<T>) t)
            {
                yield return t.Item1;
                foreach (var x in t.Item2)
                {
                    yield return x;
                }
            }
        }

        public static IParser<string> Then(this IParser<char> first, IParser<char> second)
        {
            return new SequenceParser<char, char>(first, second).Map(t => t.Item1.ToString() + t.Item2);
        }

        public static IParser<string> Then(this IParser<string> first, IParser<char> second)
        {
            return new SequenceParser<string, char>(first, second).Map(t => t.Item1 + t.Item2);
        }

        public static IParser<string> Literal(string s)
        {
            IParser<string> res = Literal(s[0]).Map(c => c.ToString());
            for (var i = 1; i < s.Length; i++)
            {
                res = res.Then(Literal(s[i]));
            }
            return res;
        }

        public static IParser<string> String(this IParser<char> parser)
        {
            return parser.Repeat(1, int.MaxValue).Map(e => new string(e.ToArray()));
        }

        public static IParser<int> Integer()
        {
            return Digit().Repeat().Map(e => e.Aggregate(0, (a, b) => 10 * a + b));
        }

        public static IParser<T> Enum<T>()
        {
            return Letter().String().Map(s => (T)System.Enum.Parse(typeof(T), s, true));
        }

        public static IParser<Discard> Whitespace() => WhitespaceChar().Repeat().Discard();

        public static IParser<(TFirst, TSecond)> ThenWs<TFirst, TSecond>(this IParser<TFirst> first, IParser<TSecond> second) => first.ThenOther(Whitespace()).ThenOther(second);

        public static IParser<T> ThenWs<T>(this IParser<T> first, IParser<Discard> second) => first.ThenOther(Whitespace()).ThenOther(second);

        public static IParser<T> ThenWs<T>(this IParser<Discard> first, IParser<T> second) => first.ThenOther(Whitespace()).ThenOther(second);

        public static IParser<Discard> ThenWs(this IParser<Discard> first, IParser<Discard> second) => first.ThenOther(Whitespace()).ThenOther(second);

        public static IParser<T> ThenOther<T>(this IParser<T> first, string second) => first.ThenOther(Literal(second).Discard());

        public static IParser<T> ThenOther<T>(this string first, IParser<T> second) => Literal(first).Discard().ThenOther(second);

        public static IParser<T> ThenWs<T>(this IParser<T> first, string second) => first.ThenWs(Literal(second).Discard());

        public static IParser<T> ThenWs<T>(this string first, IParser<T> second) => Literal(first).Discard().ThenWs(second);

        public static IParser<IEnumerable<TFirst>> RepeatWithSeperator<TFirst, TSep>(this IParser<TFirst> first, IParser<TSep> seperator)
        {
            return first.Then(seperator.Discard().ThenOther(first).Repeat());
        }

        public static IParser<IEnumerable<T>> RepeatWithSeperator<T>(this IParser<T> first, string seperator) => first.RepeatWithSeperator(Literal(seperator).Discard());

        public static IParser<IEnumerable<TFirst>> RepeatWithSeperatorWs<TFirst, TSep>(this IParser<TFirst> first, IParser<TSep> seperator)
        {
            return first.Then(Whitespace().ThenOther(seperator.Discard().ThenWs(first).Repeat()));
        }

        public static IParser<IEnumerable<T>> RepeatWithSeperatorWs<T>(this IParser<T> first, string seperator) => first.RepeatWithSeperatorWs(Literal(seperator).Discard());

        public static IParser<T> Or<T>(this IParser<T> left, IParser<T> right) => new ChoiceParser<T>(left, right);
    }
}
