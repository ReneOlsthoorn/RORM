using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RORM
{
    public class Connector
    {
        public Connection Connection { get; set; }
        public Action<string> Logger = null;
        
        public void Log(string log)
        {
            if (Logger != null)
                Logger(log);
        }

        public void Open()
        {
            if (Connection != null)
                Connection.Open();
        }

        public void Close()
        {
            if (Connection != null)
                Connection.Close();
        }

        public virtual Connection NewConnection(string connectionString) => throw new NotImplementedException();
        public virtual Parameter NewParameter() => throw new NotImplementedException();
        public virtual Command NewCommand() => throw new NotImplementedException();
        public virtual Task<object> GetAutoincrementValue(Transaction transaction, string currVal) => throw new NotImplementedException();

        public Command NewCommand(string sql)
        {
        	Command cmd = NewCommand();
        	cmd.CommandText = sql;
        	return cmd;
        }

        public dynamic NewRow(string tableName = null)
        {
            DynaRow newDynaRow = new DynaRow(tableName);
            newDynaRow._row.Connector = this;
            return newDynaRow;
        }

        public virtual Parameter GetParameter(object dataValue = null, string dataKey = null, int? teller = null)
        {
            Parameter parameter = this.NewParameter();
            parameter.Value = dataValue;
            parameter.ParameterName = null;
            if (dataKey != null)
                parameter.ColumnName = dataKey;

            return parameter;
        }

        public async Task<object> GetAutoincrementValue_helper(Transaction transaction, string identitySelectStatement)
        {
            Command cmd = this.NewCommand();
            cmd.Connection = this.Connection;
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.CommandText = identitySelectStatement;
            if (this.Connection.Connector.Logger != null)
            {
                if (identitySelectStatement.EndsWith(";"))
                    this.Connection.Connector.Log(identitySelectStatement);
                else
                    this.Connection.Connector.Log(string.Format("{0};", identitySelectStatement));
            }
            object result = await cmd.ExecuteScalarAsync();
            if (Convert.IsDBNull(result) || (result == null))
                return null;

            return result;
        }
        
        private void AdhocQueryLogging(string sql, Command cmd = null)
        {
            if (this.Connection.Connector.Logger == null)
                return;

            string strTolog = sql;
            if (!sql.EndsWith(";"))
                strTolog = sql + ";";

            if (cmd != null)
            {
                Dictionary<string, string> strDict = cmd.GetParameterDictForLogging();
                foreach (string key in strDict.Keys)
                    strTolog = strTolog.Replace(key, strDict[key]);
            }

            this.Connection.Connector.Log(strTolog);     	
        }

        public string DetermineTablename(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return null;

            string singleLineSQL = Regex.Replace(sql, "[\n\r\x20]+", " ");
            MatchCollection coll = Regex.Matches(singleLineSQL, "[\x20]+FROM[\x20]+([\"A-Z0-9_]+)", RegexOptions.IgnoreCase);
            if (coll.Count != 1)
                return null; // Minder of meer dan één tabel mag niet.

            foreach (Match m in coll)
                if (m.Success)
                    return m.Groups[1].Value;

            return null;
        }

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
        {
            return ExecuteNonQueryAsync(sql, transaction: null, parameters: parameters);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, Transaction transaction, params object[] parameters)
        {
            Command cmd = this.NewCommand();
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.Connection = this.Connection;
            cmd.CommandText = sql;
            if (parameters != null)
                cmd.AddParameters(parameters);

            AdhocQueryLogging(sql, cmd);

            int nrRowsAffected = await cmd.ExecuteNonQueryAsync();
            cmd.ClearParameters();
            cmd.Close();

            return nrRowsAffected;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, List<Parameter> parameters = null, Transaction transaction = null)
        {
            Command cmd = this.NewCommand();
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.Connection = this.Connection;
            cmd.CommandText = sql;
            if (parameters != null)
                cmd.AddHoParameters(parameters);

            AdhocQueryLogging(sql, cmd);

            int nrRowsAffected = await cmd.ExecuteNonQueryAsync();
            cmd.ClearParameters();
            cmd.Close();

            return nrRowsAffected;
        }

        public IAsyncEnumerable<dynamic> ExecuteQueryAsync(string sql, params object[] parameters)
        {
            return ExecuteQueryAsync(sql, transaction: null, parameters);
        }

        public IAsyncEnumerable<dynamic> ExecuteQueryAsync(string sql, Transaction transaction, params object[] parameters)
        {
            Command cmd = this.NewCommand();
            if (transaction != null)
                cmd.Transaction = transaction;

            return ExecuteQueryAsync(sql, cmd, parameters);
        }

        public async IAsyncEnumerable<dynamic> ExecuteQueryAsync(string sql, Command cmd, params object[] parameters)
        {
            cmd.Connection = this.Connection;
            cmd.CommandText = sql;
            if (parameters != null)
                cmd.AddParameters(parameters);

            AdhocQueryLogging(sql, cmd);

            string tableName = DetermineTablename(sql);
            DataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync())
            {
                DynaRow newDynaRow = new DynaRow(tableName);
                newDynaRow._row.Connector = this;

                for (int i = 0; i < reader.FieldCount(); i++)
                {
                    object waarde = reader[i];
                    if (Convert.IsDBNull(waarde))
                        waarde = null;

                    newDynaRow[reader.GetName(i)] = waarde;
                }
                newDynaRow._row.JustLoadedFromDatabase();
                yield return newDynaRow;
            }
            await reader.CloseAsync();
            cmd.ClearParameters();
            cmd.Close();
        }

        public async Task<List<Dictionary<string, object>>> GetDictionaryListAsync(string sqlStatement, params object[] parameters)
        {
            var result = new List<Dictionary<string, object>>();
            await foreach (var row in ExecuteQueryAsync(sqlStatement, parameters))
                result.Add(row.GetDictionary());

            return result;
        }

        public async Task<List<dynamic>> GetDynamicListAsync(string sqlStatement, params object[] parameters)
        {
            var result = new List<dynamic>();
            await foreach (var row in ExecuteQueryAsync(sqlStatement, parameters))
                result.Add(row);

            return result;
        }


    }
}
