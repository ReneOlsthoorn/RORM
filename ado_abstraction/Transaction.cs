using System;

namespace RORM
{
    public class Transaction
    {
        public virtual void Commit() => throw new NotImplementedException();
        public virtual void Rollback() => throw new NotImplementedException();
    }
}
