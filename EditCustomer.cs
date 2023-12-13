﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace holidaymaker_group2;

public class Customer(NpgsqlDataSource db)
{

public async Task UpdateCustomer(NpgsqlDataSource db)
{
    await using (var cmd = db.CreateCommand())
    {
        Console.WriteLine("Redigera kund!");
        Console.WriteLine("Ange födelsedatum för att söka efter en kund: ");
        string DOB = Console.ReadLine();

        Console.WriteLine("Ange nytt förnamn (Tryck på Enter för att behålla befintligt)");
        string NewFirstName = Console.ReadLine();
        Console.WriteLine("Ange nytt efternamn (Tryck på Enter för att behålla befintligt)");
        string NewLastName = Console.ReadLine();
        Console.WriteLine("Ange ny e-post (Tryck på Enter för att behålla befintligt)");
        string NewMail = Console.ReadLine();
        Console.WriteLine("Ange nytt telefonnummer (Tryck på Enter för att behålla befintligt)");
        string NewPhone = Console.ReadLine();
        Console.WriteLine("Ange nytt födelsedatum (Tryck på Enter för att behålla befintligt)");
        string NewDob = Console.ReadLine();
        Console.WriteLine("Ange ny storlek på företaget (Tryck på Enter för att behålla befintligt)");
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

