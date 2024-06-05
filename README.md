# RORM
Object Relational Mapper for C# dotnet
'''
/*
CREATE TABLE public.testtable (
	id bigserial NOT NULL,
	jsondata jsonb NULL
);
*/

using Npgsql;
using NpgsqlTypes;
using RORM;

string pgConnStr = "Host=localhost;Username=postgres;Password=x;Database=x";

RORM.ConnectorPostgreSQL pgConn = new RORM.ConnectorPostgreSQL();
pgConn.Connection = pgConn.NewConnection(pgConnStr);
pgConn.Open();

dynamic row = pgConn.NewRow();
((DynaRowHelper)(row._row)).OnAfterParameterCreation = JsonParameterCorrection;
row.jsondata = """
{ "yoho":"werwer" }
""";
row.Insert(table: "testtable", autoincrement: "id;testtable_id_seq");


var dynamicList = await pgConn.GetDynamicListAsync($"select * from testtable where id = @1", 1);
foreach (var item in dynamicList)
{
    Console.WriteLine(item.jsondata);
}

pgConn.Close();

void JsonParameterCorrection(string table, List<RORM.Parameter> parameters)
{
    foreach (var p in parameters)
    {
        NpgsqlParameter param = ((RORM.ParameterPostgreSQL)p).parameter;
        if (table == "testtable" && p.ColumnName == "jsondata")
        {
            param.NpgsqlDbType = NpgsqlDbType.Jsonb;
        }
    }
}
'''
