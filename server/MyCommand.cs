using System;
using NServiceBus;

namespace server
{
    public class MyCommand : ICommand
    {
        public DateTime Timestamp { get; set; }

        public int Id { get; set; }
    }
}