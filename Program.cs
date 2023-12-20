using ConsoleTables;
using holidaymaker_group2;
using Npgsql;
using System.Data;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker;Include Error Detail=True";

await using var db = NpgsqlDataSource.Create(dbUri);

var tables = new Tables(db);
await tables.CreateAll();
Menu Menu = new Menu(db);
await Menu.Main();