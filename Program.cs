using holidaymaker_group2;
using Npgsql;

const string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

var tables = new Tables(db);
await tables.CreateAll();

/*
var customer = new Customers(db);
await customer.Reg();


var displaycustomer = new Customers(db);
await displaycustomer.DisplayCustomers();    
*/

var search = new Search(db);
Cart cart = await search.AvailableRooms();

Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~");
Console.WriteLine(cart.StartDate);
Console.WriteLine(cart.EndDate);
foreach (int item in cart.Rooms)
{
    Console.WriteLine(item);
}

