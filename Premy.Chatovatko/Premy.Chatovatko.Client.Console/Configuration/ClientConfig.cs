using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Configuration
{
/*    public class ClientConfig
    {
        private String serverAddress = null;
        private String serverName = null;
        private String databaseAddress = null;

        public string DatabaseAddress { get => databaseAddress; set => databaseAddress = value; }
        public string ServerName { get => serverName; set => serverName = value; }
        public string ServerAddress { get => serverAddress; set => serverAddress = value; }

        public void Load()
        {
            if (!Directory.Exists(Utils.GetConfigDirectory()))
            {
                Logger.LogUserData("Configuration folder doesn't exist. Time to create it.");
                Directory.CreateDirectory(Utils.GetConfigDirectory());
            }

            if (!File.Exists(Utils.GetClientCertificate()))
            {
                Logger.LogUserData("Client certificate doesn't exist. I'm coping default one.");
                File.Copy("./DefaultConfig/client.crt", Utils.GetClientCertificate());
            }

            if (!File.Exists(Utils.GetConfigFile()))
            {
                Logger.LogUserData("Config file doesn't exist. I'm coping default one.");
                File.Copy("./DefaultConfig/config.txt", Utils.GetConfigFile());
            }

            Logger.LogUserData("Trying to load user config.");
            LoadConfig();

            Logger.LogUserData("Trying to load client certificate.");
            ClientCertificate.theCert = new X509Certificate2(Utils.GetClientCertificate());
            ClientCertificate.clientCertificateCollection = new X509CertificateCollection(new X509Certificate[] { ClientCertificate.theCert });

            Logger.LogUserData("Done!");

        }

        private void LoadConfig()
        {
            using (StreamReader sr = new StreamReader(Utils.GetConfigFile()))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] parts = line.Split('=');
                    String name = parts[0];
                    String rest = "";
                    for (int i = 1; i != parts.Length; i++)
                    {
                        rest += parts[i];
                        if (i != parts.Length - 1)
                        {
                            rest += '=';
                        }
                    }

                    switch (parts[0])
                    {
                        case "ServerAddress":
                            ServerAddress = rest;
                            break;
                        case "ServerName":
                            ServerName = rest;
                            break;
                        default:
                            Logger.LogUserData(String.Format("The parameter {0} doesn't exist.", name));
                            break;
                    }
                }
            }
            if(ServerAddress == null)
            {
                throw new Exception("ServerAddress parameter missing.");
            }
            if (ServerName == null)
            {
                throw new Exception("ServerName parameter missing.");
            }
        }
    }*/
}
