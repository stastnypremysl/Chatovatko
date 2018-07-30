using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Premy.Chatovatko.Server.Logging;

namespace Premy.Chatovatko.Server.Database
{
    public class DBConnection : IDisposable
    {
        private ulong id;
        private MySqlConnection theConnection;
        private IServerLogger logger;
        private ServerConfig config;
        private DatabaseIDGenerator idGenerator;

        public DBConnection(IServerLogger logger, ServerConfig config, DatabaseIDGenerator idGenerator)
        {
            this.logger = logger;
            this.config = config;
            id = idGenerator.getNext();

            try
            { 
                theConnection = new MySqlConnection(DatabaseSelector.GetDatabaseAddress());
                theConnection.Open();
                ConsoleServerLogger.LogConnectionInfo(id, "The connection has been opened.");
            }
            catch(Exception ex)
            {
                logger.LogConnectionInfo(id, "The connection can't be opened.");
                logger.LogConnectionError(id, ex.Message);
                throw ex;
            }
        }

        ~DBConnection()
        {
            Dispose();
        }

        public ulong Id { get => id; set => id = value; }
        public MySqlConnection TheConnection { get => theConnection; }

        public void Dispose()
        {
            theConnection.Close();
            theConnection.Dispose();
            ConsoleServerLogger.LogConnectionInfo(id, "The connection has been closed.");
        }

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
