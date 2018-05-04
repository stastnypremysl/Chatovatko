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
                serverCertificate = new X509Certificate2(Config.certAddress, Config.certPasswd);
                if (!serverCertificate.HasPrivateKey)
                {
                    throw new Exception("Certificate has no private key.");
                }
            }
            catch(Exception ex)
            {
                Logger.LogClientListenerError(ex.Message);
                throw ex;
            }
            Logger.LogClientListenerInfo(String.Format("Certificate has been successfully loaded."));
        }
    }
}
