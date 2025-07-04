﻿using System;
using System.Collections.Generic;
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

        public override void Close() => Command.Dispose();

        public override Connection Connection
        {
            set => Command.Connection = ((ConnectionPostgreSQL)value).Connection;
        }

        public override Transaction Transaction
        {
            set => Command.Transaction = ((TransactionPostgreSQL)value).Transaction;
        }

        public override string CommandText
        {
            get => Command.CommandText;
            set => Command.CommandText = value;
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
                    param.ParameterName = $"{pCounter++}";
                    Command.Parameters.Add(param);
                }
            }
        }

        public override void ClearParameters() => Command.Parameters.Clear();
        public override async Task<object> ExecuteScalarAsync() => await Command.ExecuteScalarAsync();
        public override async Task<int> ExecuteNonQueryAsync() => await Command.ExecuteNonQueryAsync();

        public override async Task<DataReader> ExecuteReaderAsync()
        {
            Command.CommandTimeout = RORM.Command.Timeout;
            DataReaderPostgreSQL reader = new DataReaderPostgreSQL();
            reader.datareader = await Command.ExecuteReaderAsync();
            return reader;
        }

        public override Dictionary<string, string> GetParameterDictForLogging()
        {
            Dictionary<string, string> result = new();
            foreach (NpgsqlParameter param in Command.Parameters)
            {
                string theValue = $"@{param.ParameterName}";
                if (param.Value == null)
                    theValue = "NULL";
                else if (param.Value is string)
                    theValue = $"'{param.Value.ToString()}'";
                else if (param.Value is bool boolObj)
                    theValue = (boolObj == true) ? "true" : "false";
                else if (param.Value is DateTime dtObj)
                    theValue = $"'{dtObj.ToString("o")}'::timestamp";
                else
                    theValue = Convert.ToString(param.Value);

                result[$"@{param.ParameterName}"] = theValue;
            }
            return result;
        }
    }
}
