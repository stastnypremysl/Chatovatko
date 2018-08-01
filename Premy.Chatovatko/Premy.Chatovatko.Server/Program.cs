using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.ClientListener;
using Premy.Chatovatko.Server.Database;
using Premy.Chatovatko.Server.Logging;
using System;

namespace Premy.Chatovatko.Server
{
    class Program : ILoggable
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chatovatko at your service!");
            Logger logger = new Logger(new ConsoleLoggerOutput());
            try
            {
                ServerConfig config = new ServerConfig(logger);
                config.LoadConfig();

                ServerCert certificate = new ServerCert();
                certificate.Load(config);

                DBPool pool = new DBPool(logger, config);

                GodotFountain godotFountain = new GodotFountain(logger, pool, certificate);
                godotFountain.Run();
            }
            catch(Exception ex)
            {
                logger.Log("Program", "Core", String.Format("The server has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
                logger.Close();
                Console.ReadLine();
            }
        }

        public string GetLogSource()
        {
            return "Core";
        }
    }
}
