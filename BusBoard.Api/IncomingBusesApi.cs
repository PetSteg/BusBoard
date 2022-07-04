using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BusBoard.Api
{
    public class IncomingBusesApi
    {
        private const string PrimaryKey = "1056f6a94b6e44b48368f54a155429a3";

        private static string GetJsonResponse(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var responseJson = string.Empty;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseJson = reader.ReadToEnd();
                }

                return responseJson;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static string GetStopPointUrl(string id)
        {
            return $@"https://api.tfl.gov.uk/StopPoint/{id}/Arrivals?api_key={PrimaryKey}";
        }

        private static Location GetLocationFromPostCode(string postCode)
        {
            var jsonResponse = GetJsonResponse($@"https://api.postcodes.io/postcodes/{postCode}?api_key={PrimaryKey}");

            if (jsonResponse == null)
            {
                return null;
            }

            var postCodeDetailsJson = JObject.Parse(jsonResponse)["result"];

            var longitude = (double)postCodeDetailsJson["longitude"];
            var latitude = (double)postCodeDetailsJson["latitude"];
            return new Location(longitude, latitude);
        }

        private static List<string> GetStopTypes(string type)
        {
            var jsonResponse =
                GetJsonResponse($@"https://api.tfl.gov.uk/StopPoint/Meta/StopTypes?api_key={PrimaryKey}");

            var stopTypesJson = JArray.Parse(jsonResponse);
            var stopTypes = stopTypesJson.Select(stopType => (string)stopType);

            return stopTypes.Where(stopType => stopType.Contains(type)).ToList();
        }

        private static List<Station> GetClosestStopPointsFromLocation(Location location, int maxCount)
        {
            var stations = new List<Station>();
            var stopTypes = GetStopTypes("Bus");
            var stopTypesUrlList = String.Join(",", stopTypes.ToArray());

            var url =
                $@"https://api.tfl.gov.uk/StopPoint/?lat={location.Latitude}&lon={location.Longitude}&stopTypes={stopTypesUrlList}";

            var jsonResponse = GetJsonResponse(url);
            var stopPoints = (JArray)JObject.Parse(jsonResponse)["stopPoints"];

            if (stopPoints == null || !stopPoints.HasValues)
            {
                return null;
            }

            if (stopPoints.Count < maxCount)
            {
                maxCount = stopPoints.Count;
            }

            foreach (var stopPoint in stopPoints.ToList().GetRange(0, maxCount))
            {
                stations.AddRange(GetStationsFromStopPoint(stopPoint));
            }

            return stations;
        }

        private static List<Station> GetStationsFromStopPoint(JToken stopPoint)
        {
            var stations = new List<Station>();
            var stationsJson = stopPoint["children"].ToList();

            foreach (var station in stationsJson)
            {
                var naptanId = (string)station["naptanId"];
                var indicator = (string)station["indicator"];
                var name = (string)station["commonName"];

                stations.Add(new Station(naptanId, indicator, name));
            }

            return stations;
        }

        private static string PrintIncomingBusesStopPoint(string stopPoint, int maxCount)
        {
            var incomingBuses = "";
            var jsonResponse = GetJsonResponse(GetStopPointUrl(stopPoint));

            var arrivalPredictionsJson = JArray.Parse(jsonResponse);

            var arrivalPredictions = arrivalPredictionsJson.Select(p => new ArrivalPrediction
            {
                DestinationName = (string)p["destinationName"],
                LineId = (string)p["lineId"],
                TimeToStation = (int)p["timeToStation"]
            }).OrderBy(arrivalPrediction => arrivalPrediction.TimeToStation).ToList();

            if (arrivalPredictions.Count > maxCount)
            {
                arrivalPredictions = arrivalPredictions.GetRange(0, maxCount);
            }

            foreach (var arrivalPrediction in arrivalPredictions)
            {
                incomingBuses += (arrivalPrediction.ToString());
            }

            return incomingBuses;
        }

        private static List<ArrivalPrediction> GetIncomingBusesStopPoint(string stopPoint, int maxCount)
        {
            var jsonResponse = GetJsonResponse(GetStopPointUrl(stopPoint));

            var arrivalPredictionsJson = JArray.Parse(jsonResponse);

            var arrivalPredictions = arrivalPredictionsJson.Select(p => new ArrivalPrediction
            {
                DestinationName = (string)p["destinationName"],
                LineId = (string)p["lineId"],
                TimeToStation = (int)p["timeToStation"]
            }).OrderBy(arrivalPrediction => arrivalPrediction.TimeToStation).ToList();

            if (arrivalPredictions.Count > maxCount)
            {
                arrivalPredictions = arrivalPredictions.GetRange(0, maxCount);
            }

            return arrivalPredictions;
        }

        public static void PrintIncomingBusesPostCode(string postCode)
        {
            var location = GetLocationFromPostCode(postCode);

            var stations = GetClosestStopPointsFromLocation(location, 2);
            foreach (var station in stations)
            {
                Console.WriteLine(station.Name + " (" + station.Indicator + ")");
                Console.WriteLine(PrintIncomingBusesStopPoint(station.NaptanId, 5));
                Console.WriteLine();
            }
        }

        public static List<Station> GetStationsWithIncomingBusesPostCode(string postCode)
        {
            var location = GetLocationFromPostCode(postCode);

            if (location == null)
            {
                return null;
            }

            var stations = GetClosestStopPointsFromLocation(location, 2);

            foreach (var station in stations)
            {
                station.IncomingBusses = GetIncomingBusesStopPoint(station.NaptanId, 5);
            }

            return stations;
        }
    }
}