using System;
using NServiceBus.MessageMutator;

namespace server
{
    public class MyMutator : IMessageMutator
    {
        private static int Count;
        private readonly int Current = ++Count;

        public MyMutator()
        {
            Console.WriteLine("Mutator Created {0}", Current);
        }

        public object MutateOutgoing(object message)
        {
            Console.WriteLine("MutateOutgoing {0}", Current);
            return message;
        }

        public object MutateIncoming(object message)
        {
            Console.WriteLine("MutateIncoming {0}", Current);
            return message;
        }
    }
}