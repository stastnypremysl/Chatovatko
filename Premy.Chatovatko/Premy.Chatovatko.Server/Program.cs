using Premy.Chatovatko.Server.ClientListener;
using Premy.Chatovatko.Server.Database;
using Premy.Chatovatko.Server.Logging;
using System;

namespace Premy.Chatovatko.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chatovatko at your service!");

            try
            { 
                ServerConfig.LoadConfig();
                ServerCert.Load();
                DBPool.Init();
                GodotFountain.Run();
            }
            catch(Exception ex)
            {
                ConsoleServerLogger.LogCoreError(String.Format("The server has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            { 
                Console.ReadLine();
            }
        }
    }
}
