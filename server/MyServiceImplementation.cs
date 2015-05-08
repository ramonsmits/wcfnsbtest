using System;
using System.ServiceModel;
using NServiceBus;

namespace server
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.PerCall,
        ReleaseServiceInstanceOnTransactionComplete = true
    )]
    public class MyServiceImplementation : IMyService
    {
        private readonly IBus Bus;
        private static int CallCount;

        public MyServiceImplementation(IBus bus)
        {
            Bus = bus;
            Console.WriteLine("Service created at {0}", DateTime.UtcNow);
        }

        [OperationBehavior(
            TransactionAutoComplete = true,
            TransactionScopeRequired = true
        )]
        public void Test(DateTime sent)
        {
            var received = DateTime.UtcNow;
            var duration = received - sent;

            Console.WriteLine("Send {0} | Received {1} | Duration {2}", sent, received, duration);

            Bus.SendLocal(new MyCommand { Timestamp = sent, Id = ++CallCount });
            Bus.SendLocal(new MyCommand { Timestamp = sent, Id = ++CallCount });

            Console.WriteLine("Done");
        }
    }
}