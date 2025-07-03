using System;
using System.Threading.Tasks;
using Npgsql;

namespace RORM
{
    public class DataReaderPostgreSQL : DataReader
    {
        public NpgsqlDataReader datareader { get; set; }

        public DataReaderPostgreSQL() { }

        public override async Task<bool> ReadAsync() => await this.datareader.ReadAsync();
        public override async Task CloseAsync()
        {
            await datareader.CloseAsync();
            datareader.Dispose();
        }
        public override int FieldCount() => this.datareader.FieldCount;
        public override string GetName(int index) => datareader.GetName(index)?.ToLowerInvariant() ?? "_unknown";

        public override object this[string kolom]
        {
            get => datareader[kolom];
        }

        public override object this[int index]
        {
            get => datareader[index];
        }
        public override short GetInt16(int index) => datareader.GetInt16(index);
        public override int GetInt32(int index) =>  datareader.GetInt32(index);
        public override long GetInt64(int index) => datareader.GetInt64(index);
        public override string GetString(int index) => datareader.GetString(index);
        public override bool GetBoolean(int index) => datareader.GetBoolean(index);
        public override DateTime GetDateTime(int index) => datareader.GetDateTime(index);
        public override decimal GetDecimal(int index) => datareader.GetDecimal(index);
        public override double GetDouble(int index) => datareader.GetDouble(index);
    }
}
