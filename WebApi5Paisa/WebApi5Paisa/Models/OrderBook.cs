using System;

namespace WebApi5Paisa.Models
{
    public class OrderBook
    {
        public string AHProcess { get; set; }
        public string AfterHours { get; set; }
        public string AtMarket { get; set; }
        public int AveragePrice { get; set; }
        public int BrokerOrderId { get; set; }
        public DateTime BrokerOrderTime { get; set; }
        public string BuySell { get; set; }
        public string DelvIntra { get; set; }
        public int DisClosedQty { get; set; }
        public string Exch { get; set; }
        public string ExchOrderID { get; set; }
        public DateTime ExchOrderTime { get; set; }
        public string ExchType { get; set; }
        public int MarketLot { get; set; }
        public int OldorderQty { get; set; }
        public string OrderRequesterCode { get; set; }
        public string OrderStatus { get; set; }
        public string OrderValidUpto { get; set; }
        public int OrderValidity { get; set; }
        public int PendingQty { get; set; }
        public int Qty { get; set; }
        public int Rate { get; set; }
        public string Reason { get; set; }
        public string RemoteOrderID { get; set; }
        public string RequestType { get; set; }
        public int SLTriggerRate { get; set; }
        public string SLTriggered { get; set; }
        public int SMOProfitRate { get; set; }
        public int SMOSLLimitRate { get; set; }
        public int SMOSLTriggerRate { get; set; }
        public int SMOTrailingSL { get; set; }
        public int ScripCode { get; set; }
        public string ScripName { get; set; }
        public int TerminalId { get; set; }
        public int TradedQty { get; set; }
        public string WithSL { get; set; }
    }
}
