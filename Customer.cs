using Npgsql;
using System.Data;
using System.Numerics;
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
            Console.WriteLine("First Name: " + first_name);
            Console.WriteLine();   

            Console.Write("Enter last name: ");
            string? last_name = Console.ReadLine();
            Console.WriteLine("Last name: " + last_name);
            Console.WriteLine();


            Console.Write("Enter e-mail: ");
            string? mail = Console.ReadLine();
            Console.WriteLine("E-mail: " + mail);
            Console.WriteLine();

            Console.Write("Enter phone number: ");
            string phone = Console.ReadLine();
            Console.WriteLine("Phone number: " + phone);
            Console.WriteLine();

            Console.Write("Enter date of birth 'yyyy-mm-dd': ");
            string? stringDateOfBirth = Console.ReadLine();
            DateTime dateOfBirth = DateTime.Parse(stringDateOfBirth);
            Console.WriteLine("Date of birth: " + (dateOfBirth.ToShortDateString()));
            Console.WriteLine();

            Console.Write("Enter company size: ");
            string? stringCo_size = Console.ReadLine();
            int co_size = int.Parse(stringCo_size);
            Console.WriteLine("Last name: " + co_size);

            cmd.CommandText = "INSERT INTO customers (first_name, last_name, mail, phone, date_of_birth, co_size) VALUES ($1, $2, $3, $4, $5, $6)";

            cmd.Parameters.AddWithValue(first_name);
            cmd.Parameters.AddWithValue(last_name);
            cmd.Parameters.AddWithValue(mail);
            cmd.Parameters.AddWithValue(phone);
            cmd.Parameters.AddWithValue(dateOfBirth);
            cmd.Parameters.AddWithValue(co_size);

            await cmd.ExecuteNonQueryAsync();

        }
    }


    public async Task DisplayCustomers()
    {
        await using (var cmd = db.CreateCommand("SELECT first_name, last_name, mail, phone, dateOfBirth, co_size FROM customers"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}





