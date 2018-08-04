using MySql.Data.MySqlClient;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.Cryptography;
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
    public class Godot : IDisposable, ILoggable
    {

        private readonly ulong id;
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

        private readonly ServerConfig config;


        public Godot(ulong id, Logger logger, ServerConfig config, ServerCert serverCert, GodotCounter godotCounter)
        {
            this.id = id;
            this.logger = logger;
            this.serverCert = serverCert;
            this.godotCounter = godotCounter;
            Thread initLife = new Thread(() => Init());
            theLife = new Thread(() => MyJob());
            initLife.Start();
        }

        ~Godot()
        {
            Dispose();
        }


        private void Init()
        {
            dataListener = new TcpListener(IPAddress.Any, 0);
            dataListener.Start();

            readyForLife = true;
            logger.Log(this, "Godot has been created.");
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
                logger.Log(this, String.Format("Godot has been activated. Client IP address is {0}", 
                    Utils.GetIpAddress(client)));
                godotCounter.IncreaseRunning();

                sslStream = new SslStream(client.GetStream(), false, CertificateValidation);
                sslStream.AuthenticateAsServer(serverCert.ServerCertificate, true, SslProtocols.Tls12, false);

                logger.Log(this, "Godot is sending data connection port and waiting for connection.");

                TextEncoder.SendString(sslStream, ((IPEndPoint)dataListener.LocalEndpoint).Port.ToString());
                dataClient = dataListener.AcceptTcpClient();
                logger.Log(this, "Data connection initializing.");

                dataStream = new SslStream(dataClient.GetStream(), false, CertificateValidation);
                dataStream.AuthenticateAsServer(serverCert.ServerCertificate, true, SslProtocols.Tls12, true);

                logger.Log(this, "Data connection has been successfully estamblished.");


            }
            catch(Exception ex)
            {
                logger.Log(this, String.Format("Godot has crashed. Exception:\n{0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            { 
                sslStream.Close();
                client.Close();
                dataListener.Stop();
                godotCounter.IncreaseDestroyed();
                Dispose();
                logger.Log(this, "Godot has died.");
            }
        }

        private bool CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        public void Dispose()
        {
            sslStream.Dispose();
            client.Dispose();
        }

        public string GetLogSource()
        {
            return String.Format("Godot {0}", id);
        }
    }
}
