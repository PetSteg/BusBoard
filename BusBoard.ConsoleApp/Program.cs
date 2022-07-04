using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        private const string PrimaryKey = "1056f6a94b6e44b48368f54a155429a3";

        private static string GetStopPointUrl(string id)
        {
            return @"https://api.tfl.gov.uk/StopPoint/" + id + "/Arrivals" + "?api_key=" + PrimaryKey;
        }

        private static string GetStopPointResponse(string requestUrl)
        {
            var html = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }

        static void Main(string[] args)
        {
            var id = "490015367S"; //Console.ReadLine();
            var jsonResponse = GetStopPointResponse(GetStopPointUrl(id));

            var arrivalPredictionsJson = JArray.Parse(jsonResponse);

            IList<ArrivalPrediction> arrivalPredictions = arrivalPredictionsJson.Select(p => new ArrivalPrediction
            {
                DestinationName = (string)p["destinationName"],
                LineId = (string)p["lineId"],
                TimeToStation = (int)p["timeToStation"]
            }).OrderBy(arrivalPrediction => arrivalPrediction.TimeToStation).ToList().GetRange(0, 5);

            foreach (var arrivalPrediction in arrivalPredictions)
            {
                Console.WriteLine(arrivalPrediction.ToString());
            }
        }
    }
}