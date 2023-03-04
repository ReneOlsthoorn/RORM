using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace RORM
{
    public class Parameter
    {
        public string ParameterName { get; set; }
        public string ColumnName { get; set; } // when changing a parameter to JSON, we must know what column it is.
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

        public virtual string EnsureValidColumnName(string columnName)
        {
            return columnName;
        }
    }
}
