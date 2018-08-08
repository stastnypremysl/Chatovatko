#define DEBUG
//#undef DEBUG

using Premy.Chatovatko.Client.Libs.ClientCommunication.Scenarios;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Premy.Chatovatko.Client.Libs.Database;

namespace Premy.Chatovatko.Client.Libs.ClientCommunication
{
    public class Connection : ILoggable
    {
        private readonly int ServerPort = TcpConstants.MAIN_SERVER_PORT;
        private TcpClient client;
        private bool isConnected = false;
        private readonly IClientDatabaseConfig config;

        private SslStream stream;
        private Logger logger;

        private readonly String serverAddress;
        private readonly IConnectionVerificator verificator;

        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public X509Certificate2 ClientCertificate { get; }

        /// <summary>
        /// Constructor for init operations.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="verificator"></param>
        /// <param name="serverAddress"></param>
        /// <param name="clientCertificate"></param>
        /// <param name="userName"></param>
        public Connection(Logger logger, IConnectionVerificator verificator, String serverAddress, 
            X509Certificate2 clientCertificate, IClientDatabaseConfig config, String userName = null)
        {
            this.logger = logger;
            this.verificator = verificator;
            this.serverAddress = serverAddress;
            this.ClientCertificate = clientCertificate;
            this.UserName = userName;
            this.config = config;
        }

        /// <summary>
        /// Constructor for regular use.
        /// Verification will run against public key in settings.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        public Connection(Logger logger, SettingsCapsula settings)
        {
            this.logger = logger;
            this.verificator = new ConnectionVerificator(logger, settings.ServerPublicCertificate);
            this.serverAddress = settings.ServerAddress;
            this.ClientCertificate = settings.ClientCertificate;
            this.UserName = settings.UserName;
            this.config = settings.Config;
        }

        public bool IsConnected()
        {
            return isConnected && stream.IsEncrypted;
        }

        public void Connect()
        {

            client = new TcpClient(serverAddress, ServerPort);
            logger.Log(this, "Client connected.");

            stream = new SslStream(client.GetStream(), false, verificator.AppCertificateValidation);
            X509CertificateCollection clientCertificates = new X509CertificateCollection();
            clientCertificates.Add(ClientCertificate);

            stream.AuthenticateAsClient("Dummy", clientCertificates, SslProtocols.Tls12, false);
            logger.Log(this, "SSL authentication completed.");


            logger.Log(this, "Handshake started.");
            var handshake = Handshake.Login(logger, stream, ClientCertificate, UserName);
            logger.Log(this, "Handshake successeded.");

            UserName = handshake.UserName;
            UserId = handshake.UserId;
            logger.Log(this, $"User {UserName} has id {UserId}.");

            InitSync();
            isConnected = true;

        }

        private void InitSync()
        {
            logger.Log(this, "Initializating synchronization");
            InitClientSync toSend;
            using (Context context = new Context(config))
            {
                toSend = new InitClientSync()
                {
                    UserIds = context.Contacts.Select(c => c.PublicId).ToList(),
                    AesKeysUserIds = context.Contacts.Where(c => c.ReceiveAesKey != null).Select(c => c.PublicId).ToList(),
                    PublicBlobMessagesIds = context.BlobMessages.Where(bm => bm.PublicId != null).Select(bm => bm.PublicId).ToList()
                };

            }
            TextEncoder.SendJson(stream, toSend);

            logger.Log(this, "Initialization of synchronization done");
        }

        public void Disconnect()
        {
            Log("Sending END_CONNECTION command.");
            isConnected = false;
            TextEncoder.SendCommand(stream, ConnectionCommand.END_CONNECTION);
            stream.Close();
            client.Close();
        }

