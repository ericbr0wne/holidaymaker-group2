using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace holidaymaker_group2;

public class SearchParam(NpgsqlDataSource db)
{
    public int distanceToBeach { get; set; } = 22;

    public int distanceToCentre { get; set; } = 24;

    public int rating { get; set; } = 5;

    public int price { get; set; } = 3750;
}
