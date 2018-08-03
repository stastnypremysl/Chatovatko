using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels
{
    public class ServerInfo
    {
        public ServerInfo(String name, String publicKey)
        {
            this.Name = name;
            this.PublicKey = publicKey;
        }
        public String Name { get; set; }
        public String PublicKey { get; set; }
    }
}
