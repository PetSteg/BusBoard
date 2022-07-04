namespace BusBoard.ConsoleApp
{
    public class ArrivalPrediction
    {
        public string DestinationName { get; set; }
        public string LineId { get; set; }
        public int TimeToStation { get; set; }

        private string ExpectedArrivalMinutes()
        {
            var minutes = TimeToStation / 60;

            switch (minutes)
            {
                case 0:
                    return "due";
                case 1:
                    return "1 minute";
                default:
                    return minutes + " minutes";
            }
        }

        public override string ToString()
        {
            return "Line " + LineId + ": " + DestinationName + " in " + ExpectedArrivalMinutes();
        }
    }
}