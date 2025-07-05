using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace RORM
{
    public class DynaRow : DynamicObject
    {
        public Dictionary<string, object?> Properties = new Dictionary<string, object?>();
        public DynaRowHelper _row = null;
        public DynaRow(string tableName = null)
        {
            _row = new DynaRowHelper(tableName);
        }

        public object this[string kolom]
        {
            set { _row.Change(kolom, value); } /* Als je geen Change wilt gebruiken, ga je maar regelrecht naar de Data hashtable toe */
            get { return _row.Data[kolom]; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string kolom = binder.Name;
            return _row.Data.TryGetValue(kolom, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string kolom = binder.Name;
            _row.Change(kolom, value);
            return true;
        }

        public int Update(Transaction transaction = null)
        {
            return _row.UpdateAsync(transaction).GetAwaiter().GetResult();
        }
        public async Task<int> UpdateAsync(Transaction transaction = null)
        {
            return await _row.UpdateAsync(transaction);
        }

        public DynaRowHelper helper { get => _row; }

        public int Delete(Transaction transaction = null)
        {
            return _row.DeleteAsync(transaction).GetAwaiter().GetResult();
        }
        public async Task<int> DeleteAsync(Transaction transaction = null)
        {
            return await _row.DeleteAsync(transaction);
        }

        public int Insert(string table = null, string autoincrement = null, Transaction transaction = null)
        {
            return _row.InsertAsync(table, autoincrement, transaction).GetAwaiter().GetResult();
        }
        public async Task<int> InsertAsync(string table = null, string autoincrement = null, Transaction transaction = null)
        {
            return await _row.InsertAsync(table, autoincrement, transaction);
        }

        public Dictionary<string, object> GetDictionary() => _row.Data;
    }
}
