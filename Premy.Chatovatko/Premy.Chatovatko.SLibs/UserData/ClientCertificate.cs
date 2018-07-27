using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.UserData
{
    public class ClientCertificate
    {
        public X509Certificate2 theCert;
        public X509CertificateCollection clientCertificateCollection;
    }
}
