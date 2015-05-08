using System;
using NServiceBus;

namespace server
{
    public class MyCommandHandler : IHandleMessages<MyCommand>
    {
        public IBus Bus { get; set; }

        public void Handle(MyCommand message)
        {
            Console.WriteLine("Handle: {0}", message.Id);
            if (message.Id == 0) return;
            message.Id = 0;
            Bus.SendLocal(message);
        }
    }
}