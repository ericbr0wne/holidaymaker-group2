using ConsoleTables;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace holidaymaker_group2;

public class Booking(NpgsqlDataSource db)
{

    public async Task Cancel()
    {
        await using (var cmd = db.CreateCommand())
        {
            bool menuCancel = false;
            bool cancel = false;

            while (!menuCancel)
            {
                Console.WriteLine("1. Cancel a booking");
                Console.WriteLine("2. Return to booking menu");
                int? menuInput = Convert.ToInt32(Console.ReadLine());
                switch (menuInput)
                {
                    case 1:
                        Console.Clear();
                        menuCancel = true;
                        break;
                    case 2:
                        menuCancel = true;
                        cancel = true;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid input, please choose either 1 or 2.\n");
                        break;
                }
            }

            bool finalCancel = true;
            int bookingID = 0;

            while (!cancel)
            {

                Console.Write("Enter Booking number: ");
                int? BN = Convert.ToInt32(Console.ReadLine());

                bool checkNumber = true;

                while (checkNumber)
                {
                    int count = 0;

                    string qSearchBookingNumber = @$"
                    SELECT COUNT(*)
                    FROM bookings b
                    WHERE b.number = {BN}
                ";

                    var reader = await db.CreateCommand(qSearchBookingNumber).ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        count = reader.GetInt32(0);
                    }

                    if (count == 1)
                    {
                        var qShowBooking = @$"
                            SELECT *
                            FROM bookings b
                            WHERE b.number = {BN}";

                        var reader2 = await db.CreateCommand(qShowBooking).ExecuteReaderAsync();

                        while (await reader2.ReadAsync())
                        {
                            bookingID = reader2.GetInt32(0);
                            Console.WriteLine("\nBooking details:\n" + "\nBooking number: " + reader2.GetInt32(1) + "\nCustomer ID: " + reader2.GetInt32(2) + "\nRoom ID: " + reader2.GetInt32(3) +
                                "\nStart date: " + reader2.GetDateTime(4) + "\nEnd date: " + reader2.GetDateTime(5));
                        }
                        cancel = true;
                        finalCancel = true;
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine($"Booking number '{BN}' does not exist.");
                        finalCancel = false;
                        break;
                    }
                }

                Console.WriteLine();

                while (finalCancel)
                {
                    Console.WriteLine("Are you sure you want to cancel this booking? (Y/N)");
                    string? input = Console.ReadLine().ToUpper();
                    if (input == "Y")
                    {
                        cmd.CommandText = $"DELETE FROM bookings_to_add_ons ba WHERE ba.booking_id = {bookingID}";

                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = $"DELETE FROM bookings b WHERE b.number = {BN}";

                        await cmd.ExecuteNonQueryAsync();

                        Console.Clear();
                        Console.WriteLine($"The booking ({BN}) has been deleted from the system.");
                        break;
                    }
                    else if (input == "N")
                    {
                        Console.Clear();
                        Console.WriteLine("The booking was not cancelled.\n");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please type either 'Y' or 'N'\n");
                    }
                }
            }
        }
    }

    public async Task Create(Cart cart)
    {
        await using (var cmd = db.CreateCommand())
        {
            int customerNumber = 0;
            int companySize = 0;
            string? customerName = string.Empty;
            bool foundCustomer = false;

            while (!foundCustomer)
            {
                Console.Write("Enter customer's first and last name: ");
                customerName = Console.ReadLine();
                string[] customerNameSplit = customerName.Split(" ");
                while (customerNameSplit.Length != 2)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try to enter name again.");
                    customerName = Console.ReadLine();
                    customerNameSplit = customerName.Split(" ");
                }

                string qSearchCustomer = @$"
                    SELECT c.first_name, c.last_name, c.customer_id, c.co_size
                    FROM customers c
                    WHERE c.first_name = $1 AND c.last_name = $2
                ";

                var query = db.CreateCommand(qSearchCustomer);
                query.Parameters.AddWithValue(customerNameSplit[0]);
                query.Parameters.AddWithValue(customerNameSplit[1]);
                var reader = await query.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    Console.WriteLine("\nCustomer name: " + reader.GetString(0) + " " + reader.GetString(1) + "\nCustomer ID: "
                        + (customerNumber = reader.GetInt32(2)) + "\nCompany size: " + (companySize = reader.GetInt32(3)));
                    foundCustomer = true;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Customer not found, please try again.\n");
                }
            }

            int totalRoomSize = 0;
            foreach (var item in cart.RoomSize)
            {
                totalRoomSize += item;
            }

            if (companySize > totalRoomSize)
            {
                Console.WriteLine($"\nYour company size exceeds the number of beds in these rooms ({totalRoomSize})");
                Console.Write("Press any key to return to menu");
                Console.ReadKey();
                return;
            }

            string pattern = "yyyy-mm-dd";
            string? stringStartDate = cart.StartDate;
            string? stringEndDate = cart.EndDate;

            DateTime startDate = DateTime.ParseExact(stringStartDate, pattern, null);

            DateTime endDate = DateTime.ParseExact(stringStartDate, pattern, null);

            List<int> listRoomID = cart.Rooms;
            Console.WriteLine();
            foreach (int item in listRoomID)
            {
                Console.WriteLine("Room ID: " + item);
            }

            List<int> addOnList = new List<int>();
            List<string> addOnList2 = new List<string>();

            bool addOns = false;

            Console.Write("\nDo you want any add-ons for the booking? (Y/N): ");

            do
            {
                string? input = Console.ReadLine().ToUpper();
                if (input == "Y")
                {
                    Console.Clear();
                    Console.WriteLine("1. Extra bed\n2. Half board\n3. Full board");
                    Console.Write("Type the number for the desired add-on: ");
                    int addOnInput = Convert.ToInt32(Console.ReadLine());

                    switch (addOnInput)
                    {
                        case 1:
                            addOnList.Add(1);
                            addOnList2.Add("Extra bed");
                            break;
                        case 2:
                            if (addOnList.Contains(2))
                            {
                                Console.Clear();
                                Console.WriteLine("Can't order more than a half board per booking.\n");
                                break;
                            }
                            addOnList.Add(2);
                            addOnList2.Add("Half board");
                            break;
                        case 3:
                            if (addOnList.Contains(3))
                            {
                                Console.Clear();
                                Console.WriteLine("Can't order more than a full board per booking.\n");
                                break;
                            }
                            addOnList.Add(3);
                            addOnList2.Add("Full board");
                            break;
                        default:
                            break;
                    }
                }
                else if (input == "N")
                {
                    Console.Clear();
                    addOns = true;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, please type either 'Y' or 'N'");
                    Thread.Sleep(1500);
                    Console.Clear();
                }
                Console.Write("Does the customer want more add-ons? (Y/N): ");
            }
            while (!addOns);

            int bookingNumber = await GenerateNumber();

            List<string> listLocationName = new List<string>();
            for (int i = 0; i < listRoomID.Count; i++)
            {
                listLocationName.Add(await FindLocationName(listRoomID[i]));
            }

            Console.Clear();
            Console.WriteLine("Booking details\n\nCustomer name: " + customerName + "\nCustomer ID: " + customerNumber +
                "\nCompany size: " + companySize);
            for (int i = 0; i < listRoomID.Count; i++)
            {
                Console.WriteLine($"Room ID: {listRoomID[i]} ({listLocationName[i]})");
            }
            foreach (var item in addOnList2)
            {
                Console.WriteLine("Add-ons: " + item);
            }
            Console.WriteLine("Start date: " + stringStartDate + "\nEnd date: " + stringEndDate);

            decimal totalRoomPrice = await GetRoomPrices(listRoomID);
            decimal totalAddonPrice = await GetAddonPrices(addOnList);
            Console.WriteLine("Total price: " + (totalRoomPrice + totalAddonPrice));

            Console.Write("\nPress any key to finish the booking...");
            Console.ReadKey();

            for (int i = 0; i < listRoomID.Count; i++)
            {
                cmd.CommandText = "INSERT INTO bookings (number, customer_id, room_id, start_date, end_date) VALUES ($1, $2, $3, $4, $5)";

                cmd.Parameters.AddWithValue(bookingNumber);
                cmd.Parameters.AddWithValue(customerNumber);
                cmd.Parameters.AddWithValue(listRoomID[i]);
                cmd.Parameters.AddWithValue(startDate);
                cmd.Parameters.AddWithValue(endDate);
                await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
            }

            Console.Clear();
            Console.WriteLine("The booking was successful!\n");
            Console.WriteLine("Booking number: " + bookingNumber);

            await InsertAddon(customerNumber, addOnList);

            Console.Write("\nPress any key to return to booking menu");
            Console.ReadKey();
        }
    }

    public async Task<decimal> GetRoomPrices(List<int> roomIDs)
    {
        await using (var cmd = db.CreateCommand())
        {
            decimal totalPrice = 0;

            for (int i = 0; i < roomIDs.Count; i++)
            {
                string qGetPrices = @$"
                    SELECT r.price
                    FROM rooms r
                    WHERE r.room_id = $1
            ";

                var query = db.CreateCommand(qGetPrices);
                query.Parameters.AddWithValue(roomIDs[i]);
                var reader = await query.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    totalPrice += reader.GetDecimal(0);
                }
            }
            return totalPrice;
        }
    }

    public async Task<decimal> GetAddonPrices(List<int> addOns)
    {
        await using (var cmd = db.CreateCommand())
        {
            decimal totalPrice = 0;
            for (int i = 0; i < addOns.Count; i++)
            {
                string qGetPrices = @$"
                    SELECT a.price
                    FROM add_ons a
                    WHERE a.add_on_id = $1
            ";

                var query = db.CreateCommand(qGetPrices);
                query.Parameters.AddWithValue(addOns[i]);
                var reader = await query.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    totalPrice += reader.GetDecimal(0);
                }
            }
            return totalPrice;
        }
    }

    public async Task<int> GenerateNumber()
    {
        await using (var cmd = db.CreateCommand())
        {
            Random rnd = new Random();
            int bookingNumber = rnd.Next(10000, 99999);

            bool checkNumber = true;

            while (checkNumber)
            {
                int count = 0;

                string qSearchBookingNumber = @$"
                    SELECT COUNT(*)
                    FROM bookings b
                    WHERE b.number = $1
                ";

                var query = db.CreateCommand(qSearchBookingNumber);
                query.Parameters.AddWithValue(bookingNumber);
                var reader = await query.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    count = reader.GetInt32(0);
                }

                if (count == 1)
                {
                    bookingNumber = rnd.Next(10000, 99999);
                }
                else
                {
                    checkNumber = false;
                }
            }
            return bookingNumber;
        }
    }

    public async Task<string> FindLocationName(int roomID)
    {
        await using (var cmd = db.CreateCommand())
        {
            string locationName = string.Empty;

            string qSearchLocation = @$"
                    SELECT l.name
                    FROM locations l
                    JOIN rooms r ON l.location_id = r.location_id
                    WHERE r.room_id = $1
            ";

            var query = db.CreateCommand(qSearchLocation);
            query.Parameters.AddWithValue(roomID);
            var reader = await query.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                locationName = reader.GetString(0);
            }
            return locationName;
        }
    }

    public async Task InsertAddon(int cusNum, List<int> addOns)
    {
        await using (var cmd = db.CreateCommand())
        {
            int bookingID = 0;

            string qFindBookingID = @$"
                    SELECT MAX(b.booking_id)
                    FROM bookings b
                    WHERE b.customer_id = $1
                ";

            var query = db.CreateCommand(qFindBookingID);
            query.Parameters.AddWithValue(cusNum);
            var reader = await query.ExecuteReaderAsync();

            while (reader.Read())
            {
                bookingID = reader.GetInt32(0);
            }

            cmd.CommandText = "INSERT INTO bookings_to_add_ons (booking_id, add_on_id) values ($1, $2)";

            foreach (var item in addOns)
            {
                cmd.Parameters.AddWithValue(bookingID);
                cmd.Parameters.AddWithValue(item);
                await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
            }
        }
    }


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

    public async Task<Cart> AddToCart(string startdate, string enddate, string searchquery)
    {
        List<int> allRooms = new List<int>();
        List<int> allRoomSizes = new List<int>();
        List<int> bookedRooms = new List<int>();
        List<int> bookedRoomSizes = new List<int>();

        var reader = await db.CreateCommand(searchquery).ExecuteReaderAsync();

        var resultTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price");
        resultTable.Configure(o => o.EnableCount = false);

        var cartTable = new ConsoleTable("#", "Hotel", "Room No", "Room size", "Rating", "Distance to Beach", "Distance to city centre", "Price");
        cartTable.Configure(o => o.EnableCount = false);

        int i = 1;
        while (await reader.ReadAsync())
        {
            allRooms.Add(reader.GetInt32(7));
            allRoomSizes.Add(reader.GetInt32(2));
            resultTable.AddRow(i, reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), $"{reader.GetInt32(3)}/5", $"{reader.GetInt32(4)}km", $"{reader.GetInt32(5)}km", $"{reader.GetDecimal(6)}$");
            i++;
        }

        while (true)
        {

            //Because of bug where console.clear doesn't clear console window
            for (int j = 0; j < 40; j++)
            {
                Console.WriteLine();
            }
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
                    Cart cart = new Cart(bookedRooms, startdate, enddate, bookedRoomSizes);
                    return cart;
                }
            }

            if (int.TryParse(input, out int value) && value > 0 && value <= allRooms.Count && !bookedRooms.Contains(allRooms[value - 1]))
            {
                bookedRooms.Add(allRooms[value - 1]);
                bookedRoomSizes.Add(allRoomSizes[value - 1]);
                cartTable.AddRow(resultTable.Rows[value - 1]);
            }
            else
            {
                Console.WriteLine("Invalid input!\n Make sure you did't try to add a room that's already added or isn't on the list");
                Console.ReadKey();
            }
        }
    }
}


