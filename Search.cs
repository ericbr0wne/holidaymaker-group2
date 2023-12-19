using Npgsql;
using ConsoleTables;
using System.Security.Cryptography.X509Certificates;
namespace holidaymaker_group2;

public class Search(NpgsqlDataSource db)
{
    public async Task<Cart> AvailableRooms()
    {
        string pattern = "yyyy-MM-dd";
        var startOfSeason = new DateTime(2022, 05, 31);
        var endOfSeason = new DateTime(2022, 08, 01);
        DateTime sDate = new DateTime();
        DateTime eDate = new DateTime();

        string? startDate = string.Empty;
        string? endDate = string.Empty;

        string inputPromt = "Please enter start day of booking ('yyyy-mm-dd')";
        bool validInput = false;
        do
        {
            Console.Clear();
            Console.WriteLine(inputPromt);
            startDate = Console.ReadLine() ?? string.Empty;
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
            endDate = Console.ReadLine() ?? string.Empty;
            try
            {
                eDate = DateTime.ParseExact(endDate, pattern, null);
                if (eDate > startOfSeason && eDate < endOfSeason)
                {
                    if (eDate > sDate)
                    {
                        validInput = true;
                        inputPromt = $"{inputPromt}\n{endDate}\nMaximum distance from beach (leave empty if N/A): ";
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

        string beachDistance = string.Empty;
        string centreDistance = string.Empty;
        string hasPool = string.Empty;
        string hasEveningEnternainment = string.Empty;
        string hasRestaurant = string.Empty;
        string hasKidsClub = string.Empty;

        string beachInput = string.Empty;
        string centreInput = string.Empty;
        string poolInput = string.Empty;
        string entertainmentInput = string.Empty;
        string restaurantInput = string.Empty;
        string kidsClubInput = string.Empty;

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine(inputPromt);
            beachInput = Console.ReadLine() ?? string.Empty;
            if (beachInput == string.Empty)
            {
                validInput = true;
                beachInput = "N/A";
                inputPromt = $"{inputPromt}{beachInput}\nMaximum distance from city centre (leave empty if N/A): ";
            }
            else if (int.TryParse(beachInput, out int distance))
            {
                validInput = true;
                inputPromt = $"{inputPromt}{beachInput}\nMaximum distance from city centre (leave empty if N/A): ";
                beachDistance = @$"                                                                  
	            INTERSECT               

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
		        FROM rooms r
	        	JOIN locations l USING (location_id)
	           	WHERE l.beach_distance < {beachInput}
                ";

            }
            else
            {
                Console.WriteLine("Invalid input");
                Thread.Sleep(1000);
            }

        } while (!validInput);

        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine(inputPromt);
            centreInput = Console.ReadLine() ?? string.Empty;
            if (centreInput == string.Empty)
            {
                validInput = true;
                centreInput = "N/A";
                inputPromt = $"{inputPromt}{centreInput}\nChoose requirements (y/n)";
            }
            else if (int.TryParse(centreInput, out int distance))
            {
                validInput = true;
                inputPromt = $"{inputPromt}{centreInput}\nChoose requirements (y/n)";
                centreDistance = @$"                                                                  
	            INTERSECT               

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
		        FROM rooms r
	        	JOIN locations l USING (location_id)
	           	WHERE l.beach_distance < {centreInput}
                ";

            }
            else
            {
                Console.WriteLine("Invalid input");
                Thread.Sleep(1000);
            }

        } while (!validInput);
        do
        {
            validInput = false;
            Console.Clear();
            Console.WriteLine(inputPromt);
            Console.Write("Pool: ");
            poolInput = Console.ReadLine() ?? string.Empty;
            if (poolInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nPool: {poolInput}";
                validInput = true;
                hasPool = @"                                                                  
	            INTERSECT               

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
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
            entertainmentInput = Console.ReadLine() ?? string.Empty;
            if (entertainmentInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nEvening entertainment: {entertainmentInput}";
                validInput = true;
                hasEveningEnternainment = @"
            	INTERSECT

            	SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
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
            restaurantInput = Console.ReadLine() ?? string.Empty;
            if (restaurantInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nRestaurant: {restaurantInput}";
                validInput = true;
                hasRestaurant = @"
	            INTERSECT

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
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
            kidsClubInput = Console.ReadLine() ?? string.Empty;
            if (kidsClubInput is "y" or "Y")
            {
                inputPromt = $"{inputPromt}\nKids club: {kidsClubInput}";
                validInput = true;
                hasKidsClub = @"
	            INTERSECT

	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
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

        string orderPriceOrRating = string.Empty;

        Console.Clear();

        while (true)
        {

            string qSearchRooms = @$"
	        SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
	        FROM rooms r
		    JOIN locations l USING (location_id)
		    WHERE r.room_id NOT IN(
		        SELECT b.room_id
			    FROM bookings b
			    WHERE (b.start_date, b.end_date) OVERLAPS (date '{startDate}', date '{endDate}'))
            {beachDistance}
            {centreDistance}
		    {hasPool}
		    {hasEveningEnternainment}
    		{hasRestaurant}
	    	{hasKidsClub}
            ORDER BY {orderPriceOrRating}name ASC, number ASC
            ";

            var reader = await db.CreateCommand(qSearchRooms).ExecuteReaderAsync();

            var resultTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price");
            resultTable.Configure(o => o.EnableCount = false);

            int i = 1;
            while (await reader.ReadAsync())
            {
                resultTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), $"{reader.GetInt32(3)}/5", $"{reader.GetInt32(4)}km", $"{reader.GetInt32(5)}km", $"{reader.GetDecimal(6)}$");
                i++;
            }
            Console.WriteLine(resultTable);


            Console.WriteLine("1. Order by price");
            Console.WriteLine("2. Order by rating");
            Console.WriteLine("3. Add room to booking");
            Console.WriteLine("4. Return to previous menu");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    orderPriceOrRating = "price ASC, ";
                    break;
                case ConsoleKey.D2:
                    orderPriceOrRating = "rating DESC, ";
                    break;
                case ConsoleKey.D3:
                    var booking = new Booking(db);
                    Cart cart = await booking.AddToCart(startDate, endDate, qSearchRooms);
                    if (cart != null)
                    {
                        return cart;
                    }
                    else
                    {
                        break;
                    }
                case ConsoleKey.D4:
                    return null;
            }
            // This is because Console.Clear doesn't actually clear the terminal for some reason
            for (i = 0; i < 40; i++)
            {
                Console.WriteLine();
            }
        }
    }
}