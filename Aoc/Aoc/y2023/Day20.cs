using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Aoc.Parsing.Parser;

namespace Aoc.y2023
{
    public class Day20 : DayBase
    {
        public Day20() : base(20)
        {
        }

        private enum Pulse
        {
            None,
            Low,
            High
        }

        private class Gate
        {
            private readonly Func<Gate, Pulse, Pulse> _transition;

            public Pulse State { get; set; }

            public List<string> Inputs { get; set; } = new();

            public List<string> Outputs { get; set; } = new();

            public char Type { get; }

            public Gate(char type, Pulse initial, Func<Gate, Pulse, Pulse> transition)
            {
                this._transition = transition;
                this.State = initial;
                this.Type = type;
            }

            public void Update(Pulse pulse, System system)
            {
                if (pulse == Pulse.None)
                {
                    return;
                }

                system.Pulses[pulse] = system.Pulses.GetWithDefault(pulse, 0) + 1;
                var next = _transition(this, pulse);
                if (next != Pulse.None)
                {
                    State = next;
                    foreach (var o in Outputs)
                    {
                        system.Queue.Enqueue(() =>
                        {
                            system.Gates[o].Update(State, system);
                        });
                    }
                }
            }

            public static Gate Identity() => new (' ', Pulse.Low, (_, p) => p);

            public static Gate Rx(System system) => new ('r', Pulse.Low, (_, p) => 
            {
                if (p == Pulse.Low)
                {
                    Console.WriteLine($"RX hit at {system.Iteration}");
                    Environment.Exit(0);
                }

                return p;
            });

            public static Gate FlipFlop() => new ('%', Pulse.Low, (gate, p) =>
            {
                if (p == Pulse.Low)
                {
                    return gate.State switch
                    {
                        Pulse.Low => Pulse.High,
                        _ => Pulse.Low
                    };
                }

                return Pulse.None;
            });

            public static Gate Conjunction(System system) => new Gate('&', Pulse.Low, (gate, _) =>
            {
                return gate.Inputs.All(s => system.Gates[s].State == Pulse.High)
                    ? Pulse.Low
                    : Pulse.High;
            });

            public override string ToString() => State.ToString();
        }

        private class System
        {
            public Dictionary<string, Gate> Gates { get; } = new();
            public Dictionary<Pulse, int> Pulses { get; } = new();
            public Queue<Action> Queue { get; } = new();
            public long Iteration { get; set; }

            public void Tick(Pulse pulse)
            {
                Iteration++;
                Gates["broadcaster"].Update(pulse, this);
                while (Queue.TryDequeue(out var a))
                {
                    a();
                }
            }
        }

        private System Load()
        {
            var system = new System();
            var parser = Literal("%")
                .Or(Literal("&"))
                .OptionalC()
                .Map(s => s switch
                {
                    "%" => Gate.FlipFlop(),
                    "&" => Gate.Conjunction(system),
                    _ => Gate.Identity()
                })
                .ThenWs(Letter().String())
                .Map(t => (Gate: t.Item1, Name: t.Item2))
                .ThenWs("->")
                .ThenWs(Letter().String().RepeatWithSeperatorWs(","))
                .Map(t =>
                {
                    t.Item1.Gate.Outputs = t.Item2.ToList();
                    return t.Item1;
                });
            foreach (var gateSpec in GetInputLines(false).Select(parser.Evaluate))
            {
                if (system.Gates.TryGetValue(gateSpec.Name, out var existing))
                {
                    gateSpec.Gate.Inputs = existing.Inputs;
                }

                foreach (var o in gateSpec.Gate.Outputs)
                {
                    if (!system.Gates.TryGetValue(o, out var other))
                    {
                        other = Gate.Identity();
                        system.Gates.Add(o, other);
                    }
                    other.Inputs.Add(gateSpec.Name);
                }

                system.Gates[gateSpec.Name] = gateSpec.Gate;
            }
            system.Gates["rx"] = Gate.Rx(system);

            return system;
        }

        public override void Solve()
        {
            var system = this.Load();
            for (var i = 0; i < 1000; i++)
            {
                system.Tick(Pulse.Low);
            }

            Console.WriteLine(system.Pulses[Pulse.Low] * system.Pulses[Pulse.High]);
        }

        public override void SolveMain()
        {
            var system = this.Load();
            while (true)
            {
                if (system.Iteration % 1000 == 0)
                {
                    Console.WriteLine($"Iteration: {system.Iteration}");
                }

                system.Tick(Pulse.Low);
            }
        }
    }
}
