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
        public bool GetScriptCodePECENoofStrike(string inputData)
        {
            bool response = false;
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
                pORequest.OptionType = myString[7];
                pORequest.ScriptCode = Convert.ToInt32(myString[8]);
                pORequest.NoOfStrike = Convert.ToInt32(myString[9]);
                pORequest.StrikeDirection = Convert.ToInt32(myString[10]);

                var Scode = getDataFromExcel.Get_LTPByScripCode(dtWeeklyMonthly, pORequest.Expiry, pORequest.Symbol);

                //Implement WebSocket

                WebsocketMarketFeedDataListReq websokectMarket = new WebsocketMarketFeedDataListReq();
                websokectMarket.Exch = pORequest.Exchange;
                websokectMarket.ExchType = pORequest.ExchangeType;
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

                do
                {
                    string jsonData = string.Empty;
                    jsonData = File.ReadAllText(getJsonValue.GetUrlFromWSFinalOutputFolderPath);
                    jswsData = JsonConvert.DeserializeObject<List<wsData>>(jsonData.ToString());

                } while (jswsData == null);

                var ltpWS = jswsData.Select(l => l.LastRate).First();

                if (pORequest.Price > 0)
                {
                    ltpWS = pORequest.Price;
                }

                //// need to get the input data from excel
                var strikerate = getDataFromExcel.Get_StrikeRate(dtSymbol_StrikeRate, pORequest.Symbol, ltpWS, pORequest.NoOfStrike, 0);
               
                var WeeklyMonthlyExpDate = getDataFromExcel.Get_WeeklyMonthlyExpDate(dtWeeklyMonthly, pORequest.Expiry, pORequest.Symbol);

                pORequest.IsIntraday = Convert.ToBoolean(myString[11]);
                pORequest.DisQty = Convert.ToInt32(myString[12]);
                pORequest.StopLossPrice = Convert.ToInt32(myString[13]);
                pORequest.TriggerPrice = Convert.ToInt32(myString[14]);
                pORequest.OrderStartTimeStamp = DateTime.Parse(myString[15]);
                pORequest.OrderEndTimeStamp = DateTime.Parse(myString[16]);
                pORequest.RemoteOrderId = Guid.NewGuid();

                var year = WeeklyMonthlyExpDate.Year;
                var month = WeeklyMonthlyExpDate.ToString("MM");
                var day = WeeklyMonthlyExpDate.ToString("dd");
                var actmonth = WeeklyMonthlyExpDate.ToString("MMM");

                int i = 0;
                while(i < pORequest.NoOfStrike)
                {
                    var PEScriptData = pORequest.Symbol + " " + day + " " + actmonth + " " + year + " " + pORequest.OptionType + " " + String.Format("{0:0.00}", strikerate) + "_" + year + month + day + "_" + "PE" + "_" + Math.Round(strikerate);
                    
                    Logger.LogWrite("ScriptCodePENoofStrike " + i + DateTime.Now + " " + PEScriptData);

                    var CEScriptData = pORequest.Symbol + " " + day + " " + actmonth + " " + year + " " + pORequest.OptionType + " " + String.Format("{0:0.00}", strikerate) + "_" + year + month + day + "_" + "CE" + "_" + Math.Round(strikerate);

                    Logger.LogWrite("ScriptCodeCENoofStrike "+ i + DateTime.Now + " " + CEScriptData);

                    strikerate = strikerate + 100;
                    i++;
                }
                
            }


            return response;
        }
    }
}
