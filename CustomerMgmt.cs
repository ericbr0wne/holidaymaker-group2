using ConsoleTables;
using Npgsql;
using System.ComponentModel.Design;
using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
namespace holidaymaker_group2;


public class CustomerMgmt(NpgsqlDataSource db)
{
    public async Task Reg()
    {

        await using (var cmd = db.CreateCommand())
        {
            string first_name = string.Empty;
                    Console.Clear();
                Console.WriteLine("*** Register Customer ***");
            do
            {
                Console.WriteLine();
                Console.Write("Enter first name: ");
                first_name = Console.ReadLine();

                if (string.IsNullOrEmpty(first_name))
                {
                    Console.WriteLine("Please enter first name.");
                    Console.WriteLine();

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
                    Console.WriteLine();

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
                    Console.WriteLine();

                }
                else
                {
                    Console.WriteLine("E-mail: " + mail);
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(mail));

            string phone = string.Empty;
            do
            {

                Console.Write("Enter phone number: ");
                phone = Console.ReadLine();
                if (!Regex.IsMatch(phone, @"^[0-9]+$"))
                {
                    Console.WriteLine("Please enter digits only.");
                    Console.WriteLine();

                }
                else
                {
                    Console.WriteLine("Phone number: " + phone);
                    Console.WriteLine();
                }
            } while (!Regex.IsMatch(phone, @"^[0-9]+$"));

            string pattern = "yyyy-MM-dd";
            DateTime dateOfBirth;
            bool validInput = false;

            do
            {
                Console.Write("Enter date of birth 'yyyy-mm-dd': ");
                string? stringDateOfBirth = Console.ReadLine();

                validInput = DateTime.TryParseExact(stringDateOfBirth, pattern, null, System.Globalization.DateTimeStyles.None, out dateOfBirth);

                if (validInput)
                {
                    Console.WriteLine("Date of birth: " + dateOfBirth.ToShortDateString());
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Please enter the correct format 'yyyy-mm-dd'");
                    Console.WriteLine();
                }
            } while (!validInput);
        
    


    int co_size;
            do
            {
                Console.Write("Enter company size (between 1 and 15): ");
                string stringCo_size = Console.ReadLine();

                if (int.TryParse(stringCo_size, out co_size))
                {
                    if (co_size >= 1 && co_size <= 15)
                    {
                        Console.WriteLine("Company size: " + co_size);
                        Console.WriteLine();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Company size must be between 1 and 15.");
                        Console.WriteLine();

                    }
                }
                else
                {
                    Console.WriteLine("Please enter digits only.");
                    Console.WriteLine();

                }
            } while (!(co_size >= 1 && co_size <= 15));

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


    public async Task Edit()
    {
        await using (var cmd = db.CreateCommand())
        {
            Console.Clear();
            Console.WriteLine("*** Edit Customer ***");
            Console.WriteLine();
            Console.WriteLine("Enter the date of birth to find customer Format yyyy-mm-dd: ");
            string DOB = Console.ReadLine();

            Console.WriteLine("Enter Firstname: (Tap enter to keep old value)");
            string NewFirstName = Console.ReadLine();
            Console.WriteLine("Enter Lastname (Tap enter to keep old value)");
            string NewLastName = Console.ReadLine();
            Console.WriteLine("Enter Email (Tap enter to keep old value)");
            string NewMail = Console.ReadLine();
            Console.WriteLine("Enter Phonenumber (Tap enter to keep old value)");
            string NewPhone = Console.ReadLine();
            Console.WriteLine("Enter birth of date (Tap enter to keep old value)");
            string NewDob = Console.ReadLine();
            Console.WriteLine("Enter CO Size(Tap enter to keep old value)");
            string NewCoSize = Console.ReadLine();

            string setClause = "SET ";
            if (!string.IsNullOrEmpty(NewFirstName))
            {
                setClause += $"first_name='{NewFirstName}', ";
            }

            if (!string.IsNullOrEmpty(NewLastName))
            {
                setClause += $"last_name='{NewLastName}', ";
            }

            if (!string.IsNullOrEmpty(NewMail))
            {
                setClause += $"mail='{NewMail}', ";
            }

            if (!string.IsNullOrEmpty(NewPhone))
            {
                setClause += $"phone='{NewPhone}', ";
            }

            if (!string.IsNullOrEmpty(NewDob))
            {
                setClause += $"date_of_birth='{NewDob}', ";
            }

            if (!string.IsNullOrEmpty(NewCoSize))
            {
                setClause += $"co_size='{NewCoSize}', ";
            }

            if (setClause.EndsWith(", "))
            {
                setClause = setClause.Substring(0, setClause.Length - 2);
            }

            cmd.CommandText = $"UPDATE customers {setClause} WHERE date_of_birth = '{DOB}'";

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task SelectAll()
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
            Console.ReadKey();
            Console.WriteLine("Press any key to go back to customer menu.");
        }
    }
}





