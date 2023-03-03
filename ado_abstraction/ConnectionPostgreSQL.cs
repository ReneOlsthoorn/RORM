using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace RORM
{
    public class ConnectionPostgreSQL : Connection
    {
        public NpgsqlConnection Connection { get; set; }

        public ConnectionPostgreSQL()
        {
            Connection = new NpgsqlConnection();
        }

        public override void Open()
        {
            Connection.Open();
        }

        public override void Close()
        {
            Connection.Close();
        }

        public override Transaction BeginTransaction()
        {
            TransactionPostgreSQL transaction = new TransactionPostgreSQL();
            transaction.Transaction = Connection.BeginTransaction();
            return transaction as Transaction;
        }
    }
}
