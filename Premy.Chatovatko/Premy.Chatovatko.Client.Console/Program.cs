using System;
using static System.Console;
using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Client.Comunication;
using Premy.Chatovatko.Server.Logging;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Client.Console;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Client.Libs.Database;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger(new ConsoleLoggerOutput());
            WriteLine("Chatovatko client at your service!");
            try {
                IClientDatabaseConfig config = new ConsoleClientDatabaseConfig();
                DBInitializator initializator = new DBInitializator(config, logger);
                initializator.DBEnsureCreated();

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
                logger.Log("Core", "Program",String.Format("The client has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
                logger.Close();
            }
            
        }

        static void WriteSyntaxError(String where)
        {
            WriteLine(String.Format("Syntax error near {0}", where));
        }
    }
}
