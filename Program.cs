using holidaymaker_group2;
using Npgsql;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

var tables = new Tables(db);
await tables.CreateAll();


var e = new Customer(db);


        bool menu = true;

        while (menu)
        {
            Console.WriteLine("1: Booking");
            Console.WriteLine("2: Customer");
            Console.WriteLine("0: Search");
            string choice = Console.ReadLine();
            if (int.TryParse(choice, out int userinput))
            {
                switch (userinput)
                {
                     case 1: Console.WriteLine("booking function");
                         break;
                     case 2:
                         await e.UpdateCustomer();
                         break; 
                     case 0: menu = false;
                         break;

                }
            }
        }
