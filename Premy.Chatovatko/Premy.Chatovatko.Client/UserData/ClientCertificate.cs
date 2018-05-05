using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.UserData
{
    public static class ClientCertificate
    {
        public static X509Certificate2 theCert;
        public static X509CertificateCollection clientCertificateCollection;
    }
}
