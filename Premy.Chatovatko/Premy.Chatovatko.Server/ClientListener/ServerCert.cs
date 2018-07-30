using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class ServerCert
    {
        public X509Certificate2 serverCertificate;
        public void Load(ServerConfig config)
        {
            serverCertificate = new X509Certificate2(config.certAddress, config.certPasswd);
            if (!serverCertificate.HasPrivateKey)
            {
                throw new Exception("Certificate has no private key.");

            }
        }
    }
}
