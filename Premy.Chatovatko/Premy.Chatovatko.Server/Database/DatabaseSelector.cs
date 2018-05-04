using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public static class DatabaseSelector
    {
        public static String GetDatabaseAddress()
        {
            return Config.connectionString;
        }
    }
}
