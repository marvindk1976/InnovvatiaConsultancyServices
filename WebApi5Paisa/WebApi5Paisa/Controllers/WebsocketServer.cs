using Microsoft.Extensions.Logging;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using WebSocket4Net;
namespace WebApi5Paisa.Controllers
{
    class WebsocketServer
    {
        string jsonData = string.Empty;
        public static void Connect(string uri,string clientcode, string stringtoSend)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"ws\wsData.txt");
            TextWriter tw = new StreamWriter(fullPath, false);
            tw.Write(string.Empty);
            tw.Close();

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                writer.WriteLine(stringtoSend.Trim());
            }
            string BearerToken = "";
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");
            
            if (File.Exists(path))
            {
                BearerToken = File.ReadAllText(path);
            }
            string url = uri + BearerToken + "|" + clientcode;
            using (WebSocket websocket = new WebSocket(url))
            {
                websocket.Opened += new EventHandler(websocket_Opened);
                //websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Open();

                //Console.WriteLine("before loop");
                while (websocket.State != WebSocketState.Open)
                {
                    //Console.Write("");
                }
                
                websocket.Send(stringtoSend);
                Console.ReadKey();
            }
        }

        private static void websocket_Opened(object sender, EventArgs e)
        {
            //Console.WriteLine($"socket OPENED, sender: {sender} and eventargs e: {e}");
            string jsonData = string.Empty;
            jsonData = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), @"ws\wsData.txt"));
            
            ((WebSocket)sender).Send(jsonData);
        }

        //private static void websocket_Error(object sender, System.IO.ErrorEventArgs e)
        //{
        //    Console.WriteLine($"socket ERROR, sender: {sender} and eventargs e: {e.GetException(}");
        //}

        private static void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine($"socket CLOSED, sender: {sender} and eventargs e: {e}");
        }

        private static void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"socket MESSAGE RECEIVED, sender: {sender} and eventargs e: {e.Message}");
            Logger logRun = new Logger();
            logRun.LogWriteWS(e.Message);
            //((WebSocket)sender).Close();
        }
    }
}