using Npgsql;
using System.Data;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
namespace holidaymaker_group2;


public class Customers(NpgsqlDataSource db)
{

    public async Task Reg()

    {
        await using (var cmd = db.CreateCommand())
        {

            Console.Write("Enter first name: ");
            string? first_name = Console.ReadLine();

            DateTime birth = new DateTime(1984, 04, 03);

            cmd.CommandText = "INSERT INTO customers (first_name, last_name, mail, phone, date_of_birth, co_size) VALUES ($1, $2, $3, $4, $5, $6)";

         
            Console.WriteLine("First Name: " + first_name);

            Console.Write("Enter last name: ");
            string? last_name = Console.ReadLine();



            cmd.Parameters.AddWithValue(first_name);
            cmd.Parameters.AddWithValue(last_name);
            cmd.Parameters.AddWithValue("mail");
            cmd.Parameters.AddWithValue(0704963907);
            cmd.Parameters.AddWithValue(birth);
            cmd.Parameters.AddWithValue(4);



            await cmd.ExecuteNonQueryAsync();

            

            
        }

    }


    public async Task DisplayCustomers()
    {
        await using (var cmd = db.CreateCommand("SELECT first_name FROM customers"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}





