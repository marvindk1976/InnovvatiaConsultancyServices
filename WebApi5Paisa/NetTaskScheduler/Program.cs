using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            string qty = ""; string Price = ""; string OrderType = ""; string ScripData = ""; DateTime Fromdatetime;
            var Todatetime = "";

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
                if (row1 == "IsSuccessfull" && row2 == "0")
                {
                    break;
                }

                inputData += string.Concat(string.Concat(row2), ",");
            }
            if (row1 == "IsSuccessfull" && row2 == "0")
            {

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
                var Host = new HostBuilder()
                .ConfigureHostConfiguration(hConfig => { })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<MyProcessor>();
                }).UseConsoleLifetime().Build();
                Host.Run();
            }
            else
            {
                Console.WriteLine("Request already Raised for this details.");
            }

        }
    }

    public class MyProcessor : BackgroundService
    {
        string Exchange = "";
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

                string fullPath3 = Path.Combine(Directory.GetCurrentDirectory(), @"InputTextFile\FinalInput.txt");
                string[] lines = File.ReadAllLines(fullPath3);
                foreach (string line in lines)
                {
                    //var qty = line.Split(1) ;
                    // splitline.Split(new char[0])
                    var myString = line.Split(',');
                    Exchange = myString[0];
                    ExchangeType = myString[1];
                    Qty = myString[2];
                    Price = myString[3];
                    OrderType = myString[4];
                    ScripData = myString[5];
                    IsIntraday = Convert.ToBoolean(myString[6]);
                    DisQty = Convert.ToInt32(myString[7]);
                    StopLossPrice = Convert.ToInt32(myString[8]);
                    Fromdatetime = DateTime.Parse(myString[9]);
                    Todatetime = DateTime.Parse(myString[10]);
                }


                DateTime frmdatetime = DateTime.Parse(Fromdatetime.ToString("MM/dd/yyyy HH:mm:ss"));
                DateTime todatetime = DateTime.Parse(Todatetime.ToString("MM/dd/yyyy HH:mm:ss"));

                DateTime frmSysDate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                DateTime toSysDate = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                //if (fromDate >= frmSysDate && )
                if (frmSysDate >= frmdatetime && toSysDate <= todatetime)
                {
                    
                    var Request = JsonConvert.SerializeObject(new
                    {
                        head = new { key = "C67swLy6gPrdmUhNQA8JcrTRtPAvwDA5" },
                        body = new
                        {
                            ClientCode = "50084790",
                            Exchange = Exchange,
                            ExchangeType = ExchangeType,
                            Qty = Qty,
                            Price = Price,
                            OrderType = OrderType,
                            ScripData = ScripData,
                            IsIntraday = IsIntraday,
                            DisQty = DisQty,
                            StopLossPrice = StopLossPrice
                        }

                    });
                    String result = Request.Replace("^\"|\"$", "");
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:26328/");
                    var response = await client.PostAsync("api/FivepaisaApi/POrderRequest", new StringContent(result, Encoding.UTF8, "application/json"));
                    if (response != null)
                    {
                        Console.WriteLine(response.ToString());
                    }
                    Console.WriteLine($"The Current date is {DateTime.Now}");
                    await Task.Delay(10000);
                }
                else
                {
                    Console.WriteLine($"The Current date time is {DateTime.Now} not Valid");
                    await Task.Delay(10000);
                }
            }
            return;
        }
    }
}
