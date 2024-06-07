using System;
using System.Collections.Generic;
using System.Text;

namespace NetTaskScheduler
{
    public class Symbol_GetLTPbyScripcode
    {
        public string symbol { get; set; }
        public DateTime WeeklyExpDate { get; set; }
        public DateTime MonthlyExpDate { get; set; }
        public string sCode { get; set; }
    } 
    public class ScripMaster
    {
        public string Exch { get; set; }
        public string ExchType { get; set; }
        public string ScripCode { get; set; }
        public string Name { get; set; }
        public string Expiry { get; set; }
        public string ScripType { get; set; }
        public string StrikeRate { get; set; }
        public string FullName { get; set; }
        public string TickSize { get; set; }
        public string LotSize { get; set; }
        public string QtyLimit { get; set; }
        public string Multiplier { get; set; }
        public string SymbolRoot { get; set; }
        public string BOCOAllowed { get; set; }
        public string ISIN { get; set; }
        public string ScripData { get; set; }
        public string Series { get; set; }
    }
}
