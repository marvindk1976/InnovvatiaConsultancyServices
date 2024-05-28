using System;
using System.Collections.Generic;
using System.Text;

namespace NetTaskScheduler.Models
{
    public class wsData
    {
        public string Exch { get; set; }
        public string ExchType { get; set; }
        public int Token { get; set; }
        public double LastRate { get; set; }
        public int LastQty { get; set; }
        public int TotalQty { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public int OpenRate { get; set; }
        public double PClose { get; set; }
        public double AvgRate { get; set; }
        public int Time { get; set; }
        public int BidQty { get; set; }
        public double BidRate { get; set; }
        public int OffQty { get; set; }
        public double OffRate { get; set; }
        public int TBidQ { get; set; }
        public int TOffQ { get; set; }
        public string TickDt { get; set; }
        public string ChgPcnt { get; set; }
    }
}
