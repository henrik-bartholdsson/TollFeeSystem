using System;

namespace TollFeeSystem.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stuff myEnum = new Stuff();

            Console.WriteLine("Hello World! " + (int) Stuff.StuffB);
        }



        public enum Stuff
        {
            StuffA = 1,
            StuffB = 2,
        }
    }
}
