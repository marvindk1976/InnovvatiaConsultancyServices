namespace WebApi5Paisa.Models
{
    public class JsonObj
    {
        public int BrokerOrderID { get; set; }
        public int ClientCode { get; set; }
        public string Exch { get; set; }
        public int ExchOrderID { get; set; }
        public string ExchType { get; set; }
        public int LocalOrderID { get; set; }
        public string Message { get; set; }
        public int RMSResponseCode { get; set; }
        public string RemoteOrderID { get; set; }
        public int ScripCode { get; set; }
        public int Status { get; set; }
        public string Time { get; set; }
    }
}
