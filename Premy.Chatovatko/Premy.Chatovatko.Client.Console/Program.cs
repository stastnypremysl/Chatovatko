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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Premy.Chatovatko.Client.Libs.Cryptography;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.InsertModels;
using Premy.Chatovatko.Client.Libs.Database.DeleteModels;
using Premy.Chatovatko.Client.Libs.Database.UpdateModels;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.SearchContact;

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
        main:
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

                                        WriteLine("If you are logining to this server first time, it is nessary to enter you new unique username:");
                                        userName = ReadLine();

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
                                    connection = new Connection(logger, verificator, serverAddress, clientCert, config, userName);
                                    connection.Connect();

                                    Log("Saving settings.");
                                    settingsLoader.Create(clientCert, connection.UserId, connection.UserName, info.Name, serverAddress, info.PublicKey, (int)connection.ClientId);
                                    settings = settingsLoader.GetSettingsCapsula();

                                    Log("Saving the self AES key.");
                                    //The only user outside of the chain
                                    using (Context context = new Context(config))
                                    {
                                        context.Contacts.Add(new Contacts()
                                        {
                                            PublicId = connection.UserId,
                                            UserName = connection.UserName,
                                            AlarmPermission = 1,
                                            BlobMessagesId = null,

                                            NickName = null,
                                            Trusted = 1,
                                            ReceiveAesKey = connection.SelfAesPassword?.Password,
                                            SendAesKey = connection.SelfAesPassword?.Password,

                                            PublicCertificate = X509Certificate2Utils.ExportToPem(clientCert)
                                        });
                                        context.SaveChanges();
                                    }

                                    Log("Self-trustification begin.");
                                    connection.TrustContact(connection.UserId);
                                    Log("Self-trustification done.");

                                    Log("Updating.");
                                    connection.Pull();
                                    connection.Push();
                                    Log("Updating done.");

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

                            case "search":
                                if (commandParts.Length < 3)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }

                                SearchCServerCapsula searchCapsula;
                                switch (commandParts[1])
                                {
                                    case "id":
                                        searchCapsula = connection.SearchContact(Int32.Parse(commandParts[2]));
                                        break;

                                    case "username":
                                        searchCapsula = connection.SearchContact(BuildFromRest(commandParts, 2));
                                        break;

                                    default:
                                        WriteSyntaxError(commandParts[1]);
                                        throw new Exception("Syntax error");
                                }
                                break;

                            case "push":
                                connection.Push();
                                break;

                            case "pull":
                                connection.Pull();
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
                                        WriteLine();

                                        running = false;
                                        config = null;
                                        initializator = null;
                                        settingsLoader = null;
                                        settings = null;
                                        connection = null;
                                        goto main;

                                    case "message":
                                        if (commandParts.Length < 3)
                                        {
                                            WriteNotEnoughParameters();
                                            break;
                                        }
                                        DeleteMessage(Int32.Parse(commandParts[2]));
                                        break;

                                    case "thread":
                                        if (commandParts.Length < 3)
                                        {
                                            WriteNotEnoughParameters();
                                            break;
                                        }
                                        DeleteThread(Int32.Parse(commandParts[2]));
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

                            case "ls":
                                if (commandParts.Length < 2)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                switch (commandParts[1])
                                {
                                    case "users":
                                        WriteUsers();
                                        break;
                                    case "threads":
                                        WriteThreads();
                                        break;
                                    case "messages":
                                        if (commandParts.Length < 3)
                                        {
                                            WriteNotEnoughParameters();
                                            break;
                                        }
                                        WriteMessages(Int32.Parse(commandParts[2]));
                                        break;
                                    default:
                                        WriteSyntaxError(commandParts[1]);
                                        break;
                                }
                                break;

                            case "post":
                                if (commandParts.Length < 4)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                switch (commandParts[1])
                                {
                                    case "thread":
                                        PostThread(Int32.Parse(commandParts[2]), BuildFromRest(commandParts, 3));
                                        break;
                                    case "message":
                                        PostMessage(Int32.Parse(commandParts[2]), commandParts[3]);
                                        break;
                                    default:
                                        WriteSyntaxError(commandParts[1]);
                                        break;
                                }
                                break;

                            case "rename":
                                if (commandParts.Length < 4)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                switch (commandParts[1])
                                {
                                    case "thread":
                                        RenameThread(Int32.Parse(commandParts[2]), BuildFromRest(commandParts, 3));
                                        break;
                                    default:
                                        WriteSyntaxError(commandParts[1]);
                                        break;
                                }
                                break;

                            case "trust":
                                if (commandParts.Length < 2)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                VerifyConnectionOpened(true);
                                connection.TrustContact(Int32.Parse(commandParts[1]));
                                break;

                            case "untrust":
                                if (commandParts.Length < 2)
                                {
                                    WriteNotEnoughParameters();
                                    break;
                                }
                                VerifyConnectionOpened(true);
                                connection.UntrustContact(Int32.Parse(commandParts[1]));
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
                            case "aesTrial":
                                if (!VerifyConnectionOpened(true))
                                {
                                    break;
                                }
                                AesTrial();
                                break;

                            case "exit":
                            case "quit":
                                running = false;
                                break;

                            case "--":
                            case "":
                            case "#":
                                break;

                            case "status":
                                WriteStatus(settings.Settings, config);
                                break;

                            default:
                                WriteSyntaxError(commandParts[0]);
                                break;
                        }
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
                logger.LogException(ex, "Program", "Core", "Core has crashed.");
            }
            finally
            {
                logger.Close();
                logger = null;
            }
            
        }

        static void AesTrial()
        {
            using(Context context = new Context(config))
            {
                byte[] aesBinKey = context.Contacts
                .Where(u => u.PublicId == connection.UserId)
                .Select(u => u.ReceiveAesKey)
                .SingleOrDefault();

                AESPassword key = new AESPassword(aesBinKey);
                String testStr = "Testy tisty teristy\n";
                for(int i = 0; i != 3; i++)
                {
                    testStr += testStr;
                }
                WriteLine($"Encrypting string: {testStr}");

                byte[] data = Encoding.UTF8.GetBytes(testStr);
                byte[] encrypted = key.Encrypt(data);
                WriteLine(Encoding.UTF8.GetString(key.Decrypt(encrypted)));
            }
        }

        static string BuildFromRest(string[] data, int startIndex)
        {
            StringBuilder builder = new StringBuilder();
            for(int i = startIndex; i != data.Length; i++)
            {
                builder.Append(data[i]);
                builder.Append(' ');
            }
            return builder.ToString();
        }

        static void PostMessage(long threadId, string eof)
        {
            StringBuilder textBuilder = new StringBuilder();
            while (true)
            {
                String readedLine = ReadLine();
                if (readedLine.Equals(eof))
                {
                    break;
                }
                textBuilder.Append(readedLine);
                textBuilder.Append('\n');
            }
            using (Context context = new Context(config))
            {
                long recepientId = context.MessagesThread
                    .Where(u => u.Id == threadId)
                    .Select(u => u.WithUser)
                    .Single();
                CMessage message = new CMessage(InfoTools.GetMessageThreadPublicId (context, threadId)
                    , textBuilder.ToString(), DateTime.Now, recepientId, settings.UserPublicId);
                
                PushOperations.Insert(context, message, recepientId, settings.UserPublicId);
                context.SaveChanges();
            }
        }

        static void DeleteMessage(long messageId)
        {
            using (Context context = new Context(config))
            {
                var toDelete = context.Messages
                    .Where(u => u.Id == messageId)
                    .Single();
                new DMessage(toDelete, settings.UserPublicId).DoDelete(context);
                context.SaveChanges();
            }
        }

        static void DeleteThread(long threadId)
        {
            using(Context context = new Context(config))
            {
                var toDelete = context.MessagesThread
                    .Where(u => u.Id == threadId)
                    .Single();
                new DMessageThread(toDelete, settings.UserPublicId).DoDelete(context);
                context.SaveChanges();
            }
        }

        static void RenameThread(long threadId, string newName)
        {
            using (Context context = new Context(config))
            {
                var toRename = new UMessageThread(context.MessagesThread
                    .Where(u => u.Id == threadId)
                    .Single(), settings.UserPublicId);
                toRename.Name = newName;
                PushOperations.Update(context, toRename, toRename.WithUser, settings.UserPublicId);
                context.SaveChanges();
            }
        }

        static void PostThread(long userId, string name)
        {
            using (Context context = new Context(config))
            {
                CMessageThread thread = new CMessageThread(context, name, false, userId, settings.UserPublicId);
                PushOperations.Insert(context, thread, userId, settings.UserPublicId);
                context.SaveChanges();
            }
        }

        static void WriteMessages(long threadId)
        {
            String format = "{0,-4} {1,-25} {2,-12}\n{3}\n";
            using (Context context = new Context(config))
            {
                WriteLine();
                var publicThreadId = InfoTools.GetMessageThreadPublicId(context, threadId);

                foreach (var message in context.Messages
                    .Where(u => u.IdMessagesThread == publicThreadId))
                {
                    WriteLine("{0,-4} {1,-25} {2,-12}", "Id", "Date", "Sender");
                    WriteLine(format, message.Id, message.Date, InfoTools.GetMessageSenderUserId(context, message.Id), message.Text);
                }
            }
        }

        static void WriteThreads()
        {
            String format = "{0,-4} {5,-15} {2,-12} {3,-12} {4,-12} {1,-35}";
            using (Context context = new Context(config))
            {
                WriteLine();
                WriteLine(format, "Id", "Name", "Archived", "WithUserId", "Onlive", "PublicId");
                foreach(var thread in context.MessagesThread)
                {
                    WriteLine(format, thread.Id, thread.Name, thread.Archived == 1, thread.WithUser, thread.Onlive == 1, thread.PublicId);
                }
            }
        }

        static void WriteUsers()
        {
            String format = "{0,-4} {1,-12} {2,-12} {3,-12} {4,-12} {5,-12} {6,-30}";
            using (Context context = new Context(config))
            {
                WriteLine();
                WriteLine(format, "Id", "NickName", "Trusted", "AlarmPer", "ReceiveKey", "SendKey", "UserName");
                foreach(var user in 
                    from contacts in context.Contacts
                    select new
                    {
                        Id = contacts.PublicId,
                        Trusted = contacts.Trusted == 1,
                        contacts.NickName,
                        AlarmPermission = contacts.AlarmPermission == 1,

                        contacts.UserName,
                        LoadedReceiveAesKey = contacts.ReceiveAesKey != null,
                        LoadedSendAesKey = contacts.SendAesKey != null,
                    })
                {
                    WriteLine(format, user.Id, user.NickName, user.Trusted, user.AlarmPermission, user.LoadedReceiveAesKey, user.LoadedSendAesKey, user.UserName);
                }
            }
        }

        static void Set

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
                connection.Connect();
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
            WriteLine("Public key SHA-256 sum:");
            WriteLine(SHA256Utils.ComputeSha256Hash(info.PublicKey));
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
            logger.Log("Program", "Core", message, false);
        }
    }
}
