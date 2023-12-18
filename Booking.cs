using System;
using System.Threading.Tasks;
using ConsoleTables;
using Npgsql;

namespace holidaymaker_group2
{
    public class Booking(NpgsqlDataSource db)
    {
        public async Task EditBooking()
        {

            await using (var cmd = db.CreateCommand())
            {
                Console.WriteLine("Edit bookings");
                Console.WriteLine("Enter Booking number");
                string BN = Console.ReadLine();

                Console.WriteLine("Enter Number");
                string NewNumber = Console.ReadLine();
                Console.WriteLine("Enter customer ID");
                string NewCustomerID = Console.ReadLine();
                Console.WriteLine("Enter Room ID");
                string NewRoomID = Console.ReadLine();
                Console.WriteLine("Enter start date");
                string NewStartDate = Console.ReadLine();
                Console.WriteLine("enter end date");
                string NewEndDate = Console.ReadLine();

                string setClause = "SET ";

                if (!string.IsNullOrEmpty(NewNumber))
                {
                    setClause += $"number = '{NewNumber}', ";
                }
                if (!string.IsNullOrEmpty(NewCustomerID))
                {
                    setClause += $"customer_id = '{NewCustomerID}', ";
                }
                if (!string.IsNullOrEmpty(NewRoomID))
                {
                    setClause += $"room_id = '{NewRoomID}', ";
                }
                if (!string.IsNullOrEmpty(NewStartDate))
                {
                    setClause += $"start_date = '{NewStartDate}', ";
                }
                if (!string.IsNullOrEmpty(NewEndDate))
                {
                    setClause += $"end_date = '{NewEndDate}', ";
                }

                if (setClause.EndsWith(", "))
                {
                    setClause = setClause.Substring(0, setClause.Length - 2);
                }

                cmd.CommandText = $"UPDATE bookings {setClause} WHERE booking_id = '{BN}'";

                await cmd.ExecuteNonQueryAsync();


            }


        }

        public async Task<Cart> AddToCart(DateTime startdate, DateTime enddate, string searchquery)
        {
            List<int> allRooms = new List<int>();
            List<int> bookedRooms = new List<int>();

            Console.WriteLine("\n########################################################################\n");
            var reader = await db.CreateCommand(searchquery).ExecuteReaderAsync();

            var resultTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price"); //Creating a table using NuGet-package ConsoleTables 
            resultTable.Configure(o => o.EnableCount = false); // Removes annoying counter from displaying below the table.

            var cartTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price"); //Creating a table using NuGet-package ConsoleTables 
            cartTable.Configure(o => o.EnableCount = false); // Removes annoying counter from displaying below the table.

            int i = 1;
            while (await reader.ReadAsync()) //Gets the values from the database and adds it to rows in the table for search results.
            {
                allRooms.Add(reader.GetInt32(7));
                resultTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), $"{reader.GetInt32(3)}/5", $"{reader.GetInt32(4)}km", $"{reader.GetInt32(5)}km", $"{reader.GetDecimal(6)}$");
                i++;
            }

            while (true)
            {
                Console.WriteLine("\n########################################################################\n");
                Console.WriteLine(resultTable); // prints the search results
                Console.WriteLine("\n Booked Rooms");
                Console.WriteLine(cartTable);

                Console.WriteLine("Write the number of the room you'd like to add to your cart");   //menu for choices after search
                Console.WriteLine("Leave empty to return to previous menu and add any selected rooms to cart");
                string input = Console.ReadLine() ?? string.Empty;
                if (input == string.Empty)
                {
                    if (bookedRooms.Count == 0) //if there isn't any rooms added to the list this exits the task and returns a null value to cart.
                    {
                        return null;
                    }
                    else
                    {
                        Cart cart = new Cart(bookedRooms, startdate, enddate); //If there are rooms added this creates an instance of the cart-class and returns that instance.
                        return cart;
                    }
                }

                if (int.TryParse(input, out int value) && value > 0 && value <= allRooms.Count && !bookedRooms.Contains(allRooms[value - 1])) //Makes sure the user input is a number that is in the list and that isn't already added to cart.
                {
                    bookedRooms.Add(allRooms[value - 1]);
                    cartTable.AddRow(resultTable.Rows[value -1]);
                }
                else
                {
                    Console.WriteLine("Invalid input!\n Make sure you did't try to add a room that's already added or isn't on the list");
                    Console.ReadKey();
                }
            }
        }
    }
}


