﻿using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.ClientCommunication
{
    public class InitConnectionVerificator : IConnectionVerificator, ILoggable
    {
        private readonly String acceptedPublicKey;
        private readonly Logger logger;

        public InitConnectionVerificator(Logger logger, String acceptedPublicKey)
        {
            this.acceptedPublicKey = acceptedPublicKey;
            this.logger = logger;
        }
        public bool AppCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors.Equals(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                logger.Log(this, "Remote certificate not available.", true);
                return false;
            }
            if (!certificate.GetPublicKeyString().Equals(acceptedPublicKey))
            {
                logger.Log(this, $"Remote certificate has other public key.\n{certificate.GetPublicKeyString()}", true);
                return false;
            }
            return true;
        }

        public string GetLogSource()
        {
            return "Certification verificator";
        }
    }
}