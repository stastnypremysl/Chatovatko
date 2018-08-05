using Premy.Chatovatko.Client.Libs.ClientCommunication.Scenarios;
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

namespace Premy.Chatovatko.Client.Libs.ClientCommunication
{
    public class Connection : ILoggable
    {
        private readonly int ServerPort = TcpConstants.MAIN_SERVER_PORT;
        private TcpClient client;

        private SslStream stream;
        private Logger logger;

        private readonly String serverAddress;
        private readonly IConnectionVerificator verificator;
        private readonly X509Certificate2 x509;
        private readonly String userName;

        public Connection(Logger logger, IConnectionVerificator verificator, String serverAddress, X509Certificate2 x509, String userName = null)
        {
            this.logger = logger;
            this.verificator = verificator;
            this.serverAddress = serverAddress;
            this.x509 = x509;
            this.userName = userName;
        }

        public void Connect()
        {

            client = new TcpClient(serverAddress, ServerPort);
            logger.Log(this, "Client connected.");

            stream = new SslStream(client.GetStream(), false, verificator.AppCertificateValidation);
            X509CertificateCollection clientCertificates = new X509CertificateCollection();
            clientCertificates.Add(x509);

            stream.AuthenticateAsClient("Dummy", clientCertificates, SslProtocols.Tls12, false);
            logger.Log(this, "SSL authentication completed.");

            logger.Log(this, "Handshake started.");
            Handshake.Login(logger, stream, x509, userName);
            logger.Log(this, "Handshake successeded.");
        }

        public string GetLogSource()
        {
            return "Connection";
        }

        s
    }
}
