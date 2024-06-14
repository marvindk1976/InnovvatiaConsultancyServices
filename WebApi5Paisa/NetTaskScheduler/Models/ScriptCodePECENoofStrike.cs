using Microsoft.Extensions.Configuration;
using NetTaskScheduler.AlgoHNIBAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using WebApi5Paisa;

namespace NetTaskScheduler.Models
{
    public class ScriptCodePECENoofStrike
    {
        public string GetScriptCodePECENoofStrike(string inputData)
        {
            string response = "";
            string ClientCode = "50084790";
            PORequest pORequest = new PORequest();
           
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"InputTextFile\FinalInput.txt");
            TextWriter tw = new StreamWriter(fullPath, false);
            tw.Write(string.Empty);
            tw.Close();

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                writer.WriteLine(inputData.TrimEnd(','));
            }

            string Symbol_StrikeRatePath = Path.Combine(Directory.GetCurrentDirectory(), @"Excel\Symbol_StrikeRate.xlsx");
            string Symbol_WeeklyMonthlyPath = Path.Combine(Directory.GetCurrentDirectory(), @"Excel\Symbol_WeeklyMonthly.xlsx");
            
            
            GetDataFromExcel getDataFromExcel = new GetDataFromExcel();
            DataTable dtSymbol_StrikeRate = getDataFromExcel.excel(Symbol_StrikeRatePath);

            string ScripMaster = Path.Combine(Directory.GetCurrentDirectory(), @"Excel\ScripMaster.csv");      
            DataTable dtScripMaster = getDataFromExcel.ReadCSVFile(ScripMaster);

            DataTable dtWeeklyMonthly = getDataFromExcel.excel(Symbol_WeeklyMonthlyPath);
            string fullPath3 = Path.Combine(Directory.GetCurrentDirectory(), @"InputTextFile\FinalInput.txt");
            string[] lines = File.ReadAllLines(fullPath3);

            foreach (string line in lines)
            {

                var myString = line.Split(',');
                pORequest.ClientCode = ClientCode;
                pORequest.Exchange = myString[0];
                pORequest.ExchangeType = myString[1];
                pORequest.Qty = Convert.ToInt32(myString[2]);
                pORequest.Price = Convert.ToInt32(myString[3]);
                pORequest.OrderType = myString[4];
                pORequest.Symbol = myString[5];
                pORequest.Expiry = myString[6];
                //pORequest.OptionType = myString[7];
                //pORequest.ScriptCode = Convert.ToInt32(myString[8]);
                pORequest.NoOfStrike = Convert.ToInt32(myString[7]);
                pORequest.StrikeDirection = Convert.ToInt32(myString[8]);
                pORequest.IsIntraday = Convert.ToBoolean(myString[9]);
                pORequest.DisQty = Convert.ToInt32(myString[10]);
                pORequest.StopLossPrice = Convert.ToInt32(myString[11]);
                pORequest.TriggerPrice = Convert.ToInt32(myString[12]);
                pORequest.OrderStartTimeStamp = DateTime.Parse(myString[13]);
                pORequest.OrderEndTimeStamp = DateTime.Parse(myString[14]);
                pORequest.RemoteOrderId = Guid.NewGuid();

                var strate = 0;
                var Scode = getDataFromExcel.Get_LTPByScripCode(dtScripMaster, pORequest.Exchange, pORequest.ExchangeType , pORequest.Symbol, pORequest.Expiry, strate);

                double ltpWS = 0;
                ltpWS = CallWSGetLTPByScipCode(pORequest.Exchange,pORequest.ExchangeType,Scode,pORequest.Price,ClientCode);
                //// need to get the input data from excel
                var strikerate = getDataFromExcel.Get_StrikeRate(dtScripMaster, pORequest.Symbol, ltpWS, pORequest.NoOfStrike, 0);

                //var WeeklyMonthlyExpDate = getDataFromExcel.Get_WeeklyMonthlyExpDate(dtWeeklyMonthly, pORequest.Expiry, pORequest.Symbol);
                var ScodePECEXX = getDataFromExcel.Get_LTPByScripCode(dtScripMaster, pORequest.Exchange, pORequest.ExchangeType ="D", pORequest.Symbol, pORequest.Expiry, strikerate);


                var intList = new List<int>(Array.ConvertAll(ScodePECEXX.Split(','), Convert.ToInt32));
                var scPE = intList[0];
                var scCE = intList[1];

                double ltpWSPe = 0;
                ltpWSPe = CallWSGetLTPByScipCode(pORequest.Exchange, pORequest.ExchangeType, scPE.ToString(), pORequest.Price, ClientCode);

                //// need to get the input data from excel
                var strikeRatePe = getDataFromExcel.Get_StrikeRate(dtScripMaster, pORequest.Symbol, ltpWS, pORequest.NoOfStrike, 0);
                var ScripDataPE = getDataFromExcel.Get_ScripData(dtScripMaster, pORequest.Exchange, pORequest.ExchangeType, pORequest.Symbol, pORequest.Expiry, strikeRatePe, "PE");

                double ltpWSCe = 0;
                ltpWSCe = CallWSGetLTPByScipCode(pORequest.Exchange, pORequest.ExchangeType, scCE.ToString(), pORequest.Price, ClientCode);

                var strikeRateCe = getDataFromExcel.Get_StrikeRate(dtScripMaster, pORequest.Symbol, ltpWS, pORequest.NoOfStrike, 0);
                var ScripDataCE = getDataFromExcel.Get_ScripData(dtScripMaster, pORequest.Exchange, pORequest.ExchangeType, pORequest.Symbol, pORequest.Expiry, strikeRateCe, "CE");
                response = ScripDataPE + "," + ScripDataCE;
            }

