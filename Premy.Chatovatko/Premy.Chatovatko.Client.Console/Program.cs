using System;
using static System.Console;
using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Client.Comunication;
using Premy.Chatovatko.Server.Logging;
using Premy.Chatovatko.Libs.Logging;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger(new ConsoleLoggerOutput());
            WriteLine("Chatovatko client at your service!");
            try {
                
                //Config config;

                //Connection.Connect();
            }
            catch(Exception ex)
            {
                logger.Log("Core", "Program",String.Format("The client has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
                logger.Close();
            }
            
        }
    }
}
