using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Premy.Chatovatko.Server.Logging;

namespace Premy.Chatovatko.Server.Database
{
    public class DBConnection
    {
        private ulong id;
        private MySqlConnection theConnection;

        public DBConnection()
        {
            id = IDGenerator.getNext();

            theConnection = new MySqlConnection(DatabaseSelector.GetDatabaseAddress());
            theConnection.Open();
            Logger.LogConnectionInfo(id, "The connection has been opened");
        }

        ~DBConnection()
        {
            theConnection.Close();
            Logger.LogConnectionInfo(id, "The connection has been closed");
        }

        public ulong Id { get => id; set => id = value; }
        public MySqlConnection TheConnection { get => theConnection; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
