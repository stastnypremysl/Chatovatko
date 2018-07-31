using Premy.Chatovatko.Client.UserData;
using Premy.Chatovatko.Libs;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Comunication
{
    public class ClientConnection
    {
        private readonly int ServerPort = 8471;
        private int dataPort = -1;
        private TcpClient client;
        private TcpClient dataClient;

        private SslStream stream;
        private SslStream dataStream;

        public void Connect()
        {
            client = new TcpClient(Config.serverAddress, ServerPort);
            Logger.LogConnection("Client connected.");

            stream = new SslStream(client.GetStream(), false, App_CertificateValidation);
            stream.AuthenticateAsClient(Config.serverName, ClientCertificate.clientCertificateCollection, SslProtocols.Tls12, false);
            Logger.LogConnection("SSL authentication completed.");

            dataPort = Int32.Parse(TextEncoder.ReadStringFromSSLStream(stream));
            dataClient = new TcpClient(Config.serverAddress, dataPort);
            dataStream = new SslStream(dataClient.GetStream(), false, App_CertificateValidation);
            dataStream.AuthenticateAsClient(Config.serverName, ClientCertificate.clientCertificateCollection, SslProtocols.Tls12, false);

            Logger.LogConnection("Data connection has been successfully estamblished.");
        }

        private bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; }
            Logger.LogConnection("*** SSL Error: " + sslPolicyErrors.ToString());
            return false;
        }
    }
}
