using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace RORM
{
    public class DataReaderPostgreSQL : DataReader
    {
        public NpgsqlDataReader datareader { get; set; }

        public DataReaderPostgreSQL()
        {
        }

        public override async Task<bool> ReadAsync()
        {
            return await datareader.ReadAsync();
        }

        public override async Task CloseAsync()
        {
            await datareader.CloseAsync();
            datareader.Dispose();
        }

        public override int FieldCount()
        {
            return datareader.FieldCount;
        }

        public override string GetName(int index)
        {
            return datareader.GetName(index)?.ToLowerInvariant() ?? "_unknown";
        }

        public override object this[string kolom]
        {
            get { return datareader[kolom]; }
        }

        public override object this[int index]
        {
            get { return datareader[index]; }
        }
        public override short GetInt16(int index)
        {
        	return datareader.GetInt16(index);
        }
        public override int GetInt32(int index)
        {
        	return datareader.GetInt32(index);
        }        
        public override long GetInt64(int index)
        {
        	return datareader.GetInt64(index);
        }
        public override string GetString(int index)
        {
        	return datareader.GetString(index);
        }
        public override bool GetBoolean(int index)
        {
        	return datareader.GetBoolean(index);
        }
        public override DateTime GetDateTime(int index)
        {
        	return datareader.GetDateTime(index);
        }
        public override decimal GetDecimal(int index)
        {
        	return datareader.GetDecimal(index);
        }
        public override double GetDouble(int index)
        {
        	return datareader.GetDouble(index);
        }        
    }
}
