using holidaymaker_group2;
using Npgsql;
using System.Data;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

var tables = new Tables(db);
await tables.CreateAll();

Menu menu = Menu.Main;

while (true)
{
    if (menu.Equals(Menu.Main))
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("             The HolidayMaker               ");
        Console.WriteLine("|------------------------------------------|");
        Console.WriteLine("| 1. Customers                             |");
        Console.WriteLine("| 2. Bookings                              |");
        Console.WriteLine("| 3. Search                                |");
        Console.WriteLine("| 4. Exit                                  |");
        Console.WriteLine("|------------------------------------------|");
    }


    if (menu.Equals(Menu.Main))
    {
        string? input = Console.ReadLine();
        switch (input)
        {
            case "1":
                menu = Menu.Customers;
                break;
            case "2":
                menu = Menu.Bookings;
                break;
            case "3":
                menu = Menu.Search;
                break;
            case "4":
                menu = Menu.Exit;
                break;
            default:
                Console.Clear();
                Console.WriteLine("You didn't pick a valid option.");
                Console.WriteLine("Please press enter to break!");
                Console.ReadKey();
                Console.Clear();
                break;
        }
    }

    if (menu.Equals(Menu.Customers))
    {
        Console.Clear();
        Console.WriteLine("*** Customers ***");
        Console.WriteLine();
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Edit");
        Console.WriteLine("3. Display All");
        Console.WriteLine("4. Return to Main menu");
        Console.WriteLine("5. Exit HolidayMaker");

        switch (Console.ReadLine())
        {
            case "1":
                //Customers.Reg();
                break;
            case "2":
                //Customers.Edit();
                break;
            case "3":
                //Customers.Display();
                break;
            case "4":
                menu = Menu.Main;
                continue;
            case "5":
                menu = Menu.Exit;
                break;
            default:
                Console.Clear();
                Console.WriteLine("You didn't pick a valid option.");
                Console.WriteLine("Please press enter to continue!");
                Console.ReadKey();
                Console.Clear();
                break;
        }
    }

    if (menu.Equals(Menu.Bookings))
    {

        Console.Clear();
        Console.WriteLine("*** Bookings ***");
        Console.WriteLine();
        Console.WriteLine("1. Create");
        Console.WriteLine("2. Edit");
        Console.WriteLine("3. Delete");
        Console.WriteLine("4. Return to Main menu");
        Console.WriteLine("5. Exit HolidayMaker");

        switch (Console.ReadLine())
        {
            case "1":
                //Bookings.Create();
                break;
            case "2":
                //Bookings.Edit();
                break;
            case "3":
                //Bookings.Delete();
                break;
            case "4":
                menu = Menu.Main;
                continue;
            case "5":
                menu = Menu.Exit;
                break;
            default:
                Console.Clear();
                Console.WriteLine("You didn't pick a valid option.");
                Console.WriteLine("Please press enter to continue!");
                Console.ReadKey();
                Console.Clear();
                break;
        }
    }


    if (menu.Equals(Menu.Search))
    {
        Console.Clear();
        Console.WriteLine("*** Search ***");
        Console.WriteLine();
        Console.WriteLine("1. Rooms");
        Console.WriteLine("2. Locations");
        Console.WriteLine("3. Bookings");
        Console.WriteLine("4. Customers");
    
        Console.WriteLine("5. what else?");
        Console.WriteLine("6. what else?");
        Console.WriteLine("7. Return to Main menu");
        Console.WriteLine("8. Exit HolidayMaker");

        switch (Console.ReadLine())
        {
            case "1":
                //Search.Rooms();
                break;
            case "2":
                //Search.Locations();
                break;
            case "3":
                //Search.Bookings();
                break;
            case "4":
                //Search.Customers();
                break;
            case "5":
                //Search.?();
                break;
            case "6":
                //Search.?();
                break;
            case "7":
                menu = Menu.Main;
                continue;
            case "8":
                menu = Menu.Exit;
                break;
            default:
                Console.Clear();
                Console.WriteLine("You didn't pick a valid option.");
                Console.WriteLine("Please press enter to continue!");
                Console.ReadKey();
                Console.Clear();
                break;
        }
    }


    if (menu.Equals(Menu.Exit))
    {
        Console.Clear();
        Console.WriteLine("The HolidayMaker has been shut down successfully.");
        break;
    }




}