            return response;
        }
        public double CallWSGetLTPByScipCode(string Exchange,string ExchangeType,string Scode,double Price,string ClientCode)
        {
            double ltpWS = 0;
            //Implement WebSocket

            WebsocketMarketFeedDataListReq websokectMarket = new WebsocketMarketFeedDataListReq();
            websokectMarket.Exch = Exchange;
            websokectMarket.ExchType = ExchangeType;
            websokectMarket.ScripCode = Convert.ToInt32(Scode);

            List<WebsocketMarketFeedDataListReq> websokectMarketList = new List<WebsocketMarketFeedDataListReq>();

            websokectMarketList.Add(websokectMarket);

            var dataStringSession = JsonConvert.SerializeObject(new
            {
                Method = "MarketFeedV3",
                Operation = "Subscribe",
                ClientCode = ClientCode,
                MarketFeedData = websokectMarketList,
            });


            string fullPathws = Path.Combine(Directory.GetCurrentDirectory(), @"SaveWSFeedJson\wsData.txt");
            TextWriter tws = new StreamWriter(fullPathws, false);
            tws.Write(string.Empty);
            tws.Close();

            using (StreamWriter writer = new StreamWriter(fullPathws))
            {
                writer.WriteLine(dataStringSession);
            }

            //Get LTP From WSApp Final Output File Path

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);

            IConfiguration config = builder.Build();

            var getJsonValue = config.GetSection("GetJsonValue").Get<GetJsonValue>();

            List<wsData> jswsData = new List<wsData>();
            
            if (Price > 0)
            {
                ltpWS = Price;
            }
            do
            {
                string jsonData = string.Empty;
                jsonData = File.ReadAllText(getJsonValue.GetUrlFromWSFinalOutputFolderPath);
                jswsData = JsonConvert.DeserializeObject<List<wsData>>(jsonData.ToString());

                if (jswsData != null)
                    ltpWS = jswsData.Select(l => l.LastRate).First();

            } while (jswsData == null || ltpWS == 0);

            return (double)ltpWS;
        }
    }
}
