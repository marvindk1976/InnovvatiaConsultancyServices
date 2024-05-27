using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Globalization;

namespace NetTaskScheduler.Models
{
    public class GetDataFromExcel
    {
        string ActScode;
        public DataTable excel(string fullPath)
        {
            DataTable dt = new DataTable();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var streamval = File.Open(fullPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(streamval))
                {
                    var configuration = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    var dataSet = reader.AsDataSet(configuration);
                    var dataTable1 = dataSet.Tables[0];
                    dt = dataTable1;

                }
            }
            return dt;
        }
        public double Get_StrikeRate(DataTable dtStrikeRate,string symbol ,double LTP, int NoOfStrike,int StrikeDirection)
        {
            double StrikeRate = 0;

            IList<Symbol_StrikeRate> items = dtStrikeRate.AsEnumerable().Select(row => new Symbol_StrikeRate
            {
                Symbol = row.Field<string>("Symbols"),
                StrikeRate = row.Field<double>("StrikeRate")
            }).ToList();


            ///filter data save Symbol_StrikeRate
            List<Symbol_StrikeRate> newList = items.Where(m => m.Symbol == symbol).ToList();

            //var LTP = 48700;
            //int NoOfStrike = 5;
            //var StrikeDirection = 2;
            double nearLTP;
            if ((LTP % 100) >= 50)
            {
                nearLTP = LTP + (100 - LTP % 100);
            }
            else
            {
                nearLTP = LTP - (LTP % 100);
            }

            ///get data with indexStrikeRate
            int indexStrikeRate = newList.FindIndex(a => a.StrikeRate == nearLTP);

            if (indexStrikeRate < 0)
            {
                StrikeRate = nearLTP;
            }
            else if (StrikeDirection == 0)
            {
                StrikeRate = LTP;
            }
            else if (StrikeDirection == 1)
            {
                int itmindValue = indexStrikeRate - NoOfStrike;
                var firstInt = newList.ElementAt(itmindValue);
                StrikeRate = firstInt.StrikeRate;
            }
            else if (StrikeDirection == 2)
            {
                int itmindValue = indexStrikeRate + NoOfStrike;
                var firstInt = newList.ElementAt(itmindValue);
                StrikeRate = firstInt.StrikeRate;
            }
            return StrikeRate;
        }

        public DateTime Get_WeeklyMonthlyExpDate(DataTable dtWeeklyMonthly, string WeeklyMonthlyExp, string symbol)
        {
            IList<Symbol_WeeklyMonthyDate> items2 = dtWeeklyMonthly.AsEnumerable().Select(row => new Symbol_WeeklyMonthyDate
            {
                symbol = row.Field<string>("Symbols"),
                WeeklyExpDate = row.Field<DateTime>("WeeklyexpDate"),
                MonthlyExpDate = row.Field<DateTime>("MonthlyexpDate")
            }).ToList();

            ///filter data save for Symbol_WeeklyMonthyDate
            List<Symbol_WeeklyMonthyDate> newList1 = items2.Where(m => m.symbol == symbol).ToList();
            //string date = "05/21/2024";
            //var currentdate = DateTime.Parse(date);
            var currentdate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
            DateTime ActExpdate = new DateTime();
            if (WeeklyMonthlyExp.ToUpper().Contains("W"))
            {
                string[] split = WeeklyMonthlyExp.ToUpper().Split("W");
                int sp = Convert.ToInt32(split[1]);

                /////get first matching index
                int index = newList1.FindIndex(a => Convert.ToDateTime(a.WeeklyExpDate) >= currentdate);
                int indexexpDate = index + (sp - 1);

                var WeeklynewList = newList1.ElementAt(indexexpDate);
                ActExpdate = WeeklynewList.WeeklyExpDate;


            }
            else if (WeeklyMonthlyExp.ToUpper().Contains("M"))
            {
                string[] split = WeeklyMonthlyExp.ToUpper().Split("M");
                int sp = Convert.ToInt32(split[1]);

                /////get first matching index
                int index = newList1.FindIndex(a => Convert.ToDateTime(a.MonthlyExpDate) >= currentdate);
                int indexexpDate = index + (sp - 1);
                var MonthlynewList = newList1.ElementAt(indexexpDate);
                ActExpdate = MonthlynewList.MonthlyExpDate;
            }
            return ActExpdate;
        }

        public string Get_LTPByScripCode(DataTable dtWeeklyMonthly, string WeeklyMonthlyExp, string symbol)
        {
            IList<Symbol_GetLTPbyScripcode> items2 = dtWeeklyMonthly.AsEnumerable().Select(row => new Symbol_GetLTPbyScripcode
            {
                symbol = row.Field<string>("Symbols"),
                WeeklyExpDate = row.Field<DateTime>("WeeklyexpDate"),
                MonthlyExpDate = row.Field<DateTime>("MonthlyexpDate"),
                sCode = row.Field<string>("scripCode")
            }).ToList();

            ///filter data save for Symbol_WeeklyMonthyDate
            List<Symbol_GetLTPbyScripcode> newList1 = items2.Where(m => m.symbol == symbol).ToList();
            //string date = "05/21/2024";
            //var currentdate = DateTime.Parse(date);
            var currentdate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
            DateTime ActExpdate = new DateTime();
            
            if (WeeklyMonthlyExp.ToUpper().Contains("W"))
            {
                string[] split = WeeklyMonthlyExp.ToUpper().Split("W");
                int sp = Convert.ToInt32(split[1]);

                /////get first matching index
                int index = newList1.FindIndex(a => Convert.ToDateTime(a.WeeklyExpDate) >= currentdate);
                int indexexpDate = index + (sp - 1);

                var WeeklynewList = newList1.ElementAt(indexexpDate);
                ActExpdate = WeeklynewList.WeeklyExpDate;
                ActScode = WeeklynewList.sCode;

            }
            else if (WeeklyMonthlyExp.ToUpper().Contains("M"))
            {
                string[] split = WeeklyMonthlyExp.ToUpper().Split("M");
                int sp = Convert.ToInt32(split[1]);

                /////get first matching index
                int index = newList1.FindIndex(a => Convert.ToDateTime(a.MonthlyExpDate) >= currentdate);
                int indexexpDate = index + (sp - 1);
                var MonthlynewList = newList1.ElementAt(indexexpDate);
                ActExpdate = MonthlynewList.MonthlyExpDate;
                ActScode = MonthlynewList.sCode;
            }
            return ActScode;
        }
    }
}
