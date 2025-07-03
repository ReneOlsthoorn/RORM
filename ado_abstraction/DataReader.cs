using System;
using System.Threading.Tasks;

namespace RORM
{
    public class DataReader
    {
        public virtual Task<bool> ReadAsync() => throw new NotImplementedException();
        public virtual Task CloseAsync() => throw new NotImplementedException();

        public virtual object this[string kolom]
        {
            get => throw new NotImplementedException();
        }

        public virtual object this[int index]
        {
            get => throw new NotImplementedException();
        }

        public virtual int RecordsAffected
        {
            get => throw new NotImplementedException();
        }

        public virtual short GetInt16(int index) => throw new NotImplementedException();
        public virtual int GetInt32(int index) => throw new NotImplementedException();
        public virtual long GetInt64(int index) => throw new NotImplementedException();
        public virtual string GetString(int index) => throw new NotImplementedException();
        public virtual bool GetBoolean(int index) => throw new NotImplementedException();
        public virtual DateTime GetDateTime(int index) => throw new NotImplementedException();
        public virtual decimal GetDecimal(int index) => throw new NotImplementedException();
        public virtual double GetDouble(int index) => throw new NotImplementedException();
        /*
        public virtual byte[] GetByteArray(int index)
        {
        	throw new NotImplementedException();
        }
        */

        public int? GetNullableInt32(int index)
        {
            var value = this[index];
            if (!Convert.IsDBNull(value))
                return GetInt32(index);

            return null;
        }

        public string GetNullableString(int index)
        {
            var value = this[index];
            if (!Convert.IsDBNull(value))
                return GetString(index);

            return null;
        }

        public virtual int FieldCount() => throw new NotImplementedException();
        public virtual string GetName(int index) => throw new NotImplementedException();

    }
}
