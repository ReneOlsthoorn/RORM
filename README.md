# RORM
Object Relational Mapper for C# dotnet

dynamic row = pgConn.NewRow();
row.name = "hello";
row.lastname = "world";
row.Insert(table: "testtable", autoincrement: "id;testtable_id_seq");
