using Premy.Chatovatko.Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public class UserCapsula
    {
        public UserCapsula(Users user, X509Certificate2 cert)
        {
            UserId = user.Id;
            UserName = user.UserName;
            Certificate = cert;
        }

        public int UserId { get; }
        public string UserName { get; }
        public X509Certificate2 Certificate {get; }
    }
}
