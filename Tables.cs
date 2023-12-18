using Npgsql;
namespace holidaymaker_group2;

public class Tables(NpgsqlDataSource db)
{
    public async Task CreateAll()
    {
        const string qRooms = @"
        CREATE TABLE IF NOT EXISTS rooms(
            room_id         SERIAL PRIMARY KEY,
            number          INTEGER NOT NULL,
            size            INTEGER NOT NULL,
            location_id     SERIAL REFERENCES locations(location_id),
            price           MONEY NOT NULL
        )";

        const string qBookings = @"
        CREATE TABLE IF NOT EXISTS bookings(
            booking_id      SERIAL PRIMARY KEY,
            number          INTEGER NOT NULL,
            customer_id     SERIAL REFERENCES customers(customer_id),
            room_id         SERIAL REFERENCES rooms(room_id),
            start_date      DATE NOT NULL,
            end_date        DATE NOT NULL
        )";

        const string qCustomers = @"
        CREATE TABLE IF NOT EXISTS customers(
            customer_id     SERIAL PRIMARY KEY,
            first_name      TEXT NOT NULL,
            last_name       TEXT NOT NULL,
            mail            TEXT NOT NULL,
            phone           TEXT NOT NULL,
            date_of_birth   DATE NOT NULL,
            co_size         INTEGER NOT NULL
        )";

        const string qLocations = @"
        CREATE TABLE IF NOT EXISTS locations(
            location_id     SERIAL PRIMARY KEY,
            name            TEXT NOT NULL,
            rating          SMALLINT NOT NULL,
            beach_distance  INTEGER NOT NULL,
            centre_distance INTEGER NOT NULL
        )";

        const string qFacilities = @"
        CREATE TABLE IF NOT EXISTS facilities(
            facility_id     SERIAL PRIMARY KEY,
            name            TEXT NOT NULL
        )";

        const string qLocationToFacilities = @"
        CREATE TABLE IF     NOT EXISTS location_to_facilities(
            id SERIAL       PRIMARY KEY,
            location_id     SERIAL REFERENCES locations (location_id),
            facility_id     SERIAL REFERENCES facilities (facility_id)
        )";

        const string qAddons = @"
        CREATE TABLE IF NOT EXISTS add_ons(
            add_on_id       SERIAL PRIMARY KEY,
            type            TEXT NOT NULL,
            price           MONEY NOT NULL
        )";

        const string qBookingstoAddons = @"
        CREATE TABLE IF NOT EXISTS bookings_to_add_ons(
            id              SERIAL PRIMARY KEY,
            booking_id      SERIAL REFERENCES bookings(booking_id),
            add_on_id       SERIAL REFERENCES add_ons(add_on_id)
        )";

        const string qLocationstoAddons = @"
        CREATE TABLE IF NOT EXISTS locations_to_add_ons(
            id              SERIAL PRIMARY KEY,
            location_id     SERIAL REFERENCES locations(location_id),
            add_on_id       SERIAL REFERENCES add_ons(add_on_id)
        )";

        await db.CreateCommand(qLocations).ExecuteNonQueryAsync();
        await db.CreateCommand(qCustomers).ExecuteNonQueryAsync();
        await db.CreateCommand(qRooms).ExecuteNonQueryAsync();
        await db.CreateCommand(qBookings).ExecuteNonQueryAsync();
        await db.CreateCommand(qAddons).ExecuteNonQueryAsync();
        await db.CreateCommand(qBookingstoAddons).ExecuteNonQueryAsync();
        await db.CreateCommand(qLocationstoAddons).ExecuteNonQueryAsync();
        await db.CreateCommand(qFacilities).ExecuteNonQueryAsync();
        await db.CreateCommand(qLocationToFacilities).ExecuteNonQueryAsync();
    }

}