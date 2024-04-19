using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebApi5Paisa
{
    public class ApiRequest
    {
        public static string SendApiRequest(string url, string Request)
        {
            string strresponse = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (!string.IsNullOrEmpty(Request))
                        streamWriter.Write(Request);
                }
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        strresponse = streamReader.ReadToEnd();
                    }

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpResponse.StatusCode, httpResponse.StatusDescription));
                    }

                    string[] reponseURI = httpResponse.Headers.AllKeys;
                    string CookieName = httpResponse.Headers.Get("Set-Cookie");

                    SetCookies(CookieName);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strresponse;

        }
        #region Testing Api Only
        public static string SendApiRequestTOTPLogin(string url, string Request)
        {
            string strresponse = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (!string.IsNullOrEmpty(Request))
                        streamWriter.Write(Request);
                }
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        strresponse = streamReader.ReadToEnd();
                    }

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpResponse.StatusCode, httpResponse.StatusDescription));
                    }


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strresponse;

        }
        public static string SendApiRequestGetOuthLogin(string url, string Request)
        {
            string strresponse = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (!string.IsNullOrEmpty(Request))
                        streamWriter.Write(Request);
                }
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        strresponse = streamReader.ReadToEnd();


                        var agr = JsonConvert.DeserializeObject<TokenResponse>(strresponse);
                        var reqtok = agr.body.AccessToken;


                        string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                        if (File.Exists(path))
                        {
                            TextWriter tw = new StreamWriter(path, false);
                            tw.Write(string.Empty);
                            tw.Close();

                            using (StreamWriter writer = new StreamWriter(path))
                            {
                                writer.WriteLine(reqtok);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strresponse;

        }
        public static string SendApiRequestHolding(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    var objJsonData = jsonData["body"]["Data"];
                    IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }

        public static string SendApiRequestMargin(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }
        public static string SendApiRequestPosition(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }

        public static string SendApiRequestMarketStatus(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }

        public static string SendApiRequestPlaceOrderRequest(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }

        public static string SendApiRequestOrderCancel(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }
        public static string SendApiRequestOrderBook(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }
        public static string SendApiRequestTradeBook(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }
        public static string SendApiRequestTradeBookHistory(string url, string Request, string Type = "Openapi")
        {
            string Json = "";
            string BearerToken = "";
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"AuthToken\AuthKey.txt");

                if (File.Exists(path))
                {
                    BearerToken = File.ReadAllText(path);
                }

                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + BearerToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";


                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;


                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();

                    /////working as required
                    var jsonData = JObject.Parse(Json);
                    //var objJsonData = jsonData["body"]["Data"];
                    //IEnumerable<Holding> result = JsonConvert.DeserializeObject<IEnumerable<Holding>>(objJsonData.ToString());
                    //var valuesList = result.Select(v => v.AvgRate).FirstOrDefault();

                    ////working in required for json array
                    ////var jsonData = JObject.Parse(Json);
                    ////var objJsonData = jsonData["body"]["Data"];
                    ////IEnumerable<MarketSts> result = JsonConvert.DeserializeObject<IEnumerable<MarketSts>>(objJsonData.ToString());
                    ////var valuesList = result.ToList();
                    ////foreach (var member in valuesList)
                    ////{
                    ////    Console.WriteLine(member.Exch);
                    ////    Console.WriteLine(member.ExchDescription);
                    ////    Console.WriteLine(member.ExchType);
                    ////    Console.WriteLine(member.MarketStatus);
                    ////}
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json;

        }


        #endregion Testing Api Only
        public static string SendApiRequestCookies(string url, string Request, string Type = "Openapi")
        {
            string strresponse = "";
            try
            {
                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Headers.Add("Authorization", "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjUwMDg0NzkwIiwicm9sZSI6IjIyNTU5IiwiU3RhdGUiOiIiLCJSZWRpcmVjdFNlcnZlciI6IkEiLCJuYmYiOjE3MTI3Mjg2MDMsImV4cCI6MTcxMjc3Mzc5OSwiaWF0IjoxNzEyNzI4NjAzfQ._yUkCRWzPvLIevA1FMdpvI8LOqJPiwC8i5nnE-hTxRQ");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";

                byte[] byteArray = Encoding.UTF8.GetBytes(Request);
                httpWebRequest.ContentLength = byteArray.Length;

                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    strresponse = reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                return "PostError:" + ex.Message;
            }

            return strresponse;

        }

        public static void SetCookies(string CookieName)
        {
            var value = CookieName.Split(';');

            string Fivepaisacookieres = Array.Find(value, ele => ele.Contains("5paisacookie", StringComparison.Ordinal));

            if (!string.IsNullOrEmpty(Fivepaisacookieres))
            {
                var Fivepaisacookiefinal = Fivepaisacookieres.Split('=');
                string FivepaisacookieCookieValue = "5paisacookie=" + Fivepaisacookiefinal[1];
                AddDataToXML("5paisacookie", FivepaisacookieCookieValue);
            }

            string JwtTokencookieres = Array.Find(value, ele => ele.Contains("JwtToken", StringComparison.Ordinal));

            if (!string.IsNullOrEmpty(JwtTokencookieres))
            {
                var JwtTokencookiefinal = JwtTokencookieres.Split('=');
                string JwtTokenCookieValue = JwtTokencookiefinal[1];
                AddDataToXML("JwtToken", JwtTokenCookieValue);
            }

            string ASPXAUTHcookieres = Array.Find(value, ele => ele.Contains(".ASPXAUTH", StringComparison.Ordinal));

            if (!string.IsNullOrEmpty(ASPXAUTHcookieres))
            {
                var ASPXAUTHcookiefinal = ASPXAUTHcookieres.Split('=');
                string ASPXAUTHCookieValue = ASPXAUTHcookiefinal[1];
                AddDataToXML("ASPXAUTH", ASPXAUTHCookieValue);
            }
        }

        public static void AddDataToXML(string Name, string Value)
        {
            string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "CookiesFile.xml");
            XmlTextReader xmlreader = new XmlTextReader(xmlFilePath);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlreader);

            DataRow row = ds.Tables[0].Select("Name = '" + Name + "'").FirstOrDefault();

            int xmlRow = ds.Tables[0].Rows.IndexOf(row);
            ds.Tables[0].Rows[xmlRow]["Value"] = Value;
            xmlreader.Close();

            ds.WriteXml(xmlFilePath);

        }

        public static string GetCookiesByName(string Name)
        {
            string value = string.Empty;

            string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "CookiesFile.xml");
            XmlTextReader xmlreader = new XmlTextReader(xmlFilePath);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlreader);

            DataRow row = ds.Tables[0].Select("Name = '" + Name + "'").FirstOrDefault();

            int xmlRow = ds.Tables[0].Rows.IndexOf(row);
            value = Convert.ToString(ds.Tables[0].Rows[xmlRow]["Value"]);
            xmlreader.Close();

            return value;
        }

    }
}
