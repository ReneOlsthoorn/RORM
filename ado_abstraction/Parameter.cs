using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace RORM
{
    public class Parameter
    {

        public virtual System.Data.DbType DbType { get; set; }
        public virtual System.Data.ParameterDirection Direction { get; set; }
        public virtual string ParameterName { get; set; }
        public virtual object Value
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

        /* RORM uses parameters when executing sql. This is to prevent SQL injection.
         * We want the parameter to have the correct type, so we set it according to the Type dictionary,
         * which contains the native type (for most database interfaces). */
        public virtual string Type { get; set; }


        public virtual string ToSQLValueStringSpecific()
        {
            return null;
        }




    }
}
