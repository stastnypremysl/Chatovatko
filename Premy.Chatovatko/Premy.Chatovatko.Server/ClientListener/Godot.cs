﻿using MySql.Data.MySqlClient;
using Premy.Chatovatko.Libs;
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
        private Thread theLife;
        private TcpClient client;
        private bool readyForLife = false;

        private SslStream sslStream;
        private TcpListener dataListener;

        private Logger logger;
        private readonly X509Certificate2 serverCert;
        private readonly GodotCounter godotCounter;

        private readonly ServerConfig config;


        public Godot(ulong id, Logger logger, ServerConfig config, X509Certificate2 serverCert, GodotCounter godotCounter)
        {
            this.id = id;
            this.logger = logger;
            this.serverCert = serverCert;
            this.godotCounter = godotCounter;
            theLife = new Thread(() => MyJob());
            Task.Run(() => Init());
        }
        
        private void Init()
        {
            //Expensive init operations insert here

            //-------------------------------------
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
                    LUtils.GetIpAddress(client)));
                godotCounter.IncreaseRunning();

                sslStream = new SslStream(client.GetStream(), false, CertificateValidation);
                sslStream.AuthenticateAsServer(serverCert, true, SslProtocols.Tls12, false);

                logger.Log(this, "SSL authentication completed. Starting Handshake.");
                UserCapsula user = Handshake.Run(sslStream, Log, config);
                


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
                logger.Log(this, "Godot has died.");
            }
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
