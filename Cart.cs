namespace holidaymaker_group2;

public class Cart(List<int> rooms, string startdate, string enddate, List<int> roomsize)
{
    public List<int> Rooms => rooms;
    public string StartDate => startdate;
    public string EndDate => enddate;
    public List<int> RoomSize => roomsize;
}