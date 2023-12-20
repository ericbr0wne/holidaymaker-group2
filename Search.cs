using Npgsql;
using ConsoleTables;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
namespace holidaymaker_group2;

public class Search(NpgsqlDataSource db)
{
    public async Task<Cart> AvailableRooms()
    {
        await using (var cmd = db.CreateCommand())
        {

            string pattern = "yyyy-MM-dd";
            var startOfSeason = new DateTime(2022, 05, 31);
            var endOfSeason = new DateTime(2022, 08, 01);

            string startDate = await AddStartDate(pattern, startOfSeason, endOfSeason);
            string endDate = await AddEndDate(pattern, startOfSeason, endOfSeason, startDate);

            int beachDistance = await AskForBeachDistance();
            string qBeachDistance = string.Empty;
            if (beachDistance != 0)
            {
                qBeachDistance = @$"                                                                  
	                    INTERSECT               

    	                SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
    		            FROM rooms r
        	        	JOIN locations l USING (location_id)
	                   	WHERE l.beach_distance < $1
                        ";
            }
            int centreDistance = await AskForCentreDistance();
            string qCentreDistance = string.Empty;
            if (centreDistance != 0)
            {
                qCentreDistance = @$"                                                                  
        	            INTERSECT               
    
        	            SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
		                FROM rooms r
	        	        JOIN locations l USING (location_id)
        	           	WHERE l.beach_distance < $2 
                        ";

            }
            string hasPool = await PoolChoice();
            string hasEveningEntertainment = await EntertainmentChoice();
            string hasRestaurant = await RestaurantChoice();
            string hasKidsClub = await KidsClubChoice();
            string orderPriceOrRating = string.Empty;

            Console.Clear();

            while (true)
            {
                await PrintResults(qBeachDistance, qCentreDistance, hasPool, hasEveningEntertainment, hasRestaurant, hasKidsClub, orderPriceOrRating, startDate, endDate, beachDistance, centreDistance);
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
                        Cart cart = await booking.AddToCart(qBeachDistance, qCentreDistance, hasPool, hasEveningEntertainment, hasRestaurant, hasKidsClub, orderPriceOrRating, startDate, endDate, beachDistance, centreDistance);
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
                for (int i = 0; i < 50; i++)
                {
                    Console.WriteLine();
                }
            }
        }

        async Task<string> AddStartDate(string pattern, DateTime startOfSeason, DateTime endOfSeason)
        {
            await using (var cmd = db.CreateCommand())
            {
                Console.Clear();
                bool validInput = false;
                string dateInput = string.Empty;
                DateTime startDate = new DateTime();
                do
                {
                    Console.Clear();
                    Console.WriteLine("Enter start date of booking: ");
                    dateInput = Console.ReadLine() ?? string.Empty;
                    try
                    {
                        startDate = DateTime.ParseExact(dateInput, pattern, null);
                        if (startDate > startOfSeason && startDate < endOfSeason)
                        {
                            validInput = true;
                        }
                        else
                        {
                            Console.WriteLine("Date outside of Holiday Season");
                            Thread.Sleep(1000);
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("\n{0} is not in the correct format", dateInput);
                        Thread.Sleep(1000);
                    }
                } while (!validInput);
                return dateInput;
            }
        }

        async Task<string> AddEndDate(string pattern, DateTime startOfSeason, DateTime endOfSeason, string startDate)
        {
            await using (var cmd = db.CreateCommand())
            {
                Console.Clear();
                DateTime sDate = DateTime.ParseExact(startDate, pattern, null);
                bool validInput = false;
                string dateInput = string.Empty;
                DateTime endDate = new DateTime();
                do
                {
                    Console.Clear();
                    Console.WriteLine("Enter end date of booking: ");
                    dateInput = Console.ReadLine() ?? string.Empty;
                    try
                    {
                        endDate = DateTime.ParseExact(dateInput, pattern, null);
                        if (endDate > startOfSeason && endDate < endOfSeason)
                        {
                            if (endDate > sDate)
                            {
                                validInput = true;
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
                        Console.WriteLine("\n{0} is not in the correct format", dateInput);
                        Thread.Sleep(1000);
                    }
                } while (!validInput);
                return dateInput;
            }
        }

        async Task<int> AskForBeachDistance()
        {
            await using (var cmd = db.CreateCommand())
            {
                int beachDistance = 0;
                bool validInput = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Enter maximum distance(km) from beach (leave empty if N/A): ");
                    string beachInput = Console.ReadLine() ?? string.Empty;
                    if (beachInput == string.Empty)
                    {
                        validInput = true;
                    }
                    else if (int.TryParse(beachInput, out int distance))
                    {
                        if (distance != 0)
                        {
                            validInput = true;
                            beachDistance = distance;
                        }
                        else
                        {
                            Console.WriteLine("Distance can't be 0");
                            Thread.Sleep(1337);
                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        Thread.Sleep(1000);
                    }

                } while (!validInput);
                return beachDistance;
            }
        }

        async Task<int> AskForCentreDistance()
        {
            await using (var cmd = db.CreateCommand())
            {
                int centreDistance = 0;
                bool validInput = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Enter maximum distance(km) from city centre or leave empty if N/A: ");
                    string centreInput = Console.ReadLine() ?? string.Empty;
                    if (centreInput == string.Empty)
                    {
                        validInput = true;
                    }
                    else if (int.TryParse(centreInput, out int distance))
                    {
                        if (distance != 0)
                        {
                            validInput = true;
                            centreDistance = distance;
                        }
                        else
                        {
                            Console.WriteLine("Distance can't be 0");
                            Thread.Sleep(1337);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        Thread.Sleep(1000);
                    }

                } while (!validInput);
                return centreDistance;
            }
        }

        async Task<string> PoolChoice()
        {
            await using (var cmd = db.CreateCommand())
            {
                string hasPool = string.Empty;
                bool validInput = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Choose requirements (Y/N)");
                    Console.Write("Pool: ");
                    string poolInput = Console.ReadLine() ?? string.Empty;
                    if (poolInput is "y" or "Y")
                    {
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
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input");
                        Thread.Sleep(1000);
                    }
                } while (!validInput);
                return hasPool;
            }
        }

        async Task<string> EntertainmentChoice()
        {
            await using (var cmd = db.CreateCommand())
            {
                string hasEveningEntertainment = string.Empty;
                bool validInput = false;
                do
                {
                    validInput = false;
                    Console.Clear();
                    Console.WriteLine("Choose requirements (Y/N)");
                    Console.Write("Evening enternainment: ");
                    string entertainmentInput = Console.ReadLine() ?? string.Empty;
                    if (entertainmentInput is "y" or "Y")
                    {
                        validInput = true;
                        hasEveningEntertainment = @"
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
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input");
                        Thread.Sleep(1000);
                    }
                } while (!validInput);
                return hasEveningEntertainment;

            }
        }

        async Task<string> RestaurantChoice()
        {
            await using (var cmd = db.CreateCommand())
            {
                string hasRestaurant = string.Empty;
                bool validInput = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Choose requirements (Y/N)");
                    Console.Write("Restaurant: ");
                    string restaurantInput = Console.ReadLine() ?? string.Empty;
                    if (restaurantInput is "y" or "Y")
                    {
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
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input");
                        Thread.Sleep(1000);
                    }
                } while (!validInput);
                return hasRestaurant;
            }
        }

        async Task<string> KidsClubChoice()
        {
            await using (var cmd = db.CreateCommand())
            {
                string hasKidsClub = string.Empty;
                bool validInput = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Choose requirements (Y/N)");
                    Console.Write("Kids club: ");
                    string kidsClubInput = Console.ReadLine() ?? string.Empty;
                    if (kidsClubInput is "y" or "Y")
                    {
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
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid input");
                        Thread.Sleep(1000);
                    }
                } while (!validInput);
                return hasKidsClub;
            }
        }

        async Task PrintResults(string qBeachDistance, string qCentreDistance, string qHasPool, string qHasEveningEntertainment, string qHasRestaurant, string qHasKidsClub, string qOrderPriceOrRating, string startDate, string endDate, int beachDistance, int centreDistance)
        {
            {
                await using (var cmd = db.CreateCommand())
                {
                    string qSearchRooms = @$"
        	        SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id
	                FROM rooms r
        		    JOIN locations l USING (location_id)
		            WHERE r.room_id NOT IN(
        		        SELECT b.room_id
		        	    FROM bookings b
			            WHERE (b.start_date, b.end_date) OVERLAPS (date '{startDate}', date '{endDate}'))
                    {qBeachDistance}
                    {qCentreDistance}
        		    {qHasPool}
		            {qHasEveningEntertainment}
            		{qHasRestaurant}
	            	{qHasKidsClub}
                    ORDER BY {qOrderPriceOrRating}name ASC, number ASC
                    ";

                    var query = db.CreateCommand(qSearchRooms);
                    query.Parameters.AddWithValue(beachDistance);
                    query.Parameters.AddWithValue(centreDistance);
                    var reader = await query.ExecuteReaderAsync();


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

                }
            }
        }

    }
}