using System;
using System.Collections.Generic;
using System.Text;

namespace RORM
{
    public class Connection
    {
        public Connector Connector { get; set; }
        public string TheConnectionString { get; set; }
        public string OpenConnectionCommands { get; set; }

        public virtual void Open()
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
            throw new NotImplementedException();
        }

        public virtual Transaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public virtual void EnsureStableConnection()
        {

        }

    }
}
