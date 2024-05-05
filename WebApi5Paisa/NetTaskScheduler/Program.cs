using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTaskScheduler.AlgoHNIBAL;
using NetTaskScheduler.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebApi5Paisa;

namespace NetTaskScheduler
{
    public class Program
    {

        static void Main(string[] args)
        {
            
            var Host = new HostBuilder()
            .ConfigureHostConfiguration(hConfig => { })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<MyProcessor>();
            }).UseConsoleLifetime().Build();
            Host.Run();

        }
    }

    public class MyProcessor : BackgroundService
    {
        string ClientCode = "50084790";
        string Exchange = "";
        string Key = "C67swLy6gPrdmUhNQA8JcrTRtPAvwDA5";
        string ExchangeType = ""; string Qty = ""; string Price = ""; string OrderType = ""; 
        string ScripData = ""; 
        Boolean IsIntraday = false; 
        int DisQty ; int StopLossPrice; 
        DateTime Fromdatetime;
        DateTime Todatetime;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                //string qty = ""; string Price = ""; string OrderType = ""; string ScripData = ""; DateTime Fromdatetime;
                //var Todatetime = "";

                var row1 = "";
                var row2 = "";
                string inputData = "";
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"InputTextFile\InputData.txt");
                List<string> file = File.ReadAllLines(path).ToList();
                foreach (string line in file)
                {
                    string splitline = line;
                    string[] ssize = splitline.Split(new char[0]);
                    row1 = ssize[0];
                    row2 = ssize[1];

                    //// Write file using StreamWriter
                    if (row1 == "Symbol")
                    {
                        string getSymbol = splitline.Substring(7);
                        inputData += string.Concat(string.Concat(getSymbol), ",");

                    }
                    else
                    {
                        if (row1 == "IsSuccessfull" && row2 == "0")
                        {
                            break;
                        }
                        inputData += string.Concat(string.Concat(row2), ",");
                    }
                }
                if (row1 == "IsSuccessfull" && row2 == "0")
                {

                    PORequest pORequest = new PORequest();
                    ////execute
                    //string FinalPath = @"C:\Sarvesh\R&D Project\ConsoleApp1\ConsoleApp1\FInputData\FinalInput.txt";
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"InputTextFile\FinalInput.txt");
                    TextWriter tw = new StreamWriter(fullPath, false);
                    tw.Write(string.Empty);
                    tw.Close();

                    using (StreamWriter writer = new StreamWriter(fullPath))
                    {
                        writer.WriteLine(inputData.TrimEnd(','));
                    }

                    /////replace text 
                    // string fullPath2 = @"C:\Sarvesh\R&D Project\ConsoleApp1\ConsoleApp1\InputData.txt";
                    string fullPath2 = Path.Combine(Directory.GetCurrentDirectory(), @"InputTextFile\InputData.txt");
                    using (StreamReader reader = new StreamReader(fullPath2))
                    {

                        string content = reader.ReadToEnd();
                        reader.Close();
                        content = Regex.Replace(content, "IsSuccessfull	0", "IsSuccessfull	1");

                        // Write the content back to the file
                        using (StreamWriter writer = new StreamWriter(fullPath2))
                        {
                            writer.Write(content);
                        }
                    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Request already Raised for this details.");
                    //}
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
                        pORequest.LTP = Convert.ToDouble(myString[8]);
                        pORequest.NoOfStrike = Convert.ToInt32(myString[9]);
                        pORequest.StrikeDirection = Convert.ToInt32(myString[10]);

                        //// need to get the input data from excel
                        var strikerate = getDataFromExcel.Get_StrikeRate(dtSymbol_StrikeRate, pORequest.Symbol, pORequest.LTP, pORequest.NoOfStrike, pORequest.StrikeDirection);
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
                        pORequest.ScripData = pORequest.Symbol + " " + day + " " + actmonth + " " + year + " " + pORequest.OptionType + " " + String.Format("{0:0.00}", strikerate) + "_" + year + month + day + "_" + "PE" + "_" + Math.Round(strikerate);

                        //pORequest.ScripData = pORequest.Symbol + "_" + year + actmonth + date + "_"+ cePe +"_"+ Math.Round(amt);
                    }


                    DateTime frmdatetime = DateTime.Parse(pORequest.OrderStartTimeStamp.ToString("MM/dd/yyyy HH:mm:ss"));
                    DateTime todatetime = DateTime.Parse(pORequest.OrderEndTimeStamp.ToString("MM/dd/yyyy HH:mm:ss"));

                    DateTime frmSysDate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    DateTime toSysDate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    //if (fromDate >= frmSysDate && )
                    if (frmSysDate >= frmdatetime && toSysDate <= todatetime)
                    {

                        var Request = JsonConvert.SerializeObject(new
                        {
                            head = new { key = Key },
                            body = new
                            {
                                ClientCode = pORequest.ClientCode,
                                Exchange = pORequest.Exchange,
                                ExchangeType = pORequest.ExchangeType,
                                Qty = pORequest.Qty,
                                Price = pORequest.Price,
                                OrderType = pORequest.OrderType,
                                ScripData = pORequest.ScripData,
                                IsIntraday = pORequest.IsIntraday,
                                DisQty = pORequest.DisQty,
                                StopLossPrice = pORequest.StopLossPrice
                            }

                        });
                        String result = Request.Replace("^\"|\"$", "");
                        POrderRequestApi pOrderRequestApi = new POrderRequestApi();
                        var responseData = pOrderRequestApi.CallPOrderRequestApi(result);

                        Console.WriteLine($"The Place Order request is processed Successfully...! {DateTime.Now}");
                        //Thread.Sleep(10000);
                        await Task.Delay(10000);
                    }
                    else
                    {
                        Console.WriteLine($"Date time is not Valid for Place Order request...! {DateTime.Now}");
                        await Task.Delay(10000);
                    }
                }
                else
                {
                    Console.WriteLine($"The Place Order request is Already processed.Modify the inputData { DateTime.Now}");
                    await Task.Delay(10000);
                }
            }
            return;
        }
    }
}
