using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoc
{
    internal class Channel
    {
        private readonly Queue<long> q = new Queue<long>();

        public long? ReceiveUnlessCompleted(Task syncWith)
        {
            lock (this.q)
            {
                if (this.q.Count == 0)
                {
                    while (!Monitor.Wait(this.q, TimeSpan.FromMilliseconds(100)))
                    {
                        if (syncWith != null && syncWith.IsCompleted)
                        {
                            return null;
                        }
                    }
                }

                return this.q.Dequeue();
            }
        }

        public long Receive()
        {
            return ReceiveUnlessCompleted(null).Value;
        }

        public void Send(long value)
        {
            lock (this.q)
            {
                this.q.Enqueue(value);
                Monitor.Pulse(this.q);
            }
        }
    }
}
