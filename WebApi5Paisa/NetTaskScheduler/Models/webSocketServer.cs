using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebSocket4Net;

namespace NetTaskScheduler.Models
{
    public class webSocketServer
    {
        bool flag = false;
        WebSocket websocket;
        List<wsData> jswsData = new List<wsData>();
        public List<wsData> wsConnect(string wsjson)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);

            IConfiguration config = builder.Build();

            var getJsonValue = config.GetSection("GetJsonValue").Get<GetJsonValue>();
            string Token = string.Empty;
            Token = File.ReadAllText(getJsonValue.Token);
            flag = false;
            using (websocket = new WebSocket("wss://openfeed.5paisa.com/Feeds/api/chat?Value1=" + Token + "|50084790"))
            {
                websocket.Opened += new EventHandler(websocket_Opened);
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Open();

                while (websocket.State != WebSocketState.Open)
                {
                    //Console.Write("");
                }

                Console.WriteLine($"The Web Socket is Currently Processed...! {DateTime.Now}");

                websocket.Send(wsjson);
                do
                {

                } while (jswsData.Count == 0 || jswsData == null);
                return jswsData;
                //await Task.Delay(10000);
            }
        }

        private async void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (flag == false)
            {
                Console.WriteLine($"socket MESSAGE RECEIVED, sender: {sender} and eventargs e: {e.Message}");
                flag = true;

                jswsData = JsonConvert.DeserializeObject<List<wsData>>(e.Message.ToString());

                websocket.Close();
                websocket = null;

            }
        }

        private static void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine($"socket CLOSED, sender: {sender} and eventargs e: {e}");
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            
        }
    }
}
