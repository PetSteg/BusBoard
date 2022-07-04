namespace BusBoard.ConsoleApp
{
    public class Station
    {
        public string NaptanId { get; set; }
        public string Indicator { get; set; }
        public string Name { get; set; }

        public Station(string naptanId, string indicator, string name)
        {
            NaptanId = naptanId;
            Indicator = indicator;
            Name = name;
        }
    }
}