using System;
using NServiceBus.UnitOfWork;

namespace server
{
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