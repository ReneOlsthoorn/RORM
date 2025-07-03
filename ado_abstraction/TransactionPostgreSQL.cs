using Npgsql;

namespace RORM
{
    public class TransactionPostgreSQL : Transaction
    {
        public NpgsqlTransaction Transaction { get; set; }
        public override void Commit() => Transaction.Commit();
        public override void Rollback() => Transaction.Rollback();
    }
}