        public void Push()
        {
            Log("Sending PUSH command.");
            TextEncoder.SendCommand(stream, ConnectionCommand.PUSH);
            using (Context context = new Context(config))
            {
                List<long> selfMessages = new List<long>();
                var toSend = context.ToSendMessages.ToList();

                PushCapsula capsula = new PushCapsula()
                {
                    recepientIds = new List<long>()
                };

                foreach(var message in toSend)
                {
                    if(message.RecepientId == UserId)
                    {
                        selfMessages.Add((long)message.BlobMessagesId);
                    }
                    capsula.recepientIds.Add(message.RecepientId);
                }
                capsula.messageToDeleteIds = context.BlobMessages
                    .Where(u => u.DoDelete == 1)
                    .Select(u => (long)u.PublicId).ToList();

#if (DEBUG)
                Log($"Sending capsula with {toSend.Count} messages. {capsula.messageToDeleteIds.Count} will be deleted.");
#endif
                TextEncoder.SendJson(stream, capsula);
#if (DEBUG)
                Log($"Sending message blobs.");
#endif
                foreach(var message in toSend)
                {
                    BinaryEncoder.SendBytes(stream, message.Blob);
                }
#if (DEBUG)
                Log($"Receiving PushResponse");
#endif
                PushResponse response = TextEncoder.ReadJson<PushResponse>(stream);
                var selfMessagesZip = selfMessages.Zip(response.MessageIds, (u, v) => 
                    new { PrivateId = u, PublicId = v});

                foreach(var message in selfMessagesZip)
                {
                    context.BlobMessages.Where(u => u.Id == message.PrivateId)
                        .SingleOrDefault().PublicId = message.PublicId;
                }
#if (DEBUG)
                Log("Saving new public ids.");
#endif
                context.SaveChanges();
#if (DEBUG)
                Log("Cleaning queue.");
#endif
                context.Database.ExecuteSqlCommand("delete from TO_SEND_MESSAGES;");
                context.Database.ExecuteSqlCommand("delete from BLOB_MESSAGES where DO_DELETE=1;;");
                context.SaveChanges();

            }

            Log("Push have been done.");
        }

        public void Pull()
        {
            Log("Sending PULL command.");
            TextEncoder.SendCommand(stream, ConnectionCommand.PULL);

            PullCapsula capsula = TextEncoder.ReadPullCapsula(stream);
#if (DEBUG)
            Log("Received PullCapsula.");
#endif
            using (Context context = new Context(config))
            {
#if (DEBUG)
                Log("Saving new users.");
#endif
                foreach (PullUser user in capsula.Users)
                {
                    context.Contacts.Add(new Contacts
                    {
                        PublicId = user.UserId,
                        PublicCertificate = user.PublicCertificate,
                        UserName = user.UserName
                    });
                }
                context.SaveChanges();

#if (DEBUG)
                Log("Saving trusted contacts.");
#endif
                context.Database.ExecuteSqlCommand("update CONTACTS set TRUSTED=0;");
                context.SaveChanges();

                foreach(var user in context.Contacts
                    .Where(users => capsula.TrustedUserIds.Contains(users.PublicId)))
                {
                    user.Trusted = 1;
                }
                context.SaveChanges();
#if (DEBUG)
                Log("Receiving and saving AES keys.");
#endif
                foreach (var id in capsula.AesKeysUserIds)
                {
                    var user = context.Contacts.Where(con => con.PublicId == id).SingleOrDefault();
                    user.ReceiveAesKey = BinaryEncoder.ReceiveBytes(stream);
                }
                context.SaveChanges();
#if (DEBUG)
                Log("Receiving and saving messages.");
#endif
                foreach (PullMessage metaMessage in capsula.Messages)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            BlobMessages metaBlob = new BlobMessages()
                            {
                                PublicId = metaMessage.PublicId,
                                Failed = 0,
                                DoDelete = 0
                            };
                            context.BlobMessages.Add(metaBlob);
                            context.SaveChanges();

                            try
                            {
                                PullMessageParser.ParseEncryptedMessage(context, BinaryEncoder.ReceiveBytes(stream), metaBlob.SenderId, metaBlob.Id);
                            }
                            catch
                            {
                                Log($"Loading of message {metaMessage.PublicId} has failed.");
                                metaBlob.Failed = 1;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
            Log("Pull have been done.");
        }


        private void UntrustContact()
        {
            Log("Sending UNTRUST_CONTACT command.");
            TextEncoder.SendCommand(stream, ConnectionCommand.UNTRUST_CONTACT);
        }

        private void TrustContact()
        {
            Log("Sending TRUST_CONTACT command.");
            TextEncoder.SendCommand(stream, ConnectionCommand.TRUST_CONTACT);
        }


        public string GetLogSource()
        {
            return "Connection";
        }

        private void Log(String message)
        {
            logger.Log(this, message);
        }

        
    }
}
