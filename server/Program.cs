using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Wcf;
using NServiceBus;
using NServiceBus.MessageMutator;
using NServiceBus.UnitOfWork;

namespace server
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "Server";

            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            var builder = new ContainerBuilder();
            builder.RegisterType<MyServiceImplementation>().As<IMyService>();//.InstancePerDependency();
            builder.RegisterType<MyMutator>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MyUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();

            var container = builder.Build();

            InitBus(container);

            using (ServiceHost host = new ServiceHost(typeof(MyServiceImplementation)))
            {
                host.AddDependencyInjectionBehavior<IMyService>(container);
                host.Open();
                Console.WriteLine("Press *any* key");
                Console.ReadKey();
                host.Close();
            }
        }

        //static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        //{
        //	Console.WriteLine("FirstChanceException: {0}", e.Exception);
        //}

        //static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //	Console.WriteLine("UnhandledException: {0}", e.ExceptionObject);
        //}

        private static IBus InitBus(IContainer container)
        {
            var busConfiguration = new BusConfiguration();

            // http://docs.particular.net/nservicebus/containers/
            busConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            // http://docs.particular.net/samples/hosting/self-hosting/
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();


            return Bus.Create(busConfiguration).Start();

            //Configure
            //	.With() //AllAssemblies.Matching("this.dll").And("that.dll")
            //	//.DefineEndpointName("endpoint name here")
            //	.AutofacBuilder(container)
            //	.MsmqSubscriptionStorage()
            //	.UnicastBus()
            //	.CreateBus()
            //	.Start();
        }
    }

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

    public class MyUnitOfWork : IManageUnitsOfWork
    {
        private static int Count;
        private readonly int Current = ++Count;

        public MyUnitOfWork()
        {
            Console.WriteLine("UOW Created: {0}", Current);
        }

        public void Begin()
        {
            Console.WriteLine("Uow begin: {0}", Current);
        }

        public void End(Exception ex = null)
        {
            Console.WriteLine("Uow end: {0}", Current);
        }
    }
}
