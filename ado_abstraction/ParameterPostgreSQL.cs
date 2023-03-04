using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace RORM
{
    public class ParameterPostgreSQL : Parameter
    {
        public NpgsqlParameter parameter { get; set; }

        public ParameterPostgreSQL()
        {
            parameter = new NpgsqlParameter();
        }

        public override object Value
        {
            get
            {
                return parameter.Value;
            }
            set
            {
                if (value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.Value = value;
                }
            }
        }
        
        public override string EnsureValidColumnName(string columnName)
        {
            return $"\"{columnName}\"";
        }
    }
}
