using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public static class DBPool
    {
        public static DBConnection GetConnection()
        {
            return new DBConnection();
        }
    }
}
