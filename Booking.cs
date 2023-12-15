using System;
using System.Threading.Tasks;
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
    }
}


