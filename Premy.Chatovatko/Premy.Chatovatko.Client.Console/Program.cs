using System;
using static System.Console;
using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Client.Comunication;
using Premy.Chatovatko.Server.Logging;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Client.Console;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Client.Libs.Database;
using Premy.Chatovatko.Client.Libs.Database.Models;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static Logger logger;
        static void Main(string[] args)
        {
            logger = new Logger(new ConsoleLoggerOutput());
            WriteLine("Chatovatko client at your service!");
            try {
                IClientDatabaseConfig config = new ConsoleClientDatabaseConfig();
                DBInitializator initializator = new DBInitializator(config, logger);
                initializator.DBEnsureCreated();

                SettingsLoader settingsLoader = new SettingsLoader(config, logger);
                Settings settings = null;
                if (settingsLoader.Exists())
                {
                    Log("Settings exists and will be loaded.");
                    settings = settingsLoader.GetSettings();
                }
                else
                {
                    Log("Settings doesn't exist.");
                }

                bool running = true;
                while(running){
                    String command = ReadLine().Trim();
                    String[] commandParts = command.Split(' ');
                    
                    switch (commandParts[0])
                    {
                        case "delete":
                            if(commandParts.Length != 2)
                            {
                                WriteSyntaxError(commandParts[0]);
                                break;
                            }
                            switch (commandParts[1])
                            {
                                case "database":
                                    initializator.DBDelete();
                                    running = false;
                                    break;
                                default:
                                    WriteSyntaxError(commandParts[1]);
                                    break;
                            }
                            break;
                        case "exit":
                        case "quit":
                            running = false;
                            break;
                        case "--":
                        case "":
                            break;
                        case "status":
                            WriteStatus(settings, config);
                            break;
                        default:
                            WriteSyntaxError(commandParts[0]);
                            break;
                    }
                }

                //Config config;

                //Connection.Connect();
            }
            catch(Exception ex)
            {
                logger.Log("Program", "Core",String.Format("The client has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
                logger.Close();
            }
            
        }

        static void WriteStatus(Settings settings, IClientDatabaseConfig config)
        {
            WriteLine("----------Chatovatko status-----------");
            WriteLine(DateTime.Now);
            WriteLine(String.Format("Database address: {0}", config.DatabaseAddress));
            if(settings == null)
            {
                WriteLine("Settings doesn't exist.");
            }
            else
            {

            }
        }

        static void WriteSyntaxError(String where)
        {
            WriteLine(String.Format("Syntax error near {0}", where));
        }

        static void Log(String message)
        {
            logger.Log("Program", "Core", "Settings doesn't exist yet.", false);
        }
    }
}
