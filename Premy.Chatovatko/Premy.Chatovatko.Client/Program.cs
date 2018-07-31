using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Client.Comunication;
using System;
using Premy.Chatovatko.Server.Logging;
using Premy.Chatovatko.Libs.Logging;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger(new ConsoleLoggerOutput());
            Console.WriteLine("Chatovatko client at your service!");
            try { 
                
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
