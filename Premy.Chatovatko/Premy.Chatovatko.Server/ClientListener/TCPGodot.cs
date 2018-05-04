using Premy.Chatovatko.Server.Database;
using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class TCPGodot : IDisposable
    {

        private ulong id;
        private DBConnection connection;
        private Thread theLife;
        private TcpClient client;
        private bool readyForLife = false;

        private SslStream sslStream;

        public TCPGodot(ulong id)
        {
            this.id = id;
            Thread initLife = new Thread(() => Init());
            theLife = new Thread(() => MyJob());
            initLife.Start();
        }

        ~TCPGodot()
        {
            Dispose();
        }


        private void Init()
        {
            connection = DBPool.GetConnection();
            readyForLife = true;
            Logger.LogGodotInfo(id, "Godot has been created.");
        }

        public void Run(TcpClient client)
        {
            this.client = client;
            theLife.Start();
        }
        
        private void MyJob()
        {
            while (!readyForLife)
            {
                Thread.Sleep(50);
            }
            Logger.LogGodotInfo(id, "Godot has been activated.");
            GodotFountain.IncreaseRunning();

            sslStream = new SslStream(client.GetStream(), false, App_CertificateValidation);
            sslStream.AuthenticateAsServer(ServerCert.serverCertificate, true, SslProtocols.Tls12, true);

            var outputMessage = "Hello from the server.";
            var outputBuffer = Encoding.UTF8.GetBytes(outputMessage);
            sslStream.Write(outputBuffer);


            GodotFountain.IncreaseDestroyed();
            Dispose();
            Logger.LogGodotInfo(id, "Godot has died.");
        }

        private bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; } 
            Logger.LogGodotError(id, "*** SSL Error: " + sslPolicyErrors.ToString());
            return false;
        }


        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
