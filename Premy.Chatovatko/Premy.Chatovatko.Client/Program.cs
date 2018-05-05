using Premy.Chatovatko.Client.ConsoleController;
using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Client.Comunication;
using System;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chatovatko client at your service!");
            try { 
                Logger.Init();
                Logger.LogCore("Core has started inicialization");

                KeyLogger.Run();
                Config.Load();

                Connection.Connect();
            }
            catch(Exception ex)
            {
                Logger.LogCore(String.Format("The client has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                Logger.Close();
            }
            
        }
    }
}
