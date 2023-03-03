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


        public override string Type
        {
            get
            {
                return parameter.NpgsqlDbType.ToString();
            }
            set
            {
                Dictionary<string, NpgsqlDbType> searchDict = new Dictionary<string, NpgsqlDbType>();
                searchDict.Add("float", NpgsqlDbType.Real);
                searchDict.Add("varchar", NpgsqlDbType.Varchar);
                searchDict.Add("char", NpgsqlDbType.Char);
                searchDict.Add("datetime", NpgsqlDbType.Date);
                searchDict.Add("int", NpgsqlDbType.Integer);
                searchDict.Add("numeric", NpgsqlDbType.Numeric);
                searchDict.Add("text", NpgsqlDbType.Text);

                if (!searchDict.ContainsKey(value))
                {
                    throw new Exception("Unknown datatype");
                }
                parameter.NpgsqlDbType = searchDict[value];
            }
        }

        
    }
}
