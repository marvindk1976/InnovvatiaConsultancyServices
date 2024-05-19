using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApi5Paisa.Models;

namespace NetTaskScheduler
{
    public class POrderRequestApi
    {
        public async Task CallPOrderRequestApi(string json)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("config.json", optional: false);

            IConfiguration config = builder.Build();

            var getJsonValue = config.GetSection("GetJsonValue").Get<GetJsonValue>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(getJsonValue.BaseUrl);
            await client.PostAsync(getJsonValue.POrderUrl, new StringContent(json, Encoding.UTF8, "application/json"));
            //if (response != null)
            //{
            //    Console.WriteLine(response.ToString());
            //}
        }
        public async Task CallWSRequestApi(string Exchange,string ExchangeType,string scode)
        {
            try
            {
                object result;
                var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("config.json", optional: false);

                IConfiguration config = builder.Build();

                var getJsonValue = config.GetSection("GetJsonValue").Get<GetJsonValue>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(getJsonValue.BaseUrl);
                //var response = await client.GetAsync(getJsonValue.WsUrl +"?Exch=B&ExchType=D&ScripCode=46932");


                //HttpClient client = new HttpClient();
                //var response = client.GetStringAsync(getJsonValue.WsUrl + "?Exch=B&ExchType=D&ScripCode=46932").Result;
                await client.GetStringAsync(getJsonValue.WsUrl + "?" + "Exch=" + Exchange + "&" + "ExchType=" + ExchangeType + "&" + "ScripCode=" + scode);
                //client.Dispose();
                //result = response.ToString();

            }
            catch(Exception ex) 
            {
                
            }
           
        }
    }

    public class GetJsonValue
    {
        public string BaseUrl { get; set; }
        public string POrderUrl { get; set; }
        public string WsUrl { get; set; }
        public string GetUrlFromWSFinalOutputFolderPath { get; set; }
    }
}