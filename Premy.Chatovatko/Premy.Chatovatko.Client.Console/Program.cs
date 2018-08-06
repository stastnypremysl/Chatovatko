using System;
using static System.Console;
using Premy.Chatovatko.Server.Logging;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Client.Libs.Database;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels;
using Premy.Chatovatko.Client.Libs.ClientCommunication;
using Premy.Chatovatko.Client.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Premy.Chatovatko.Libs.Cryptography;

namespace Premy.Chatovatko.Client
{
    class Program
    {
        static Logger logger;
        static IClientDatabaseConfig config;
        static DBInitializator initializator;
        static SettingsLoader settingsLoader;
        static SettingsCapsula settings = null;
        static Connection connection = null;

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

                                if(settings != null)
                                {
                                    WriteLine("Chatovatko is initialized already.");
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
                                    X509Certificate2 clientCert;
                                    if (newUser == true)
                                    {
                                        WriteLine("Your certificate is being created. Please, be patient.");
                                        clientCert = X509Certificate2Generator.GenerateCACertificate(logger);

                                        WriteLine("Your certificate has been generated. Enter path to save it: [default: ~/.chatovatko/mykey.p12]");
                                        string path = ReadLine();
                                        if (path.Equals(""))
                                        {
                                            path = $"{Utils.GetConfigDirectory()}/mykey.p12";
                                        }
                                        X509Certificate2Utils.ExportToPkcs12File(clientCert, path);
                                        WriteLine("----------------------------------------------------------------");

                                        WriteLine("Enter your new unique username:");
                                        userName = ReadLine();

                                    }
                                    else
                                    {
                                        WriteLine("Enter path to your certificate please: [default: ~/.chatovatko/mykey.p12]");
                                        string path = ReadLine();
                                        if (path.Equals(""))
                                        {
                                            path = $"{Utils.GetConfigDirectory()}/mykey.p12";
                                        }
                                        clientCert = X509Certificate2Utils.ImportFromPkcs12File(path, true);
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

                                    IConnectionVerificator verificator = new ConnectionVerificator(logger, info.PublicKey);
                                    Connection conn = new Connection(logger, verificator, serverAddress, clientCert, userName);
                                    conn.Connect();

                                    Log("Saving settings.");
                                    settingsLoader.Create(clientCert, conn.UserId, conn.UserName, info.Name, serverAddress, info.PublicKey);
                                    settings = settingsLoader.GetSettingsCapsula();

                                }
                                break;

                            case "connect":
                                CreateOpenedConnection(true);
                                break;
                            case "disconnect":
                                if (!VerifyConnectionOpened(true))
                                {
                                    break;
                                }
                                connection.Disconnect();
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
                                        WriteLine(X509Certificate2Utils.ExportToBase64(cert));

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

        static bool VerifyConnectionOpened(bool log = false)
        {
            bool ret = connection != null && connection.IsConnected();
            if(ret == false && log)
            {
                Log("Connection isn't opened.");
            }
            return ret;
        }

        static bool VerifySettingsExist(bool log = false)
        {
            bool ret = settings != null;
            if (ret == false && log)
            {
                Log("Settings doesn't exist.");
            }
            return ret;
        }

        static void CreateOpenedConnection(bool log = false)
        {
            if (!VerifyConnectionOpened())
            {
                VerifySettingsExist(false);
                connection = new Connection(logger, settings);
            }
            else if (log)
            {
                Log("Connection already opened.");
            }
        }

        static void CreateIfNotOpenedConnection(bool log = false)
        {
            VerifySettingsExist(true);
            CreateOpenedConnection(false);
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
