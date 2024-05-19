using System.Collections.Generic;

namespace WebApi5Paisa.Models
{
    public class MarketFeedDatum
    {
        public string Exch { get; set; }
        public string ExchType { get; set; }
        public int ScripCode { get; set; }
    }

    public class wsDataObj
    {
        public string Method { get; set; }
        public string Operation { get; set; }
        public string ClientCode { get; set; }
        public List<MarketFeedDatum> MarketFeedData { get; set; }
    }
}
