using holidaymaker_group2;
using Npgsql;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

// var tables = new Tables(db);
// await tables.CreateAll();

Booking booking = new(db);
await booking.Create();