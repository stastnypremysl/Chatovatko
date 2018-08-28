#define DEBUG
//#undef  DEBUG

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


                bool running = true;
                while (running)
                {
                    ConnectionCommand command = BinaryEncoder.ReadCommand(stream);
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
#if (DEBUG)
                            Log("PULL command received.");
#endif
                            Push();
                            break;

                        case ConnectionCommand.PUSH:
#if (DEBUG)
                            Log("PUSH command received.");
#endif
                            Pull();
                            break;

                        case ConnectionCommand.SEARCH_CONTACT:
                            Log("SEARCH_CONTACT command received.");
                            SearchContact();
                            break;

                        case ConnectionCommand.END_CONNECTION:
                            Log("END_CONNECTION command received.");
                            running = false;
                            break;

                        default:
                            throw new Exception("Received unknown command.");

                    }
                }

            }
            catch (Exception ex)
            {
                logger.Log(this, "Godot has crashed.");
                logger.LogException(this, ex);
            }
            finally
            {
                stream.Close();
                client.Close();
                godotCounter.IncreaseDestroyed();
                logger.Log(this, "Godot has died.");
            }
        }

        private void SearchContact()
        {

        }

        
        private void Pull()
        {
#if (DEBUG)
            Log("Pulling started.");
            Log("Receiving PushCapsula.");
#endif

            PushCapsula capsula = TextEncoder.ReadJson<PushCapsula>(stream);
            PushResponse response = new PushResponse()
            {
                MessageIds = new List<long>()
            };

            using (Context context = new Context(config))
            {
#if (DEBUG)
                Log($"Receiving and saving {capsula.recepientIds.Count} blobs.");
#endif
                
                foreach (var messageRecepientId in capsula.recepientIds)
                {
                    BlobMessages blobMessage = new BlobMessages()
                    {
                        Content = BinaryEncoder.ReceiveBytes(stream),
                        RecepientId = (int)messageRecepientId,
                        SenderId = user.UserId
                    };
                    context.BlobMessages.Add(blobMessage);
                    context.SaveChanges();

                    if (messageRecepientId == user.UserId)
                    {
                        response.MessageIds.Add(blobMessage.Id);
                        messagesIdsUploaded.Add(blobMessage.Id);
                    }
                    context.SaveChanges();
                }
#if (DEBUG)
                Log($"Deleting {capsula.messageToDeleteIds.Count} blobs.");
#endif
                foreach(var toDeleteId in capsula.messageToDeleteIds)
                {
                    var toDelete = context.BlobMessages
                        .Where(u => u.RecepientId == user.UserId && u.Id == toDeleteId).SingleOrDefault();
                    if(toDelete != null)
                    { 
                        context.BlobMessages.Remove(toDelete);
                    }
                }
                context.SaveChanges();
            }


#if (DEBUG)
            Log("Sending push response.");
#endif
            TextEncoder.SendJson(stream, response);
#if (DEBUG)
            Log("Pulling ended.");
#endif
        }

        private void Push()
        {
#if (DEBUG)
            Log("Pushing started.");
#endif
            using (Context context = new Context(config))
            {
                ServerPullCapsula capsula = new ServerPullCapsula();

                List<PullUser> pullUsers = new List<PullUser>();
                foreach (Users user in context.Users.Where(u => !userIdsUploaded.Contains(u.Id)))
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
#if (DEBUG)
                Log($"{pullUsers.Count} users will be pushed.");
#endif

                capsula.TrustedUserIds = new List<long>();
                foreach (var userId in (
                    from userKeys in context.UsersKeys
                    where userKeys.Trusted == true && userKeys.SenderId == user.UserId
                    select new { userKeys.RecepientId }))
                {
                    capsula.TrustedUserIds.Add(userId.RecepientId);
                }
#if (DEBUG)
                Log($"{capsula.TrustedUserIds.Count} users is trusted by host.");
#endif

                List<byte[]> messagesBlobsToSend = new List<byte[]>();
                List<byte[]> aesBlobsToSend = new List<byte[]>();

                List<PullMessage> pullMessages = new List<PullMessage>();
                foreach (var message in
                    from messages in context.BlobMessages
                    orderby messages.Id ascending //Message order must be respected
                    where messages.RecepientId == user.UserId //Messages of connected user
                    where !messagesIdsUploaded.Contains(messages.Id) //New messages

                    join keys in context.UsersKeys on messages.RecepientId equals keys.SenderId //Keys sended by connected user
                    where keys.Trusted == true //Only trusted
                    where messages.SenderId == keys.RecepientId //Trusted information just only about sending user
                    select new { messages.SenderId, messages.Content, messages.Id }
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
                capsula.Messages = pullMessages;
#if (DEBUG)
                Log($"{messagesBlobsToSend.Count} messageBlobs will be pushed.");
#endif
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
#if (DEBUG)
                Log($"{capsula.AesKeysUserIds.Count} AES keys will be pushed.");
                Log($"Sending PullCapsula.");
#endif
                TextEncoder.SendJson(stream, capsula);
#if (DEBUG)
                Log($"Sending AES keys.");
#endif
                foreach (byte[] data in aesBlobsToSend)
                {
                    BinaryEncoder.SendBytes(stream, data);
                }
#if (DEBUG)
                Log($"Sending Messages content.");
#endif
                foreach (byte[] data in messagesBlobsToSend)
                {
                    BinaryEncoder.SendBytes(stream, data);
                }

            }
#if (DEBUG)
            Log("Pushing completed.");
#endif
        }


        private void UntrustContact()
        {
            int recepientId = BinaryEncoder.ReadInt(stream);
            using (Context context = new Context(config))
            {
                var key = context.UsersKeys
                    .Where(u => u.RecepientId == recepientId)
                    .Where(u => u.SenderId == user.UserId)
                    .SingleOrDefault();
                if(key != null)
                {
                    key.Trusted = false;
                    context.SaveChanges();
                }
            }
            Log("Untrust contact done.");
        }

        private void TrustContact()
        {
            int recepientId = BinaryEncoder.ReadInt(stream);
            using(Context context = new Context(config))
            {
                var key = context.UsersKeys
                    .Where(u => u.RecepientId == recepientId)
                    .Where(u => u.SenderId == user.UserId)
                    .SingleOrDefault();
                if(BinaryEncoder.ReadInt(stream) == 0)
                {
                    key.Trusted = true;
                }
                else
                {
                    Log("Receiving new key.");
                    var aesKey = BinaryEncoder.ReceiveBytes(stream);
                    if (key != null)
                    {
                        Log("Updating!");
                        key.AesKey = aesKey;
                        key.Trusted = true;
                    }
                    else
                    {
                        key = new UsersKeys()
                        {
                            RecepientId = recepientId,
                            SenderId = user.UserId,
                            AesKey = aesKey,
                            Trusted = true
                        };
                        context.Add(key);
                    }
                }
                context.SaveChanges();
            }
            Log("Trust contact done.");
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
