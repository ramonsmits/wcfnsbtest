using System;

namespace client
{
    class Program
    {
        static void Main()
        {
            Console.Title = "Client";

            Console.WriteLine("Press space to send a mesage any other key to quit.");

            while (Console.ReadKey().Key == ConsoleKey.Spacebar)
            {
                using (var c = new ProductClient())
                {
                    var timestamp = DateTime.UtcNow;
                    c.Test(timestamp);
                    Console.WriteLine("Call at {0}", timestamp);
                    c.Close();
                }
            }
        }
    }
}
