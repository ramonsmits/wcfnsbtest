using System;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using NServiceBus;

namespace server
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "Server";

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            var builder = new ContainerBuilder();
            builder.RegisterType<MyServiceImplementation>().As<IMyService>();

            // == Register mutator via Autofac, currently done via NServiceBus bus configuration. See below!
            // builder.RegisterType<MyMutator>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<MyUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();

            var container = builder.Build();

            InitBus(container);

            using (var host = new ServiceHost(typeof(MyServiceImplementation)))
            {
                host.AddDependencyInjectionBehavior<IMyService>(container);
                host.Open();
                Console.WriteLine("Press *any* key to quit");
                Console.ReadKey();
                host.Close();
            }
        }

        static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine("FirstChanceException: {0}", e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("UnhandledException: {0}", e.ExceptionObject);
        }

        static IBus InitBus(IContainer container)
        {
            var busConfiguration = new BusConfiguration();

            // http://docs.particular.net/nservicebus/containers/
            busConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            // http://docs.particular.net/samples/hosting/self-hosting/
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();

            busConfiguration.RegisterComponents(x => x.ConfigureComponent<MyMutator>(DependencyLifecycle.InstancePerUnitOfWork));

            return Bus.Create(busConfiguration).Start();
        }
    }
}
