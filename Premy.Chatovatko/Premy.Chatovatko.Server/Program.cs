using Premy.Chatovatko.Server.ClientListener;
using Premy.Chatovatko.Server.Database;
using System;

namespace Premy.Chatovatko.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chatovatko at your service!");

            Config.LoadConfig();
            DBPool.Init();
            GodotFountain.Run();

            Console.ReadLine();
        }
    }
}
