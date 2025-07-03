using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RORM
{
    public class Command
    {
        public static int Timeout = 300;
        public Connector Connector { get; set; }
        public string LogSQLStatement { get; set; }

        public virtual Connection Connection
        {
            set
            {
                throw new NotImplementedException();
            }
        }
        public virtual Transaction Transaction
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual System.Data.CommandType CommandType { get; set; }
        public virtual string CommandText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual void AddHoParameters(List<Parameter> paramList)
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, string> GetParameterDictForLogging()
        {
            throw new NotImplementedException();
        }

        public virtual void AddParameters(params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    this.AddParameter(Connector.GetParameter(parameter));
                }
            }
        }

        public virtual void AddParameter(Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public virtual void ClearParameters()
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
            throw new NotImplementedException();
        }

        public virtual Task<object> ExecuteScalarAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task<DataReader> ExecuteReaderAsync() {
            throw new NotImplementedException();
        }

        public virtual Task<int> ExecuteNonQueryAsync()
        {
            throw new NotImplementedException();
        }

    }
}
