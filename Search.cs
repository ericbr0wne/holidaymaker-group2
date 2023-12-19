using Npgsql;
using ConsoleTables;
using System.Security.Cryptography.X509Certificates;
namespace holidaymaker_group2;

public class Search(NpgsqlDataSource db)
{
    public async Task<Cart> AvailableRooms()
    {                case ConsoleKey.D3:
                    var booking = new Booking(db);
                    Cart cart = await booking.AddToCart(sDate, eDate, qSearchRooms);
                    if (cart != null)
                    {
                        return cart;
                    }
                    else
                    {
                        break;
                    }
                case ConsoleKey.D4:
                    return null;
                default:
                    Console.WriteLine("Invalid input");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }
}