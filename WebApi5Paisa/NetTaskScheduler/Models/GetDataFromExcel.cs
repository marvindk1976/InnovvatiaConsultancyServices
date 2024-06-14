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
        public double Get_StrikeRate(DataTable dtScripMaster,string symbol ,double LTP, int NoOfStrike,int StrikeDirection)
        {
            double StrikeRate = 0;

            IList<ScripMaster> items = dtScripMaster.AsEnumerable().Select(row => new ScripMaster
            {
                SymbolRoot = row.Field<string>("SymbolRoot"),
                StrikeRate = row.Field<string>("StrikeRate")
            }).ToList();


            ///filter data save Symbol_StrikeRate
            List<ScripMaster> newList = items.Where(m => m.SymbolRoot == symbol).OrderBy(x => x.StrikeRate).GroupBy(test => test.StrikeRate).Select(grp => grp.First()).ToList();
            
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
            int indexStrikeRate = newList.FindIndex(a =>  Convert.ToDouble(a.StrikeRate) == nearLTP);

            if (indexStrikeRate < 0)
            {
                StrikeRate = nearLTP;
            }
            else if (StrikeDirection == 0)
            {
                StrikeRate = nearLTP;
            }
            else if (StrikeDirection == 1)
            {
                int itmindValue = indexStrikeRate - NoOfStrike;
                var firstInt = newList.ElementAt(itmindValue);
                StrikeRate = Convert.ToDouble(firstInt.StrikeRate);
            }
            else if (StrikeDirection == 2)
            {
                int itmindValue = indexStrikeRate + NoOfStrike;
                var firstInt = newList.ElementAt(itmindValue);
                StrikeRate = Convert.ToDouble(firstInt.StrikeRate);
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

        public string Get_LTPByScripCode(DataTable stScripMaster, string Exch, string ExchType, string symbol,string ExpiryType,double strikerate)
        {
            IList<ScripMaster> items2 = stScripMaster.AsEnumerable().Select(row => new ScripMaster
            {
                Exch = row.Field<string>("Exch"),
                ExchType = row.Field<string>("ExchType"),
                ScripCode = row.Field<string>("ScripCode"),
                Name = row.Field<string>("Name"),
                Expiry = row.Field<string>("Expiry"),
                ScripType = row.Field<string>("ScripType"),
                StrikeRate = row.Field<string>("StrikeRate"),
                FullName = row.Field<string>("FullName"),
                TickSize = row.Field<string>("TickSize"),
                LotSize = row.Field<string>("LotSize"),
                QtyLimit = row.Field<string>("QtyLimit"),
                Multiplier = row.Field<string>("Multiplier"),
                SymbolRoot = row.Field<string>("SymbolRoot"),
                ISIN = row.Field<string>("ISIN"),
                ScripData = row.Field<string>("ScripData"),
                Series = row.Field<string>("Series")
            }).ToList();

            ///filter data save for string Exch, string ExchType, string symbol,string Expiry
           
            var ScriptType = "D";
            var scripCode = "";
            var WeeklyMonthlyExpDate = "";

            if (ExchType == "C")
            {
                ScriptType = "EQ";

                List<ScripMaster> newListScripMaster = items2.Where(m => m.Exch == Exch && m.ExchType == ExchType && m.SymbolRoot == symbol && m.ScripType == ScriptType).ToList();
                scripCode = newListScripMaster.Select(l => l.ScripCode).FirstOrDefault();
                ActScode = Convert.ToString(scripCode);
            }
            else
            {
                var currentdate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
                DateTime ActExpdate = new DateTime();
                
                
                if (ExpiryType.ToUpper().Contains("W"))
                {
                    string[] split = ExpiryType.ToUpper().Split("W");
                    int sp = Convert.ToInt32(split[1]);

                    List<ScripMaster> newScripMasterLsit = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType != "EQ").OrderBy(x => x.Expiry).ToList();

                    /////get first matching index
                    int index = newScripMasterLsit.FindIndex(a => Convert.ToDateTime(a.Expiry) >= currentdate);
                    int indexexpDate = index + (sp - 1);

                    var WeeklynewList = newScripMasterLsit.ElementAt(indexexpDate);
                    ActExpdate = Convert.ToDateTime(WeeklynewList.Expiry);

                    List<ScripMaster> newScripMasterScripCode = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType != "EQ" && Convert.ToDateTime(m.Expiry) == ActExpdate).OrderBy(x => x.Expiry).ToList();
                    var scodePE = newScripMasterScripCode.Where(l => l.ScripType == "PE" && Convert.ToDouble(l.StrikeRate) == Convert.ToDouble(strikerate)).First();
                    var scodeCE = newScripMasterScripCode.Where(l => l.ScripType == "CE" && Convert.ToDouble(l.StrikeRate) == Convert.ToDouble(strikerate)).First();
                    ActScode = Convert.ToString(scodePE.ScripCode) + "," + Convert.ToString(scodeCE.ScripCode);
                }
                else if (ExpiryType.ToUpper().Contains("M"))
                {
                    string[] split = ExpiryType.ToUpper().Split("M");
                    int sp = Convert.ToInt32(split[1]);

                    List<ScripMaster> newScripMasterLsit = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType == "XX").OrderBy(x => x.Expiry).ToList();

                    /////get first matching index
                    int index = newScripMasterLsit.FindIndex(a => Convert.ToDateTime(a.Expiry) >= currentdate);
                    int indexexpDate = index + (sp - 1);
                    var MonthlynewList = newScripMasterLsit.ElementAt(indexexpDate);

                    ActExpdate = Convert.ToDateTime(MonthlynewList.Expiry);

                    List<ScripMaster> newScripMasterScripCd = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType == "EQ" && Convert.ToDateTime(m.Expiry) == ActExpdate).ToList();
                    var scodeXX = newScripMasterScripCd.Select(l => l.ScripType == "XX" && Convert.ToDouble(l.StrikeRate) == strikerate).FirstOrDefault();

                    ActScode = Convert.ToString(scodeXX);
                }
                
            }
            return ActScode;
        }

        public DataTable ReadCSVFile(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public string Get_ScripData(DataTable stScripMaster, string Exch, string ExchType, string symbol, string ExpiryType, double strikerate,string ScriptType)
        {
            IList<ScripMaster> items2 = stScripMaster.AsEnumerable().Select(row => new ScripMaster
            {
                Exch = row.Field<string>("Exch"),
                ExchType = row.Field<string>("ExchType"),
                ScripCode = row.Field<string>("ScripCode"),
                Name = row.Field<string>("Name"),
                Expiry = row.Field<string>("Expiry"),
                ScripType = row.Field<string>("ScripType"),
                StrikeRate = row.Field<string>("StrikeRate"),
                FullName = row.Field<string>("FullName"),
                TickSize = row.Field<string>("TickSize"),
                LotSize = row.Field<string>("LotSize"),
                QtyLimit = row.Field<string>("QtyLimit"),
                Multiplier = row.Field<string>("Multiplier"),
                SymbolRoot = row.Field<string>("SymbolRoot"),
                ISIN = row.Field<string>("ISIN"),
                ScripData = row.Field<string>("ScripData"),
                Series = row.Field<string>("Series")
            }).ToList();

            ///filter data save for string Exch, string ExchType, string symbol,string Expiry,ScriptType

            var currentdate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
            DateTime ActExpdate = new DateTime();

            if (ExpiryType.ToUpper().Contains("W"))
            {
                string[] split = ExpiryType.ToUpper().Split("W");
                int sp = Convert.ToInt32(split[1]);

                List<ScripMaster> newScripMasterLsit = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType != "EQ").OrderBy(x => x.Expiry).ToList();

                /////get first matching index
                int index = newScripMasterLsit.FindIndex(a => Convert.ToDateTime(a.Expiry) >= currentdate);
                int indexexpDate = index + (sp - 1);

                var WeeklynewList = newScripMasterLsit.ElementAt(indexexpDate);
                ActExpdate = Convert.ToDateTime(WeeklynewList.Expiry);

                List<ScripMaster> newScripMasterScripCode = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType != "EQ" && Convert.ToDateTime(m.Expiry) == ActExpdate).OrderBy(x => x.Expiry).ToList();
                if (ScriptType == "PE")
                {
                    var scripDataPE = newScripMasterScripCode.Where(l => l.ScripType == "PE" && Convert.ToDouble(l.StrikeRate) == Convert.ToDouble(strikerate)).First();
                    ActScode = scripDataPE.ScripData;
                }
                else
                {
                    var scripDataCE = newScripMasterScripCode.Where(l => l.ScripType == "CE" && Convert.ToDouble(l.StrikeRate) == Convert.ToDouble(strikerate)).First();
                    ActScode = scripDataCE.ScripData;
                }
            }
            //else if (ExpiryType.ToUpper().Contains("M"))
            //{
            //    string[] split = ExpiryType.ToUpper().Split("M");
            //    int sp = Convert.ToInt32(split[1]);

            //    List<ScripMaster> newScripMasterLsit = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType == "XX").OrderBy(x => x.Expiry).ToList();

            //    /////get first matching index
            //    int index = newScripMasterLsit.FindIndex(a => Convert.ToDateTime(a.Expiry) >= currentdate);
            //    int indexexpDate = index + (sp - 1);
            //    var MonthlynewList = newScripMasterLsit.ElementAt(indexexpDate);

            //    ActExpdate = Convert.ToDateTime(MonthlynewList.Expiry);

            //    List<ScripMaster> newScripMasterScripCd = items2.Where(m => m.Exch == Exch && m.ExchType == "D" && m.SymbolRoot == symbol && m.ScripType == "EQ" && Convert.ToDateTime(m.Expiry) == ActExpdate).ToList();
            //    var scodeXX = newScripMasterScripCd.Select(l => l.ScripType == "XX" && Convert.ToDouble(l.StrikeRate) == strikerate).FirstOrDefault();

            //    ActScode = Convert.ToString(scodeXX);
            //}

            
            return ActScode;
        }

    }
}
