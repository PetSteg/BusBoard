namespace BusBoard.ConsoleApp
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Location(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}