using Premy.Chatovatko.Client.UserData;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Comunication
{
    public static class Connection
    {
        private static readonly int ServerPort = 8471;
        private static TcpClient client;
        private static SslStream stream;

        public static void Connect()
        {
            client = new TcpClient(Config.serverAddress, ServerPort);
            Logger.LogConnection("Client connected.");

            stream = new SslStream(client.GetStream(), false, App_CertificateValidation);
            stream.AuthenticateAsClient(Config.serverName, ClientCertificate.clientCertificateCollection, SslProtocols.Tls12, false);
            Logger.LogConnection("SSL authentication completed.");
        }

        private static bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; }
            Logger.LogConnection("*** SSL Error: " + sslPolicyErrors.ToString());
            return false;
        }
    }
}
