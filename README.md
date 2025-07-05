# RORM
Object Relational Mapper for C# dotnet
```
/*
CREATE TABLE public.testtable (
	id bigserial NOT NULL,
	jsondata jsonb NULL
);
*/

using NpgsqlTypes;
using RORM;

string pgConnStr = "Host=localhost;Username=postgres;Password=x;Database=x";

ConnectorPostgreSQL pgConn = new ConnectorPostgreSQL();
pgConn.Connection = pgConn.NewConnection(pgConnStr);
pgConn.Logger = Console.WriteLine;
pgConn.Open();

var row = pgConn.NewRow();
((DynaRow)row).helper.OnAfterParameterCreation = JsonParameterCorrection;
row.jsondata = """
{ "yoho":"werwer" }
""";
row.Insert(table: "testtable", autoincrement: "id;testtable_id_seq");


var dynamicList = await pgConn.GetDynamicListAsync($"select * from testtable where id = @1", 1);
dynamicList.ForEach(el => Console.WriteLine(el.jsondata));


dynamicList = await pgConn.GetDynamicListAsync($"select * from testtable;");
if (dynamicList.Count > 4)
    for (int i = 4; i < dynamicList.Count; i++)
    {
        ((DynaRow)dynamicList[i]).helper.OnAfterParameterCreation = JsonParameterCorrection;
        dynamicList[i].Delete();
    }

var listDicts = await pgConn.GetDictionaryListAsync("select * from testtable where jsondata->>'yoho' = @1", "werwer");

pgConn.Close();


void JsonParameterCorrection(string table, List<Parameter> parameters)
{
    foreach (var p in parameters)
        if (table == "testtable" && p.ColumnName.EndsWith("jsondata"))
            ((ParameterPostgreSQL)p).parameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
}
```
