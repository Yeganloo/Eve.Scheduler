using System;

namespace Eve.Scheduler
{
    class Program
    {
        public const string Version = "0.0.1-develop";

        private static string Read()
        {
            Console.Write("/> ");
            return Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($@"Eve scheduler version({Version}).");
        }
    }
}
