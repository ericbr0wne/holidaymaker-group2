using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace holidaymaker_group2;

public class Bookings(NpgsqlDataSource db)
{

    public async Task DisplayAvaibleRooms()
    {
        await using (var cmd = db.CreateCommand())
        {
            int beachDistance = DistanceInput("Enter max distance to for beach(KM): ");
            int centreDistance = DistanceInput("Enter max distance to the beach(KM):");

            string query = SearchQuery(beachDistance, centreDistance);
            ----

            bool HasPool = FacilityAddon("Do you want pool? y/n?");
            bool HasEveningEntertainment = FacilityAddon("Evening Entertainment? y/n ");
            bool Hasresturant = FacilityAddon("Resturant at the hotel?");
            bool HasKidsClub = FacilityAddon("KidsClub ? y/n");

            query = SearchQuery(beachDistance, centreDistance, HasPool, HasEveningEntertainment, Hasresturant,
                HasKidsClub);

        }

    }



    private int DistanceInput(string prompt)
    {
        int distance = 0;

        while (true)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input) || int.TryParse(input, out distance))
            {
                break;
            }

            Console.WriteLine("Enter a valid distance!");
            Thread.Sleep(1000);
        }

        return distance;
    }

    private string SearchQuery(int beachDistance, int centreDistance, bool HasPool, bool HasEveningEntertainment,
        bool HasResturant, bool HasKidsClub)
    {
        StringBuilder sQuery =
            new StringBuilder(
                "SELECT l.name, l.rating, l.beach_distance, l.centre_distance, r.room, r.number, r.size, r.price FROM rooms r JOIN locatioins l USING(location_id)");
        if (beachDistance > 0)
        {
            sQuery.AppendLine($"WHERE l.beach_distance < {beachDistance}");
        }

        if (sQuery.ToString().Contains("WHERE"))
        {
            sQuery.AppendLine($"AND l.centre_distance < {centreDistance}");

        }
        else
        {
            sQuery.AppendLine($"WHERE l.centre_distance < {centreDistance}");
        }

        if (HasPool)
        {
            FacilityAddon(ref sQuery, 1);
        }

        if (HasEveningEntertainment)
        {
            FacilityAddon(ref sQuery, 2);
        }

        if (HasResturant)
        {
            FacilityAddon(ref sQuery, 3);
        }

        if (HasKidsClub)
        {
            FacilityAddon(ref sQuery, 4);
        }

        return sQuery.ToString();

    }

    private void FacilityAddon(ref StringBuilder sQuery, int facilityId)
    {
        if (sQuery.ToString().Contains("WHERE"))
        {
            sQuery.AppendLine($"INTERSECT SELECT l.name, r.number, r.size, l.rating, l.beach_distance, l.centre_distance, r.price, r.room_id FROM rooms r JOIN locations l USING (location_id) JOIN locations_to_facilities ltf USING (location_id) WHERE ltf.facility_id = {facilityId}");");
        }
        else
        {
            sQuery.AppendLine(
                $"WHERE EXISTS ( SELECT 1 FROM location_to_facilities ltf WHERE ltf.facilityid = {facilityId} AND ltf.location_id = l.location_id");
        }
    }

}

