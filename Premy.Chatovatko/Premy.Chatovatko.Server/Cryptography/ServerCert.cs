using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Server.Cryptography
{
    public class ServerCert : ILoggable
    {
        private X509Certificate2 serverCertificate;

        public X509Certificate2 ServerCertificate { get => serverCertificate; set => serverCertificate = value; }

        public string GetLogSource()
        {
            return "Server certificate";
        }

        public void Load(ServerConfig config)
        {
            ServerCertificate = new X509Certificate2(config.CertAddress, config.CertPasswd);
            if (!ServerCertificate.HasPrivateKey)
            {
                throw new ChatovatkoException(this, "Certificate has no private key.");

            }
        }
    }
}
