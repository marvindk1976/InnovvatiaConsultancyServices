namespace WebApi5Paisa.Models
{
    public class Body
    {
        public string ClientCode { get; set; }
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string OrderType { get; set; }
        public string ScripData { get; set; }
        public bool IsIntraday { get; set; }
        public int DisQty { get; set; }
        public double StopLossPrice { get; set; }
        public string RemoteOrderID { get; set; }
    }

    public class Head
    {
        public string key { get; set; }
    }

    public class Root
    {
        public Head head { get; set; }
        public Body body { get; set; }
    }
}
