using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace RORM
{
    public class CommandPostgreSQL : Command
    {
        public NpgsqlCommand Command { get; set; }

        public CommandPostgreSQL()
        {
            Command = new NpgsqlCommand();
            Command.CommandTimeout = 999;
        }

        public override void Close()
        {
            Command.Dispose();
        }

        public override Connection Connection
        {
            set
            {
                Command.Connection = ((ConnectionPostgreSQL)value).Connection;
            }
        }

        public override Transaction Transaction
        {
            set
            {
                Command.Transaction = ((TransactionPostgreSQL)value).Transaction;
            }
        }

        public override string CommandText
        {
            get
            {
            	return Command.CommandText;
            }
            set
            {
                Command.CommandText = value;
            }
        }

        public override void AddParameters(params object[] parameters)
        {
            if (parameters != null)
            {
                int pCounter = 1;
                foreach (object prm in parameters)
                {
                    Parameter parameter = Connector.GetParameter(prm);
                    NpgsqlParameter param = ((ParameterPostgreSQL)parameter).parameter;
                    param.ParameterName = $"{pCounter++}";
                    Command.Parameters.Add(param);
                }
            }
        }

        public override void AddHoParameters(List<Parameter> parameters)
        {
            if (parameters != null)
            {
                int pCounter = 1;
                foreach (Parameter parameter in parameters)
                {
                    NpgsqlParameter param = ((ParameterPostgreSQL)parameter).parameter;
                    param.ParameterName = $"p{pCounter++}";
                    Command.Parameters.Add(param);
                }
            }
        }

        public override void ClearParameters()
        {
            Command.Parameters.Clear();
        }

        public override async Task<object> ExecuteScalarAsync()
        {
            return await Command.ExecuteScalarAsync();
        }

        public override async Task<DataReader> ExecuteReaderAsync()
        {
            Command.CommandTimeout = RORM.Command.Timeout;
            DataReaderPostgreSQL reader = new DataReaderPostgreSQL();
            reader.datareader = await Command.ExecuteReaderAsync();
            return reader as DataReader;
        }

        public override async Task<int> ExecuteNonQueryAsync()
        {
            return await Command.ExecuteNonQueryAsync();
        }
    }
}
