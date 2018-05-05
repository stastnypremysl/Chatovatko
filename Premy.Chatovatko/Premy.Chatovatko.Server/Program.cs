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
                Config.LoadConfig();
                ServerCert.Load();
                DBPool.Init();
                GodotFountain.Run();
            }
            catch(Exception ex)
            {
                Logger.LogCoreError(String.Format("The server has crashed. Exception:\n{0}", ex.Message));
            }
            finally
            { 
                Console.ReadLine();
            }
        }
    }
}
