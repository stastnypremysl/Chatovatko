using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.ClientCommunication
{
    public class InitConnectionVerificator : IConnectionVerificator
    {
        private readonly String acceptedPublicKey;

        public InitConnectionVerificator(String acceptedPublicKey)
        {
            this.acceptedPublicKey = acceptedPublicKey;
        }
        public bool AppCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors.Equals(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                return false;
            }
        }
    }
}
