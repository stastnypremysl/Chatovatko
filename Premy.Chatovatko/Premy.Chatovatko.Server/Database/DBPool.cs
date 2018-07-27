using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public class DBPool
    {
        public DBConnection GetConnection()
        {
            return new DBConnection();
        }

        public static void Init()
        {

        }
    }
}
