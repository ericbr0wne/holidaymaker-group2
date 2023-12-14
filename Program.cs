using ConsoleTables;
using holidaymaker_group2;
using Microsoft.VisualBasic;
using Npgsql;
using System.Linq.Expressions;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

var tables = new Tables(db);
await tables.CreateAll();

string pattern = "yyyy-MM-dd";
DateTime sDate;
DateTime eDate;

string? startDate = string.Empty;
string? endDate = string.Empty;
Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
startDate = Console.ReadLine();
try
{
	sDate = DateTime.ParseExact(startDate, pattern, null);
}
catch (FormatException)
{
    Console.WriteLine("{0} is not in the correct format", startDate);
}
Console.WriteLine("Please enter end day of booking ('yyyy-mm-dd')");
endDate = Console.ReadLine();
try
{
	eDate = DateTime.ParseExact(endDate, pattern, null);
}
catch (FormatException)
{
    Console.WriteLine("{0} is not in the correct format", endDate);
}


string qSearchRooms = @$"
	SELECT l.name, r.number, r.size
		FROM rooms r
		JOIN locations l ON r.location_id = l.location_id
		WHERE r.room_id NOT IN(
			SELECT b.room_id
			FROM bookings b
			WHERE (b.start_date, b.end_date) OVERLAPS (date '{startDate}', date '{endDate}')
)";

var reader = await db.CreateCommand(qSearchRooms).ExecuteReaderAsync();

var searchTable = new ConsoleTable("Row", "Hotel", "Room No", "Room size");

int i = 1;
while (await reader.ReadAsync())
{
    searchTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2));
    i++;
}

Console.WriteLine(searchTable);