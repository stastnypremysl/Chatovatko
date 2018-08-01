using Premy.Chatovatko.Client.Libs.UserData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public class DBInitializator
    {
        private readonly String databaseAddress;
        public DBInitializator(IClientDatabaseConfig config)
        {
            databaseAddress = config.DatabaseAddress;
        }

        public void DBDelete()
        {
            File.Delete(databaseAddress);
        }

        public void DBInit()
        {

        }

        public void DBGet()
        {

        }
    }
}
