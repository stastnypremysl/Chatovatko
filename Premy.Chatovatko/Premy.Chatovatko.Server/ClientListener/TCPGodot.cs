using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Server.Database;
using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Net;
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
        TcpClient dataClient;
        private bool readyForLife = false;

        private SslStream sslStream;
        private SslStream dataStream;
        private TcpListener dataListener;


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
            dataListener = new TcpListener(IPAddress.Any, 0);
            dataListener.Start();

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
            try
            { 
                Logger.LogGodotInfo(id, "Godot has been activated.");
                GodotFountain.IncreaseRunning();

                sslStream = new SslStream(client.GetStream(), false, App_CertificateValidation);
                sslStream.AuthenticateAsServer(ServerCert.serverCertificate, true, SslProtocols.Tls12, false);


                Logger.LogGodotInfo(id, "Godot is sending data connection port and waiting for connection.");
                TextEncoder.SendStringToSSLStream(sslStream, ((IPEndPoint)dataListener.LocalEndpoint).Port.ToString());
                dataClient = dataListener.AcceptTcpClient();
                Logger.LogGodotInfo(id, "Data connection initializing.");

                dataStream = new SslStream(dataClient.GetStream(), false, App_CertificateValidation);
                dataStream.AuthenticateAsServer(ServerCert.serverCertificate, true, SslProtocols.Tls12, true);

                Logger.LogGodotInfo(id, "Data connection has been successfully estamblished.");


            }
            catch(Exception ex)
            {
                Logger.LogGodotError(id, String.Format("The godot has crashed. Exception:\n{0}", ex.Message));
            }
            finally
            { 
                sslStream.Close();
                client.Close();
                dataListener.Stop();
                GodotFountain.IncreaseDestroyed();
                Dispose();
                Logger.LogGodotInfo(id, "Godot has died.");
            }
        }

        private bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; }
            Logger.LogGodotError(id, "*** SSL Error: " + sslPolicyErrors.ToString());
            return true;
        }


        public void Dispose()
        {
            connection.Dispose();
            sslStream.Dispose();
            client.Dispose();
        }
    }
}
