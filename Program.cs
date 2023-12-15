using holidaymaker_group2;
using Npgsql;
using System.Data;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

var tables = new Tables(db);
await tables.CreateAll(); //ha som en meny funktion? 

MenuClass menuClass = new MenuClass(db);
await menuClass.MainMenu();