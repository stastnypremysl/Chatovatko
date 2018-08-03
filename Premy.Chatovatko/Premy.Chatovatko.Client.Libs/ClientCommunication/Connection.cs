using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Comunication
{
    public class ClientConnection : ILoggable
    {
        private readonly int ServerPort = TcpConstants.MAIN_SERVER_PORT;
        private int dataPort = -1;
        private TcpClient client;
        private TcpClient dataClient;

        private SslStream stream;
        private SslStream dataStream;
        private Logger logger;

        public ClientConnection(Logger logger)
        {
            this.logger = logger;
        }

        public void Connect()
        {
            /*
            client = new TcpClient(Config.serverAddress, ServerPort);
            logger.Log(this,"Client connected.");

            stream = new SslStream(client.GetStream(), false, App_CertificateValidation);
            stream.AuthenticateAsClient(Config.serverName, ClientCertificate.clientCertificateCollection, SslProtocols.Tls12, false);
            logger.Log(this, "SSL authentication completed.");

            dataPort = Int32.Parse(TextEncoder.ReadStringFromSSLStream(stream));
            dataClient = new TcpClient(Config.serverAddress, dataPort);
            dataStream = new SslStream(dataClient.GetStream(), false, App_CertificateValidation);
            dataStream.AuthenticateAsClient(Config.serverName, ClientCertificate.clientCertificateCollection, SslProtocols.Tls12, false);
            */
            logger.Log(this, "Data connection has been successfully estamblished.");
        }

        public string GetLogSource()
        {
            return "Connection";
        }

        private bool AppCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if(sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                logger.Log(this, "*** SSL Error: " + sslPolicyErrors.ToString());
                return false;
            }
            
            return true;
        }
    }
}
