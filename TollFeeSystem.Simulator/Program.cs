using System;

namespace TollFeeSystem.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var gothenburg = new DomainArea();
            gothenburg.Run();

            Console.ReadKey();
        }

    }
}
