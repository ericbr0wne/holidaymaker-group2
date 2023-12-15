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

string qPool = string.Empty;
string qEveningEnternainment = string.Empty;
string qRestaurant = string.Empty;
string qKidsClub = string.Empty;

Console.WriteLine("Choose requirements (y/n):");
Console.Write("Pool: ");
if (Console.ReadLine() == "y")
{
    qPool = @"
	INTERSECT

	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		FROM rooms r
		JOIN locations l USING (location_id)
		JOIN location_to_facilities ltf USING (location_id)
		WHERE ltf.facility_id = 1
";
}

Console.Write("Evening enternainment: ");
if (Console.ReadLine() == "y")
{
    qEveningEnternainment= @"
	INTERSECT

	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		FROM rooms r
		JOIN locations l USING (location_id)
		JOIN location_to_facilities ltf USING (location_id)
		WHERE ltf.facility_id = 2
";
}

Console.Write("Restaurant: ");
if (Console.ReadLine() == "y")
{
    qRestaurant= @"
	INTERSECT

	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		FROM rooms r
		JOIN locations l USING (location_id)
		JOIN location_to_facilities ltf USING (location_id)
		WHERE ltf.facility_id = 3
";
}

Console.Write("Kids club: ");
if (Console.ReadLine() == "y")
{
    qKidsClub= @"
	INTERSECT

	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		FROM rooms r
		JOIN locations l USING (location_id)
		JOIN location_to_facilities ltf USING (location_id)
		WHERE ltf.facility_id = 4
";
}

string qSearchRooms = @$"
	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		FROM rooms r
		JOIN locations l USING (location_id)
		WHERE r.room_id NOT IN(
			SELECT b.room_id
			FROM bookings b
			WHERE (b.start_date, b.end_date) OVERLAPS (date '{startDate}', date '{endDate}')
			)
		{qPool}
		{qEveningEnternainment}
		{qRestaurant}
		{qKidsClub}

		ORDER BY name DESC
";

var reader = await db.CreateCommand(qSearchRooms).ExecuteReaderAsync();

var searchTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price");

int i = 1;
while (await reader.ReadAsync())
{
    searchTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), $"{reader.GetInt32(3)}/5", $"{reader.GetInt32(4)}km", $"{reader.GetInt32(5)}km", $"{reader.GetDecimal(6)}$");
    i++;
}

Console.WriteLine(searchTable);