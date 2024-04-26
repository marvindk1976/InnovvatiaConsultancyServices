using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class GetJsonValue
    {
        public string BaseUrl { get; set; }
        public string POrderUrl { get; set; }
    }
}