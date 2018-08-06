using MySql.Data.MySqlClient;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.ClientListener.Scenarios;
using Premy.Chatovatko.Server.Database;
using Premy.Chatovatko.Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class Godot : ILoggable
    {

        private readonly ulong id;
        private SslStream stream;

        private readonly Logger logger;
        private readonly X509Certificate2 serverCert;
        private readonly GodotCounter godotCounter;

        private readonly ServerConfig config;
        private UserCapsula user;

        /// <summary>
        /// Already uploaded users. (independetly on aes keys)
        /// </summary>
        private IList<long> userIdsUploaded;

        /// <summary>
        /// Already uploaded blob messages.
        /// </summary>
        private IList<long?> messagesIdsUploaded;

        /// <summary>
        /// Already uploaded aes keys.
        /// </summary>
        private IList<long> aesKesUserIdsUploaded;


        public Godot(ulong id, Logger logger, ServerConfig config, X509Certificate2 serverCert, GodotCounter godotCounter)
        {
            this.id = id;
            this.logger = logger;
            this.serverCert = serverCert;
            this.godotCounter = godotCounter;
            this.config = config;
            godotCounter.IncreaseCreated();
            
            logger.Log(this, "Godot has been born.");
            
        }
        
                
        public void Run(TcpClient client)
        {
            try
            {
                logger.Log(this, String.Format("Godot has been activated. Client IP address is {0}", 
                    LUtils.GetIpAddress(client)));
                godotCounter.IncreaseRunning();

                stream = new SslStream(client.GetStream(), false, CertificateValidation);
                stream.AuthenticateAsServer(serverCert, true, SslProtocols.Tls12, false);

                logger.Log(this, "SSL authentication completed. Starting Handshake.");
                this.user = Handshake.Run(stream, Log, config);

                InitSync();

                bool running = true;
                while (running)
                {
                    ConnectionCommand command = TextEncoder.ReadCommand(stream);
                    switch (command)
                    {
                        case ConnectionCommand.TRUST_CONTACT:
                            Log("TRUST_CONTACT command received.");
                            TrustContact();
                            break;

                        case ConnectionCommand.UNTRUST_CONTACT:
                            Log("UNTRUST_CONTACT command received.");
                            UntrustContact();
                            break;

                        case ConnectionCommand.PULL:
                            Log("PULL command received.");
                            Push();
                            break;

                        case ConnectionCommand.PUSH:
                            Log("PUSH command received.");
                            Pull();
                            break;

                        case ConnectionCommand.END_CONNECTION:
                            Log("END_CONNECTION command received.");
                            running = false;
                            break;
                    }
                }
                
            }
            catch(Exception ex)
            {
                logger.Log(this, String.Format("Godot has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            { 
                stream.Close();
                client.Close();
                godotCounter.IncreaseDestroyed();
                logger.Log(this, "Godot has died.");
            }
        }

        private void InitSync()
        {
            Log("Initializating synchronization.");
            Log("Downloading already uploaded users and messages.");
            InitClientSync initClientSync = TextEncoder.ReadInitClientSync(stream);

            userIdsUploaded = initClientSync.UserIds;
            messagesIdsUploaded = initClientSync.PublicBlobMessagesIds;
            aesKesUserIdsUploaded = initClientSync.AesKeysUserIds;

            Log($"Downloading done.\nUserIds: {userIdsUploaded.Count}\nMessagesIds: {messagesIdsUploaded.Count}\nAESKeys: {aesKesUserIdsUploaded.Count}");
        }

        private void Pull()
        {

        }

        private void Push()
        {
            Log("Pushing started.");
            using (Context context = new Context(config))
            {
                PullCapsula capsula = new PullCapsula();

                List<PullUser> pullUsers = new List<PullUser>();
                foreach(Users user in context.Users.Where(u => !userIdsUploaded.Contains(u.Id)))
                {
                    PullUser pullUser = new PullUser()
                    {
                        PublicCertificate = user.PublicCertificate,
                        UserId = user.Id,
                        UserName = user.UserName
                    };
                    pullUsers.Add(pullUser);
                    userIdsUploaded.Add(user.Id);
                }
                capsula.Users = pullUsers;
                Log($"{pullUsers.Count} users will be pushed.");

                capsula.TrustedUserIds = new List<long>();                
                foreach(var userId in from userKeys in context.UsersKeys
                    where userKeys.Trusted == true
                    where userKeys.SenderId == user.UserId
                    select new { userKeys.RecepientId })
                {
                    capsula.TrustedUserIds.Add(userId.RecepientId);
                }
                Log($"{capsula.TrustedUserIds.Count} users is trusted by host.");


                List<byte[]> messagesBlobsToSend = new List<byte[]>();
                List<byte[]> aesBlobsToSend = new List<byte[]>();

                List<PullMessage> pullMessages = new List<PullMessage>();
                foreach (var message in 
                    from messages in context.BlobMessages
                    where messages.RecepientId == user.UserId //Messages of connected user
                    where !messagesIdsUploaded.Contains(messages.Id) //New messages

                    join keys in context.UsersKeys on messages.RecepientId equals keys.SenderId //Keys sended by connected user
                    where keys.Trusted == true //Only trusted
                    where messages.SenderId == keys.RecepientId //Trusted information just only about sending user
                    select new {messages.SenderId, messages.Content, messages.Id}
                    )
                {
                    pullMessages.Add(new PullMessage()
                    {
                        PublicId = message.Id,
                        SenderId = message.SenderId
                    });
                    messagesBlobsToSend.Add(message.Content);
                    messagesIdsUploaded.Add(message.Id);
                }
                Log($"{messagesBlobsToSend.Count} messageBlobs will be pushed.");

                capsula.AesKeysUserIds = new List<long>();
                foreach (var user in 
                    from userKeys in context.UsersKeys
                    where userKeys.RecepientId == user.UserId
                    where !aesKesUserIdsUploaded.Contains(userKeys.SenderId)
                    select new { userKeys.SenderId, userKeys.EncryptedAesKey })
                {
                    capsula.AesKeysUserIds.Add(user.SenderId);
                    aesKesUserIdsUploaded.Add(user.SenderId);
                    aesBlobsToSend.Add(user.EncryptedAesKey);
                }
                Log($"{capsula.AesKeysUserIds.Count} AES keys will be pushed.");

                Log($"Sending PullCapsula.");
                TextEncoder.SendJson(stream, capsula);

                Log($"Sending Messages content.");
                foreach(byte[] data in messagesBlobsToSend)
                {
                    BinaryEncoder.SendBytes(stream, data);
                }

                Log($"Sending AES keys.");
                foreach(byte[] data in aesBlobsToSend)
                {
                    BinaryEncoder.SendBytes(stream, data);
                }

            }
            Log("Pushing completed.");
        }


        private void UntrustContact()
        {

        }

        private void TrustContact()
        {

        }



        private bool CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        
        public string GetLogSource()
        {
            return String.Format("Godot {0}", id);
        }

        private void Log(String message)
        {
            logger.Log(this, message);
        }
    }
}
