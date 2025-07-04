﻿using System.Threading.Tasks;

namespace RORM
{
    public class ConnectorPostgreSQL : Connector
    {
        public override Connection NewConnection(string connectionString)
        {
            ConnectionPostgreSQL result = new ConnectionPostgreSQL();
            result.TheConnectionString = connectionString;
            result.Connection.ConnectionString = connectionString;
            result.Connector = this;
            return result;
        }

        public override Parameter NewParameter() => new ParameterPostgreSQL();

        public override Command NewCommand()
        {
            ConnectorPostgreSQL pgConnector = this;
            ConnectionPostgreSQL pgConnection = this.Connection as ConnectionPostgreSQL;
            if (pgConnection.Connection.State != System.Data.ConnectionState.Open)
                pgConnection.Connection.Open();

            CommandPostgreSQL result = new CommandPostgreSQL();
            result.Connector = pgConnector;
            result.Connection = result.Connector.Connection;

            return (Command)result;
        }

        public async override Task<object> GetAutoincrementValue(Transaction transaction, string currVal)
        {
            return await GetAutoincrementValue_helper(transaction, $"select currval('{currVal}')");
        }

    }
}
