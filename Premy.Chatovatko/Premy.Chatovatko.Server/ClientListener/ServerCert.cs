using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Server.ClientListener
{
    public static class ServerCert
    {
        public static X509Certificate2 serverCertificate;
        public static void Load()
        {
            try
            { 
                serverCertificate = new X509Certificate2(ServerConfig.certAddress, ServerConfig.certPasswd);
                if (!serverCertificate.HasPrivateKey)
                {
                    throw new Exception("Certificate has no private key.");
                }
            }
            catch(Exception ex)
            {
                ConsoleServerLogger.LogClientListenerError(ex.Message);
                throw ex;
            }
            ConsoleServerLogger.LogClientListenerInfo(String.Format("Certificate has been successfully loaded."));
        }
    }
}
