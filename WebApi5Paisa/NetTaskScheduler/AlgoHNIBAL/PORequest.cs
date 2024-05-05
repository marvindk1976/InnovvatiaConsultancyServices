using System;
using System.Collections.Generic;
using System.Text;

namespace NetTaskScheduler.AlgoHNIBAL
{
    public class PORequest
    {
            public string ClientCode { get; set; }
            public string Exchange { get; set; }
            public string ExchangeType { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
            public string OrderType { get; set; }
            public string Symbol { get; set; }
            public string Expiry { get; set; }
            public string OptionType { get; set; }
            public double LTP { get; set; }
            public int NoOfStrike { get; set; }
            public int StrikeDirection { get; set; }
            public Guid RemoteOrderId { get; set; }
            public bool IsIntraday { get; set; }
            public int DisQty { get; set; }
            public double StopLossPrice { get; set; }
            public double TriggerPrice { get; set; }
            public DateTime OrderStartTimeStamp { get; set; }
            public DateTime OrderEndTimeStamp { get; set; }
            public string ScripData { get; set; }

    }
}
