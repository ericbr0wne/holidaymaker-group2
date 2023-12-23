using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace holidaymaker_group2;

    public class Bookings(NpgsqlDataSource db)
    {
        public async Task AddBooking()
        {
                DisplayHotels();

                Console.WriteLine("Enter your start date ( yyyy-mm-dd )");
                DateTime NSD = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("enter your end date ( yyyy-mm-dd )");
                DateTime NED = DateTime.Parse(Console.ReadLine());

            await using (var cmd = db.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT COUNT (*) Booking WHERE @NewStartDate < End_date AND @NewEndDate > Start_Date";
                cmd.Parameters.AddWithValue("NewStartDate", NSD);
                cmd.Parameters.AddWithValue("NewEndDate", NED);

                var overlapCheck = (int)await cmd.ExecuteScalarAsync();

                if (overlapCheck == 0)
                {
                    Console.WriteLine("Your booking had been added");
                }
                else
                {
                    Console.WriteLine("This choice is not avaible for this date, Please try another one");
                }
            }
        }

        public async Task DisplayHotels()
        {
            await using (var cmd = db.CreateCommand())
            {
                List<string> hotels = new List<string>();
             
              string qDisplayAvaible = @$"SELECT r.*
                    FROM rooms r
                    JOIN bookings b ON r.room_id = b.room_id
                    WHERE b.booking_id IS NULL 
                    AND r.location_id = @HotelID
                    AND @StartDate < b.end_date
                    AND @EndDate > b.start_date";

            }

        }
    }

