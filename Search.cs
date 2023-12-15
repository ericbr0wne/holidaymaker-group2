using Npgsql;
using ConsoleTables;
namespace holidaymaker_group2;

public class Search(NpgsqlDataSource db)
{
    public async Task AvailableRooms()
    {
        string pattern = "yyyy-MM-dd";
        var startOfSeason = new DateTime(2022 - 05 - 31);
        var endOfSeason = new DateTime(2022 - 08 - 01);
        DateTime sDate;
        DateTime eDate;

        string? startDate = string.Empty;
        string? endDate = string.Empty;

        bool validInput = false;
        do
        {
            Console.Clear();
            Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
            startDate = Console.ReadLine();
            try
            {
                sDate = DateTime.ParseExact(startDate, pattern, null);
                validInput = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("\n{0} is not in the correct format", startDate);
                Thread.Sleep(1000);
            }
        } while (!validInput);

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
            Console.WriteLine(startDate);
            Console.WriteLine("Please enter end day of booking ('yyyy-mm-dd')");
            endDate = Console.ReadLine();
            try
            {
                eDate = DateTime.ParseExact(endDate, pattern, null);
                validInput = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("\n{0} is not in the correct format", endDate);
                Thread.Sleep(1000);
            }
        } while (!validInput);

        string qPool = string.Empty;
        string qEveningEnternainment = string.Empty;
        string qRestaurant = string.Empty;
        string qKidsClub = string.Empty;
        string poolInput = string.Empty;
        string entertainmentInput = string.Empty;
        string restaurantInput = string.Empty;
        string kidsClubInput = string.Empty;

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
            Console.WriteLine(startDate);
            Console.WriteLine("Please enter end day of booking ('yyyy-mm-dd')");
            Console.WriteLine(endDate);
            Console.WriteLine("Choose requirements (y/n):");
            Console.Write("Pool: ");
            poolInput = Console.ReadLine();
            if (poolInput is "y" or "Y")
            {
                validInput = true;
                qPool = @"
	            INTERSECT

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		        FROM rooms r
	        	JOIN locations l USING (location_id)
		        JOIN locations_to_facilities ltf USING (location_id)
	           	WHERE ltf.facility_id = 1
                ";
            }
            else if (poolInput is "n" or "N")
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("\nInvalid input");
                Thread.Sleep(1000);
            }
        } while (!validInput);

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
            Console.WriteLine(startDate);
            Console.WriteLine("Please enter end day of booking ('yyyy-mm-dd')");
            Console.WriteLine(endDate);
            Console.WriteLine("Choose requirements (y/n):");
            Console.Write("Pool: ");
            Console.WriteLine(poolInput);
            Console.Write("Evening enternainment: ");
            entertainmentInput = Console.ReadLine();
            if (entertainmentInput is "y" or "Y")
            {
                validInput = true;
                qEveningEnternainment = @"
            	INTERSECT

            	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
	        	FROM rooms r
	        	JOIN locations l USING (location_id)
		        JOIN locations_to_facilities ltf USING (location_id)
	        	WHERE ltf.facility_id = 2
                ";
            }
            else if (entertainmentInput is "n" or "N")
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("\nInvalid input");
                Thread.Sleep(1000);
            }
        } while (!validInput);

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
            Console.WriteLine(startDate);
            Console.WriteLine("Please enter end day of booking ('yyyy-mm-dd')");
            Console.WriteLine(endDate);
            Console.WriteLine("Choose requirements (y/n):");
            Console.Write("Pool: ");
            Console.WriteLine(poolInput);
            Console.Write("Evening enternainment: ");
            Console.WriteLine(entertainmentInput);
            Console.Write("Restaurant: ");
            restaurantInput = Console.ReadLine();
            if (restaurantInput is "y" or "Y")
            {
                validInput = true;
                qRestaurant = @"
	            INTERSECT

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
		        FROM rooms r
        		JOIN locations l USING (location_id)
	        	JOIN locations_to_facilities ltf USING (location_id)
		        WHERE ltf.facility_id = 3
                ";
            }
            else if (restaurantInput is "n" or "N")
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("\nInvalid input");
                Thread.Sleep(1000);
            }
        } while (!validInput);

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine("Please enter start day of booking ('yyyy-mm-dd')");
            Console.WriteLine(startDate);
            Console.WriteLine("Please enter end day of booking ('yyyy-mm-dd')");
            Console.WriteLine(endDate);
            Console.WriteLine("Choose requirements (y/n):");
            Console.Write("Pool: ");
            Console.WriteLine(poolInput);
            Console.Write("Evening enternainment: ");
            Console.WriteLine(entertainmentInput);
            Console.Write("Restaurant: ");
            Console.WriteLine(restaurantInput);
            Console.Write("Kids club: ");
            kidsClubInput = Console.ReadLine();
            if (kidsClubInput is "y" or "Y")
            {
                validInput = true;
                qKidsClub = @"
	            INTERSECT

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
	        	FROM rooms r
		        JOIN locations l USING (location_id)
        		JOIN locations_to_facilities ltf USING (location_id)
	        	WHERE ltf.facility_id = 4
                ";
            }
            else if (kidsClubInput is "n" or "N")
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("\nInvalid input");
                Thread.Sleep(1000);
            }
        } while (!validInput);

        string qSearchRooms = @$"
	    SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price
	    FROM rooms r
		JOIN locations l USING (location_id)
		WHERE r.room_id NOT IN(
		    SELECT b.room_id
			FROM bookings b
			WHERE (b.start_date, b.end_date) OVERLAPS (date '{startDate}', date '{endDate}'))
		{qPool}
		{qEveningEnternainment}
		{qRestaurant}
		{qKidsClub}
        ORDER BY name DESC, number ASC
        ";

        var reader = await db.CreateCommand(qSearchRooms).ExecuteReaderAsync();

        var searchTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price");

        int i = 1;
        while (await reader.ReadAsync())
        {
            searchTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), $"{reader.GetInt32(3)}/5", $"{reader.GetInt32(4)}km", $"{reader.GetInt32(5)}km", $"{reader.GetDecimal(6)}$");
            i++;
        }
        Console.Clear();
        Console.WriteLine(searchTable);
    }
}