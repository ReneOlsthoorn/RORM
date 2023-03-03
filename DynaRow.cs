using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RORM
{
    public class DynaRow : DynamicObject
    {
        public Dictionary<string, object> _piggybag = null;
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
            return _row.Update(transaction).GetAwaiter().GetResult();
        }
        public async Task<int> UpdateAsync(Transaction transaction = null)
        {
            return await _row.Update(transaction);
        }

        public int Delete(Transaction transaction = null)
        {
            return _row.Delete(transaction).GetAwaiter().GetResult();
        }
        public async Task<int> DeleteAsync(Transaction transaction = null)
        {
            return await _row.Delete(transaction);
        }

        public int Insert(string table = null, string autoincrement = null, Transaction transaction = null)
        {
            return _row.Insert(table, autoincrement, transaction).GetAwaiter().GetResult();
        }
        public async Task<int> InsertAsync(string table = null, string autoincrement = null, Transaction transaction = null)
        {
            return await _row.Insert(table, autoincrement, transaction);
        }

        public void SetPiggyBagProperty(string key, object value)
        {
            if (_piggybag == null)
            {
                _piggybag = new Dictionary<string, object>();
            }
            _piggybag[key] = value;
        }

        public object GetPiggyBagProperty(string key)
        {
            if ((_piggybag == null) || (!_piggybag.ContainsKey(key)))
            {
                return null;
            }
            return _piggybag[key];
        }

        public Dictionary<string, object> GetDictionary()
        {
            return _row.Data;
        }
    }
}
