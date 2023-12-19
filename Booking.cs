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

            var reader = await db.CreateCommand(searchquery).ExecuteReaderAsync();

            var resultTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price"); 
            resultTable.Configure(o => o.EnableCount = false); 

            var cartTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price"); 
            cartTable.Configure(o => o.EnableCount = false); 

            int i = 1;
            while (await reader.ReadAsync()) 
            {
                allRooms.Add(reader.GetInt32(7));
                resultTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), $"{reader.GetInt32(3)}/5", $"{reader.GetInt32(4)}km", $"{reader.GetInt32(5)}km", $"{reader.GetDecimal(6)}$");
                i++;
            }

            while (true)
            {
                Console.WriteLine(resultTable); 
                Console.WriteLine("\n Booked Rooms");
                Console.WriteLine(cartTable);

                Console.WriteLine("Write the number of the room you'd like to add to your cart");   
                Console.WriteLine("Leave empty to return to previous menu and add any selected rooms to cart");
                string input = Console.ReadLine() ?? string.Empty;
                if (input == string.Empty)
                {
                    if (bookedRooms.Count == 0) 
                    {
                        return null;
                    }
                    else
                    {
                        Cart cart = new Cart(bookedRooms, startdate, enddate);
                        return cart;
                    }
                }

                if (int.TryParse(input, out int value) && value > 0 && value <= allRooms.Count && !bookedRooms.Contains(allRooms[value - 1])) 
                {
                    bookedRooms.Add(allRooms[value - 1]);
                    cartTable.AddRow(resultTable.Rows[value -1]);
                }
                else
                {
                    Console.WriteLine("Invalid input!\n Make sure you did't try to add a room that's already added or isn't on the list");
                    Console.ReadKey();
                }

                //Because of bug where console.clear doesn't clear console window
                for (int j = 0; j < 40; j++)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}


