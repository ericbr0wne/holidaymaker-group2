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
    public async Task Create()
    {
        await using (var cmd = db.CreateCommand())
        {
            int customerNumber = 0;
            bool foundCustomer = false;

            // Gjorde en while-loop för att hitta kunden (för och efternamnet) som man söker på och skriva ut denna+customer_id
            // sedan sparas customer_id i variabeln "customerNumber"
            while (!foundCustomer)
            {
                Console.Write("Enter customer's first and last name: ");
                string? customerName = Console.ReadLine();
                string[] customerNameSplit = customerName.Split(" ");
                while (customerNameSplit.Length != 2)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try to enter name again.");
                    customerName = Console.ReadLine();
                    customerNameSplit = customerName.Split(" ");
                }

                string qSearchCustomer = @$"
                    SELECT c.first_name, c.last_name, c.customer_id
                    FROM customers c
                    WHERE c.first_name = '{customerNameSplit[0]}' AND c.last_name = '{customerNameSplit[1]}'
                ";

                var reader = await db.CreateCommand(qSearchCustomer).ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    Console.WriteLine("Customer name: " + reader.GetString(0) + " " + reader.GetString(1) + "\nCustomer ID: " + (customerNumber = reader.GetInt32(2)));
                    foundCustomer = true;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Customer not found, please try again.\n");
                }
            }

            // Min tanke kring room_id är att vi hämtar den från en sökning i en annan klass (så att det faktiskt är möjligt att boka rummet det datumet)
            Console.Write("\nEnter the room ID for this vacation: ");
            string? stringRoomID = Console.ReadLine();
            int roomID = int.Parse(stringRoomID);

            // Snodde den snygga try-catchen från Klas, men la till while-loopar så att man måste skriva korrekt datum
            string pattern = "yyyy-mm-dd";
            DateTime sDate;
            DateTime eDate;
            string? stringStartDate = string.Empty;
            string? stringEndDate = string.Empty;
            bool enterDate1 = false;

            while (!enterDate1)
            {
                Console.WriteLine("\nEnter the start date for this vacation ('yyyy-mm-dd'):");
                stringStartDate = Console.ReadLine();
                try
                {
                    sDate = DateTime.ParseExact(stringStartDate, pattern, null);
                    enterDate1 = true;
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("{0} is not in the correct format", stringStartDate);
                }
            }
            DateTime startDate = DateTime.Parse(stringStartDate);

            bool enterDate2 = false;

            while (!enterDate2)
            {
                Console.WriteLine("Enter the end date for this vacation ('yyyy-mm-dd'):");
                stringEndDate = Console.ReadLine();
                try
                {
                    eDate = DateTime.ParseExact(stringEndDate, pattern, null);
                    enterDate2 = true;
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("{0} is not in the correct format", stringEndDate);
                }
            }
            DateTime endDate = DateTime.Parse(stringEndDate);

            List<int> addOnList = new List<int>();

            bool addOns = false;

            while (!addOns)
            {
                Console.Clear();
                Console.Write("Does the customer want any add ons for the booking? (Y/N): ");
                string? input = Console.ReadLine().ToUpper();
                if (input == "Y")
                {
                    Console.Clear();
                    Console.WriteLine("1. Extra bed\n2. Half board\n3. Full board");
                    Console.Write("Type the number for the desired add on: ");
                    int? addOnInput = Convert.ToInt32(Console.ReadLine());
                    switch (addOnInput)
                    {
                        case 1:
                            addOnList.Add(1);
                            break;
                        case 2:
                            addOnList.Add(2);
                            break;
                        case 3:
                            addOnList.Add(3);
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
                    Console.WriteLine("Invalid input, please type either 'Y' or 'N'");
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                }
            }


            // Genererar bara ett random nummer för bokningen mellan 10 000 och 99 999
            Random rnd = new Random();
            int bookingNumber = rnd.Next(10000, 99999);

            bool checkNumber = true;

            // Kollar så att bokningsnumret inte är en dubblett, om det är så kommer den att generera ett nytt nummer
            while (checkNumber)
            {
                int count = 0;

                string qSearchBookingNumber = @$"
                SELECT COUNT(*)
                FROM bookings b
                WHERE b.number = {bookingNumber}
            ";

                var reader = await db.CreateCommand(qSearchBookingNumber).ExecuteReaderAsync();

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

            cmd.CommandText = "INSERT INTO bookings (number, customer_id, room_id, start_date, end_date) VALUES ($1, $2, $3, $4, $5)";

            cmd.Parameters.AddWithValue(bookingNumber);
            cmd.Parameters.AddWithValue(customerNumber);
            cmd.Parameters.AddWithValue(roomID);
            cmd.Parameters.AddWithValue(startDate);
            cmd.Parameters.AddWithValue(endDate);

            await cmd.ExecuteNonQueryAsync();

            Console.Clear();
            Console.WriteLine("The booking was successful!\n");
            Console.WriteLine("Booking number: " + bookingNumber);


            int bookingID = 0;

            string qFindBookingID = @$"
                    SELECT MAX(b.booking_id)
                    FROM bookings b
                    WHERE b.customer_id = {customerNumber}
                ";

            var reader2 = await db.CreateCommand(qFindBookingID).ExecuteReaderAsync();

            while (reader2.Read())
            {
                bookingID = reader2.GetInt32(0);
            }

            foreach (var item in addOnList)
            {
                cmd.CommandText = $"INSERT INTO bookings_to_add_ons (booking_id, add_on_id) VALUES ({bookingID}, {item})";

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}