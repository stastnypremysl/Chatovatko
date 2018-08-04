using System;
using static System.Console;
using Premy.Chatovatko.Server.Logging;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Client.Libs.Database;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels;
using Premy.Chatovatko.Client.Libs.ClientCommunication;
using Premy.Chatovatko.Client.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static Logger logger;
        static IClientDatabaseConfig config;
        static DBInitializator initializator;
        static SettingsLoader settingsLoader;
        static SettingsCapsula settings = null;

        static void Main(string[] args)
        {
            logger = new Logger(new ConsoleLoggerOutput());
            WriteLine("Chatovatko client at your service!");
            try {
                config = new ConsoleClientDatabaseConfig();
                initializator = new DBInitializator(config, logger);
                initializator.DBEnsureCreated();

                settingsLoader = new SettingsLoader(config, logger);
                
                if (settingsLoader.Exists())
                {
                    Log("Settings exists and will be loaded.");
                    settings = settingsLoader.GetSettingsCapsula();
                }
                else
                {
                    Log("Settings doesn't exist.");
                }

                bool running = true;
                while(running){
                    try
                    {
                        String command = ReadLine().Trim();
                        String[] commandParts = command.Split(' ');

                        switch (commandParts[0])
                        {
                            case "init":
                                if (commandParts.Length < 3)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }

                                bool? newUser = null;
                                string userName = null;
                                string serverAddress = commandParts[2];
                                switch (commandParts[1])
                                {
                                    case "new":
                                        newUser = true;
                                        break;
                                    case "login":
                                        newUser = false;
                                        break;
                                    default:
                                        WriteSyntaxError(commandParts[1]);
                                        break;
                                }
                                if(newUser != null)
                                {
                                    X509Certificate2 cert;
                                    if (newUser == true)
                                    {                                        
                                        cert = X509Certificate2Generator.GenerateCACertificate(logger);
                                        WriteLine("Your certificate. Save it.");
                                        WriteLine(Convert.ToBase64String(cert.Export(X509ContentType.Pkcs12)));
                                        WriteLine("----------------------------------------------------------------");
                                        WriteLine("Enter your new unique username:");
                                        userName = ReadLine();

                                    }
                                    else
                                    {
                                        WriteLine("Enter yours certificate please:");
                                        string base64Cert = ReadLine();
                                        cert = new X509Certificate2(Convert.FromBase64String(base64Cert));
                                    }

                                    InfoConnection infoConnection = new InfoConnection(serverAddress, logger);
                                    WriteLine();
                                    ServerInfo info = infoConnection.DownloadInfo();
                                    WriteServerInfo(info);
                                    Write("Do you trust this server (y/n):");

                                    string pushed = ReadLine();
                                    if (!pushed.Equals("y"))
                                    {
                                        break;
                                    }

                                    IConnectionVerificator verificator = new InitConnectionVerificator(logger, info.PublicKey);
                                    Connection conn = new Connection(logger, verificator, serverAddress, cert);
                                    conn.Connect();


                                }
                                break;
                            case "delete":
                                if (commandParts.Length < 2)
                                {
                                    WriteNotEnoughParameters();
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
                            case "download":
                                if (commandParts.Length < 3)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                switch (commandParts[1])
                                {
                                    case "info":
                                        WriteServerInfo(commandParts[2]);
                                        break;
                                    default:
                                        WriteSyntaxError(commandParts[1]);
                                        break;
                                }
                                break;
                            case "generate":
                                if (commandParts.Length < 2)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                switch (commandParts[1])
                                {
                                    case "X509Certificate2":
                                        X509Certificate2 cert = X509Certificate2Generator.GenerateCACertificate(logger);
                                        WriteLine(Convert.ToBase64String(cert.Export(X509ContentType.Pkcs12)));

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
                                WriteStatus(settings.Settings, config);
                                break;
                            default:
                                WriteSyntaxError(commandParts[0]);
                                break;
                        }
                    }
                    catch (ChatovatkoException ex)
                    {
                        logger.Log("Program", "Core", "The command has failed.", true);
                        logger.LogException(ex);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex, "Program", "Core", "The command has failed.");
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

        static void WriteServerInfo(String address)
        {
            InfoConnection infoConnection = new InfoConnection(address, logger);
            ServerInfo info = infoConnection.DownloadInfo();
            WriteServerInfo(info);
        }

        static void WriteServerInfo(ServerInfo info)
        {
            WriteLine("Server name:");
            WriteLine(info.Name);
            WriteLine("Public key:");
            WriteLine(info.PublicKey);
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

        static void WriteNotEnoughParameters()
        {
            WriteLine("Not enough parameters.");
        }

        static void Log(String message)
        {
            logger.Log("Program", "Core", "Settings doesn't exist yet.", false);
        }
    }
}
