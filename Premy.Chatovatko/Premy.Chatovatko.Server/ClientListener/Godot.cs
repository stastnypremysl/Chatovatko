using MySql.Data.MySqlClient;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.ClientListener.Scenarios;
using Premy.Chatovatko.Server.Database;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private List<int> userIdsUploaded;

        /// <summary>
        /// Already uploaded blob messages.
        /// </summary>
        private List<int> messagesIdsUploaded;

        /// <summary>
        /// Already uploaded aes keys.
        /// </summary>
        private List<int> aesKesUserIdsUploaded;


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

                        case ConnectionCommand.SEND_AES_KEY:
                            Log("SEND_AES_KEY command received.");
                            ReceiveAesKey();
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

            Log("Downloading done.");
        }

        private void Pull()
        {

        }

        private void Push()
        {

        }

        private void ReceiveAesKey()
        {

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
