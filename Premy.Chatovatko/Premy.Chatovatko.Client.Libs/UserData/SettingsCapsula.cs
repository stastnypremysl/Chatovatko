using Premy.Chatovatko.Client.Libs.Database.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.UserData
{
    public class SettingsCapsula
    {
        public Settings Settings { get; }
        public X509Certificate2 Certificate { get; }
        public long UserPublicId => Settings.UserPublicId;
        public string PrivateCertificate => Settings.PrivateCertificate;
        public string UserName => Settings.UserName;
        public string ServerName => Settings.ServerName;
        public string ServerAddress => Settings.ServerAddress;
        public string ServerPublicCertificate => Settings.ServerPublicCertificate;

        public SettingsCapsula(Settings settings)
        {
            Settings = settings;
            Certificate = new X509Certificate2(Convert.FromBase64String(settings.PrivateCertificate));
        }

        
    }
}
