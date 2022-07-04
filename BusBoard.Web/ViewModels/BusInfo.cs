using System.Collections.Generic;
using BusBoard.Api;

namespace BusBoard.Web.ViewModels
{
    public class BusInfo
    {
        public BusInfo(string postCode)
        {
            PostCode = postCode;
            StationList = new List<Station>();
        }

        public string PostCode { get; set; }
        
        public List<Station> StationList { get; set; }
    }
}