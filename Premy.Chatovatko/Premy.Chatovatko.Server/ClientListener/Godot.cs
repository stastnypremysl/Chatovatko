using MySql.Data.MySqlClient;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.Logging;
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
    public class Godot : IDisposable
    {

        private readonly ulong id;
        private MySqlConnection conn;
        private Thread theLife;
        private TcpClient client;
        TcpClient dataClient;
        private bool readyForLife = false;

        private SslStream sslStream;
        private SslStream dataStream;
        private TcpListener dataListener;

        private Logger logger;
        private readonly ServerCert serverCert;
        private readonly GodotCounter godotCounter;


        public Godot(ulong id, Logger logger, DBPool pool, ServerCert serverCert, GodotCounter godotCounter)
        {
            this.id = id;
            this.logger = logger;
            this.serverCert = serverCert;
            this.godotCounter = godotCounter;
            Thread initLife = new Thread(() => Init(pool));
            theLife = new Thread(() => MyJob());
            initLife.Start();
        }

        ~Godot()
        {
            Dispose();
        }


        private void Init(DBPool pool)
        {
            conn = pool.GetConnection();
            dataListener = new TcpListener(IPAddress.Any, 0);
            dataListener.Start();

            readyForLife = true;
            logger.Log(this, String.Format("Godot {0} has been created.", id));
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
                logger.Log(this, String.Format("Godot {0} has been activated.", id));
                godotCounter.IncreaseRunning();

                sslStream = new SslStream(client.GetStream(), false, App_CertificateValidation);
                sslStream.AuthenticateAsServer(serverCert.ServerCertificate, true, SslProtocols.Tls12, false);

                logger.Log(this, String.Format("Godot {0} is sending data connection port and waiting for connection.", id));

                TextEncoder.SendStringToSSLStream(sslStream, ((IPEndPoint)dataListener.LocalEndpoint).Port.ToString());
                dataClient = dataListener.AcceptTcpClient();
                logger.Log(this, String.Format("Godot {0}: Data connection initializing.", id));

                dataStream = new SslStream(dataClient.GetStream(), false, App_CertificateValidation);
                dataStream.AuthenticateAsServer(serverCert.ServerCertificate, true, SslProtocols.Tls12, true);

                logger.Log(this, String.Format("Godot {0}: Data connection has been successfully estamblished.", id));


            }
            catch(Exception ex)
            {
                logger.Log(this, String.Format("Godot {0} has crashed. Exception:\n{1}\n{2}", id, ex.Message, ex.StackTrace));
            }
            finally
            { 
                sslStream.Close();
                client.Close();
                dataListener.Stop();
                godotCounter.IncreaseDestroyed();
                Dispose();
                logger.Log(this, String.Format("Godot {0} has died.", id));
            }
        }

        private bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; }
            logger.Log(this, String.Format("Godot {0}: ***SSL Error: {1}", id, sslPolicyErrors.ToString()));
            return true;
        }


        public void Dispose()
        {
            conn.Dispose();
            sslStream.Dispose();
            client.Dispose();
        }
    }
}
