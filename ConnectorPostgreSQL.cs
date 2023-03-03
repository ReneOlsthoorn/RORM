using System;
using System.Collections.Generic;
using System.Text;

namespace RORM
{
    public class ConnectorPostgreSQL : Connector
    {
        public override Connection NewConnection(string connectionString)
        {
            ConnectionPostgreSQL result = new ConnectionPostgreSQL();
            result.TheConnectionString = connectionString;
            result.Connection.ConnectionString = connectionString;
            result.Connector = this as Connector;
            return (Connection)result;
        }

        public override Parameter NewParameter()
        {
            ParameterPostgreSQL result = new ParameterPostgreSQL();
            return (Parameter)result;
        }

        public override Command NewCommand()
        {
            CommandPostgreSQL result = new CommandPostgreSQL();
            result.Connector = this as Connector;
            result.Connection = result.Connector.Connection;
            return (Command)result;
        }

        public override object GetAutoincrementValue(Transaction transaction, string currVal)
        {
            return GetAutoincrementValue_helper(transaction, $"select currval('{currVal}')");
        }

    }
}
