using Npgsql;

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=holidaymaker";

await using var db = NpgsqlDataSource.Create(dbUri);

await using (var cmd = db.CreateCommand(@"
CREATE TABLE IF NOT EXISTS room(
    id SERIAL,
    number INTEGER NOT NULL,
    size INTEGER NOT NULL,
    location_id INTEGER NOT NULL,
    price INTEGER NOT NULL,
    PRIMARY KEY(id)
)"))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = db.CreateCommand(@"
CREATE TABLE IF NOT EXISTS booking(
    id SERIAL,
    number INTEGER NOT NULL,
    customer_id INTEGER NOT NULL,
    room_id INTEGER NOT NULL,
    start_date TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    end_date TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    extra_bed BOOLEAN NOT NULL DEFAULT FALSE,
    half_board BOOLEAN NOT NULL DEFAULT FALSE,
    full_board BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY(id)
)"))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = db.CreateCommand(@"
CREATE TABLE IF NOT EXISTS customer(
    id SERIAL,
    first_name CHARACTER VARYING(255) NOT NULL,
    last_name CHARACTER VARYING(255) NOT NULL,
    mail CHARACTER VARYING(255) NOT NULL,
    phone BIGINT NOT NULL,
    date_of_birth DATE NOT NULL,
    co_size INTEGER NOT NULL,
    PRIMARY KEY(id)
)"))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = db.CreateCommand(@"
CREATE TABLE IF NOT EXISTS location(
    id SERIAL,
    name CHARACTER VARYING(255) NOT NULL,
    rating SMALLINT NOT NULL,
    beach_distance INTEGER NOT NULL,
    centre_distance INTEGER NOT NULL,
    pool BOOLEAN NOT NULL DEFAULT FALSE,
    kids_club BOOLEAN NOT NULL DEFAULT FALSE,
    restaurant BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY(id)
)"))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = db.CreateCommand(@"
CREATE TABLE IF NOT EXISTS add_on(
    type CHARACTER VARYING(255) NOT NULL,
    price INTEGER NOT NULL
)"))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = db.CreateCommand(@"
ALTER TABLE IF EXISTS room
    ADD FOREIGN KEY(location_id) REFERENCES location(id)    
"))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = db.CreateCommand(@"
ALTER TABLE IF EXISTS booking
    ADD FOREIGN KEY(room_id) REFERENCES room(id),
    ADD FOREIGN KEY(customer_id) REFERENCES customer(id),
"))
{
    await cmd.ExecuteNonQueryAsync();
}

