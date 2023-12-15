using Npgsql;
using ConsoleTables;
namespace holidaymaker_group2;

public class Search(NpgsqlDataSource db)
{
    public async Task AvailableRooms()
    {
        string pattern = "yyyy-MM-dd";                   //Sets the required pattern for date-input.
        var startOfSeason = new DateTime(2022, 05, 31);
        var endOfSeason = new DateTime(2022, 08, 01);
        DateTime sDate = new DateTime();                //DateTime variables for checking date input and that the dates are within required span and in the right order.
        DateTime eDate = new DateTime();

        string? startDate = string.Empty;
        string? endDate = string.Empty;

        string inputPromt = "Please enter start day of booking ('yyyy-mm-dd')";   //used to keep old input

        bool validInput = false;                        //Used to only let valid inputs through the do-while-loop
        do
        {
            Console.Clear();
            Console.WriteLine(inputPromt);
            startDate = Console.ReadLine();
            try
            {
                sDate = DateTime.ParseExact(startDate, pattern, null);
                if (sDate > startOfSeason && sDate < endOfSeason)
                {
                    validInput = true;
                    inputPromt = $"{inputPromt}\n{startDate}\nPlease enter end date of booking ('yyyy-mm-dd')";
                }
                else
                {
                    Console.WriteLine("Date outside of Holiday Season");
                    Thread.Sleep(1000);
                }
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
            Console.WriteLine(inputPromt);
            endDate = Console.ReadLine();
            try
            {
                eDate = DateTime.ParseExact(endDate, pattern, null);
                if (eDate > startOfSeason && eDate < endOfSeason)
                {
                    if (eDate > sDate)
                    {
                        validInput = true;
                        inputPromt = $"{inputPromt}\n{endDate}\nChoose requirements (y/n)";
                    }
                    else
                    {
                        Console.WriteLine("End date must be after start date.");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.WriteLine("Date outside of Holiday Season");
                    Thread.Sleep(1000);
                }
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
            Console.WriteLine(inputPromt);
            Console.Write("Pool: ");
            poolInput = Console.ReadLine();
            if (poolInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nPool: {poolInput}";
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
                inputPromt = $"{inputPromt}\nPool: {poolInput}";
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
            Console.WriteLine(inputPromt);
            Console.Write("Evening enternainment: ");
            entertainmentInput = Console.ReadLine();
            if (entertainmentInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nEvening entertainment: {entertainmentInput}";
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
                inputPromt = $"{inputPromt}\nEvening entertainment: {entertainmentInput}";
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
            Console.WriteLine(inputPromt);
            Console.Write("Restaurant: ");
            restaurantInput = Console.ReadLine();
            if (restaurantInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nRestaurant: {restaurantInput}";
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
                inputPromt = $"{inputPromt}\nRestaurant: {restaurantInput}";
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
            Console.WriteLine(inputPromt);
            Console.Write("Kids club: ");
            kidsClubInput = Console.ReadLine();
            if (kidsClubInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nKids club: {kidsClubInput}";
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
                inputPromt = $"{inputPromt}\nKids club: {kidsClubInput}";
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
        searchTable.Configure(o => o.EnableCount = false);

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