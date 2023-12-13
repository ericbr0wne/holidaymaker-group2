using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace holidaymaker_group2;

public class Customer(NpgsqlDataSource db)
{
     public async Task AddCustomer()
    {
        await using (var cmd = db.CreateCommand())
        {
                    Console.WriteLine("add a new customer!");
        Console.WriteLine("Enter first name: ");
        Console.WriteLine("Enter first lastname: ");
        Console.WriteLine("Enter mail: ");
        Console.WriteLine("Enter phonenumber: ");
        Console.WriteLine("Enter CO size : ");


        string sasadsa = Console.ReadLine();

        }
    }
        static async Task UpdateCustomer(NpgsqlConnection db)
        {
            await using (var cmd = db.CreateCommand())
            {
                Console.WriteLine("Edit customer!"); 
                Console.WriteLine("Enter date of birth to search for a customer: "); 
                string DOB = Console.ReadLine();

                    string SearchQuery = $"SELECT * FROM customers WHERE date_of_birth = '{DOB}'";

                    Console.WriteLine("Enter new Firstname(Tap enter to keep existing)");
                    string NewFirstName = Console.ReadLine();
                    Console.WriteLine("Enter new Lastname(Tap enter to keep existing)");
                    string NewLastName = Console.ReadLine();
                    Console.WriteLine("Enter new Mail(Tap enter to keep existing)");
                    string NewMail = Console.ReadLine();
                    Console.WriteLine("Enter new Phonenumber(Tap enter to keep existing)");
                    string NewPhone = Console.ReadLine();
                    Console.WriteLine("Enter new Date of birth(Tap enter to keep existing)");
                    string NewDob = Console.ReadLine();
                    Console.WriteLine("Enter new CO size(Tap enter to keep existing)");
                    string NewCoSize = Console.ReadLine();

                    cmd.CommandText = "UPDATE customers SET" +
                                      (string.IsNullOrEmpty(NewFirstName) ? "":"first_name = $1, ")+
                                      (string .IsNullOrEmpty(NewLastName) ? "":"last_name = $2, ")+
                                      (string .IsNullOrEmpty(NewMail) ? "":"mail = $3, ")+
                                      (string .IsNullOrEmpty(NewPhone) ? "":"phone = $4, ")+
                                      (string .IsNullOrEmpty(NewDob) ? "":"date_of_birth = $5, ")+
                                      (string .IsNullOrEmpty(NewCoSize) ? "":"co_size = $6 ")+
                        $"WHERE date_of_birth = '{DOB}'";

                    cmd.Parameters.AddWithValue("$1", NewFirstName);
                    cmd.Parameters.AddWithValue("$2", NewLastName);
                    cmd.Parameters.AddWithValue("$3", NewMail);
                    cmd.Parameters.AddWithValue("$4", NewPhone);
                    cmd.Parameters.AddWithValue("$5", NewDob);
                    cmd.Parameters.AddWithValue("$6", NewCoSize);

                    await cmd.ExecuteNonQueryAsync();

            }

        }


}