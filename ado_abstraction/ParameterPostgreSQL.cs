using System;
using Npgsql;

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
            get => parameter.Value;
            set
            {
                if (value == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = value;
            }
        }
        
        public override string EnsureValidColumnName(string columnName) => $"\"{columnName}\"";
    }
}
