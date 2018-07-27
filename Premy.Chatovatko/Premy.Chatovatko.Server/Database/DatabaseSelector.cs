using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public class DatabaseSelector
    {
        public String GetDatabaseAddress()
        {
            return ServerConfig.connectionString;
        }
    }
}
