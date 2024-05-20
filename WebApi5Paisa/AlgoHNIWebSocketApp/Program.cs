using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace AlgoHNIWebSocketApp
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
        bool flag = false;
        WebSocket websocket;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);

                IConfiguration config = builder.Build();

                var getJsonValue = config.GetSection("GetJsonWSJson").Get<GetJsonValue>();
                string Token = string.Empty;
                Token = File.ReadAllText(getJsonValue.Token); 
                flag = false;
                using (websocket = new WebSocket("wss://openfeed.5paisa.com/Feeds/api/chat?Value1=" + Token + "|50084790"))
                {
                    websocket.Opened += new EventHandler(websocket_Opened);
                    //websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
                    websocket.Closed += new EventHandler(websocket_Closed);
                    websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                    websocket.Open();

                    Console.WriteLine("------------------------------------------------");
                    while (websocket.State != WebSocketState.Open)
                    {
                        //Console.Write("");
                    }

                    Console.WriteLine($"The Web Socket is Currently Processed...! {DateTime.Now}");

                    var builder1 = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);

                    IConfiguration config1 = builder.Build();

                    var getJsonValue1 = config.GetSection("GetJsonWSJson").Get<GetJsonValue>();
                    string jsonData = string.Empty;
                    jsonData = File.ReadAllText(getJsonValue1.WsUrlFromTaskSchedulerPath);
                    websocket.Send(jsonData);
                    await Task.Delay(10000);
                }

            }
            return;
        }

        private async void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (flag == false)
            {
                Console.WriteLine($"socket MESSAGE RECEIVED, sender: {sender} and eventargs e: {e.Message}");
                flag = true;
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"WSResponse\FinalOutput.txt");

                TextWriter tw = new StreamWriter(fullPath, false);
                tw.Write(string.Empty);
                tw.Close();

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine(e.Message);
                }

                websocket.Close();
                websocket = null;

            }
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine($"socket CLOSED, sender: {sender} and eventargs e: {e}");
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);

            //IConfiguration config = builder.Build();

            //var getJsonValue = config.GetSection("GetJsonWSJson").Get<GetJsonValue>();
            //string jsonData = string.Empty;
            //jsonData = File.ReadAllText(getJsonValue.WsUrlFromTaskSchedulerPath);
            //((WebSocket)sender).Send(jsonData);
        }
    }
}
