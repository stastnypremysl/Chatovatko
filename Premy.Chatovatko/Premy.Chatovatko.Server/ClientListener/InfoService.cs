using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class InfoService : ILoggable
    {
        private readonly Logger logger;
        private readonly ServerConfig config;
        private readonly ServerCert cert;

        public InfoService(Logger logger, ServerConfig config, ServerCert cert)
        {
            this.logger = logger;
            this.config = config;
            this.cert = cert;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    logger.Log(this, "I have booted up.");
                    TcpListener listener = new TcpListener(IPAddress.Any, TcpConstants.INFO_SERVER_PORT);
                    listener.Start();
                    while (true)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        logger.Log(this, String.Format("Client {0} has connected.",
                            Utils.GetIpAddress(client)));
                        Task.Run(() =>
                        {
                            try
                            {
                                using (Stream stream = client.GetStream()) {
                                    TextEncoder.SendJson(stream, GetServerInfo());
                                }
                                
                            }
                            catch (Exception exception)
                            {
                                logger.LogException(this, exception);
                            }
                            finally
                            {
                                client.Dispose();
                            }
                            
                        });
                    }
                }
                catch (Exception exception)
                {
                    logger.LogException(this, exception);
                }
            }
        }

        public ServerInfo GetServerInfo()
        {
            String publicKey = cert.ServerCertificate.GetPublicKeyString();
            return new ServerInfo(config.ServerName, publicKey);
        }

        public void RunInBackground()
        {
            Task.Run(() => Run());

        }

        public string GetLogSource()
        {
            return "Information tcp service";
        }
    }
}
