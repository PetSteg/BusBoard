using BusBoard.Api;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var postCode = "NW5 1TL"; //Console.ReadLine();
            IncomingBusesApi.PrintIncomingBusesPostCode(postCode);
        }
    }
}