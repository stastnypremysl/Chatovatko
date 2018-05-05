using Premy.Chatovatko.Client.ConsoleController;
using System;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chatovatko client at your service!");
            KeyLogger.Run();
        }
    }
}
