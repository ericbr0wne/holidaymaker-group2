using ConsoleTables;
using Npgsql;
using System.ComponentModel.Design;
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
            string first_name = string.Empty;
            do
            {
                Console.Write("Enter first name: ");
                first_name = Console.ReadLine();

                if (string.IsNullOrEmpty(first_name))
                {
                    Console.WriteLine("Please enter first name.");
                }
                else
                {
                    Console.WriteLine("First Name: " + first_name);
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(first_name));

            string last_name = string.Empty; 
            do
            {
                Console.Write("Enter last name: ");
                last_name = Console.ReadLine();

                if (string.IsNullOrEmpty(last_name))
                {
                    Console.WriteLine("Please enter last name.");
                }
                else
                {
                    Console.WriteLine("Last Name: " + last_name);
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(last_name));

            string mail = string.Empty; 
            do
            {
                Console.Write("Enter e-mail: ");
                mail = Console.ReadLine();

                if (string.IsNullOrEmpty(mail))
                {
                    Console.WriteLine("Please enter e-mail.");
                }
                else
                {
                    Console.WriteLine("E-mail: " + mail);
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(mail));

            //behöver endast (x antal?)siffror annars loop 
            Console.Write("Enter phone number: ");
            string phone = Console.ReadLine();
            Console.WriteLine("Phone number: " + phone);
            Console.WriteLine();

            //behöver endast datum format annars loop
            Console.Write("Enter date of birth 'yyyy-mm-dd': ");
            string? stringDateOfBirth = Console.ReadLine();
            DateTime dateOfBirth = DateTime.Parse(stringDateOfBirth);
            Console.WriteLine("Date of birth: " + (dateOfBirth.ToShortDateString()));
            Console.WriteLine();

            //behöver int mellan 1-15 annars loop. 
            Console.Write("Enter company size: ");
            string? stringCo_size = Console.ReadLine();
            int co_size = int.Parse(stringCo_size);
            Console.WriteLine("Company size: " + co_size);

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

        var displayCustTable = new ConsoleTable("FirstName", "LastName", "E-mail", "Phone", "DateOfBirth", "CompanySize");

        await using (var cmd = db.CreateCommand("SELECT first_name, last_name, mail, phone, date_of_birth, co_size FROM customers"))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                displayCustTable.AddRow(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4).ToShortDateString(), reader.GetInt32(5));
            }

            Console.Write(displayCustTable);
        }
    }




}


