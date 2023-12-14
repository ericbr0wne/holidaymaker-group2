using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace holidaymaker_group2;

public class Customer(NpgsqlDataSource db)
{

public async Task UpdateCustomer()
{
    await using (var cmd = db.CreateCommand())
    {
        Console.WriteLine("Edit Customer!");
        Console.WriteLine("Enter the date of birth to find customer: ");
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

        // skapar en sträng för summera all input för i slutet göra en SET
        string setClause = "SET ";
        //Om strängarna fått input så blir första värdet = det nya inmatade värde, om strängen är tom hoppa över denna del
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

        // raderar 2 steg av setClause, Tar bort komma tecknet och mellanrummet som skapas i slutet. 
        if (setClause.EndsWith(", "))
        {
            setClause = setClause.Substring(0, setClause.Length - 2);
        }

        // Matar in alla värden som setClause samlat in på plats/person som söktes via DOB strängen
        cmd.CommandText = $"UPDATE customers {setClause} WHERE date_of_birth = '{DOB}'";

        await cmd.ExecuteNonQueryAsync();
    }
}
}

