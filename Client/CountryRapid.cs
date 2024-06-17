using Microsoft.VisualBasic;

namespace Client
{
    public class CountryRapid
    {
        public string name { get; set; }
        public string region { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string tz_id { get; set; }
        public string localtime_epoch { get; set; }
        public DateAndTime localtime { get; set; }

    }

 
}