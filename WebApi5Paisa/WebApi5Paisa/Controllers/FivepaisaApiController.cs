using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using WebApi5Paisa.DAL;
using WebApi5Paisa.Models;

namespace WebApi5Paisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FivepaisaApiController : ControllerBase
    {
        # region key
        private string _apiKey = "C67swLy6gPrdmUhNQA8JcrTRtPAvwDA5";
        private string EncryptionKey = "cyJ5ThTtO5uM0OSxgEqtoVSYuUVUCshR";
        private string encryptUserId = "7kSEQSA4FIS";
        private string _clientCode = "50084790";
        Token Token { get; set; }
        private string connectionString;
        EmployeeDAL employeeDAL = new EmployeeDAL();
        string msg;
        private readonly JsonData _JsonData;
        private readonly string _OpenAPIURL, _Holding,
            _MarketStatus, _LoginCheck, _AuthToken, _PlaceOrderRequest,
            _Margin, _Position, _OrderCancel, _OrderBook, _TradeBook, _TradeBookHistory, _OrderStatus,_WS,_Historical;
        #endregion
        public FivepaisaApiController(IConfiguration _iConfig)
        {
            #region config
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), "APICredentials.json");
            var JSON = System.IO.File.ReadAllText(folderDetails);

            _JsonData = JsonConvert.DeserializeObject<JsonData>(JSON);

            _OpenAPIURL = _iConfig.GetValue<string>("APIDetails:OpenAPIURL");
            _Holding = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:Holding");
            _Margin = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:Margin");
            _MarketStatus = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:MarketStatus");
            _LoginCheck = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:TOTPLogin");
            _AuthToken = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:AuthToken");
            _PlaceOrderRequest = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:PlaceOrderRequest");
            _Position = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:Position");
            _OrderCancel = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:CancelOrderRequest");
            _OrderBook = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:OrderBook");
            _TradeBook = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:TradeBook");
            _TradeBookHistory = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:TradeBookHistory");
            _OrderStatus = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:OrderStatus");
            _Historical = _iConfig.GetValue<string>("APIDetails:Historical");
            _WS = _iConfig.GetValue<string>("APIDetails:WbSocketURl");

            connectionString = _iConfig.GetConnectionString("ConnectionString");
            #endregion
        }
        [HttpGet]
        [Route("TOTPLogin")]
        public OutputBaseClass TOTPLogin(string _TOTP)
        {
            OutputBaseClass res = new OutputBaseClass();
            _JsonData.Head.requestCode = _JsonData.RequestCode.LoginCheck;

            try
            {
                string _EmailId = "8850999153";
                string _Pin = "030906";
                TokenResponse agr = new TokenResponse();
                string URL = _LoginCheck;
                var dataStringSession = JsonConvert.SerializeObject(new
                {
                    head = new { Key = _apiKey },
                    body = new { Email_ID = _EmailId, TOTP = _TOTP, PIN = _Pin }

                });
                var json = ApiRequest.SendApiRequestTOTPLogin(URL, dataStringSession);
                agr = JsonConvert.DeserializeObject<TokenResponse>(json);
                var reqtok = agr.body.RequestToken;
                //GetOuthLogin(agr.body.RequestToken);

                string URLtoken = _AuthToken;
                var dataString = JsonConvert.SerializeObject(new
                {
                    head = new { Key = _apiKey },
                    body = new { RequestToken = agr.body.RequestToken, EncryKey = EncryptionKey, UserId = encryptUserId }

                });
                var jsonData = ApiRequest.SendApiRequestGetOuthLogin(URLtoken, dataString);
                agr = JsonConvert.DeserializeObject<TokenResponse>(jsonData);
                if (agr.body.Status == "0")
                {

                    res.TokenResponse = agr.body;
                    res.status = agr.body.Status;
                    res.http_error = agr.body.Message;
                    res.http_code = agr.errorcode;
                    this.Token = agr.body;
                }
                else
                {

                    res.status = agr.body.Status;
                    res.http_error = agr.body.Message;
                    ///////Log Write in txt
                    Logger.LogWrite("TOTPLogin Successfully" + DateTime.Now);
                }

            }
            catch (Exception ex)
            {
                res.http_error = ex.Message;
            }
            return res;
        }


        [HttpGet]
        [Route("Holding")]
        public string Holding()
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {
                _JsonData.Head.requestCode = _JsonData.RequestCode.Holding;

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestHolding(_Holding, result);

                ///////Log Write in txt
                Logger.LogWrite("Holding function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }


        [HttpGet]
        [Route("Margin")]
        public string Margin()
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestMargin(_Margin, result);

                ///////Log Write in txt
                Logger.LogWrite("Margin function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        [HttpGet]
        [Route("Position")]
        public string Position()
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestPosition(_Position, result);

                ///////Log Write in txt
                Logger.LogWrite("Position function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        [HttpGet]
        [Route("MarketStatus")]
        public string MarketStatus()
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {
                _JsonData.Head.requestCode = _JsonData.RequestCode.MarketStatus;

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestMarketStatus(_MarketStatus, result);
                ///////Log Write in txt
                Logger.LogWrite("MarketStatus function data fetched" + DateTime.Now);

            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        [HttpGet]
        [Route("PlaceOrderRequest")]
        public string PlaceOrderRequest([FromQuery] PlaceOrderRequest pORequest)
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {
                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
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
                        StopLossPrice = pORequest.StopLossPrice,
                        AppSource = pORequest.AppSource,
                        RemoteOrderID = Guid.NewGuid()
                    }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestPlaceOrderRequest(_PlaceOrderRequest, result);
                ///////Log Write in txt
                Logger.LogWrite("PlaceOrderRequest function data fetched" + response);
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }
        [HttpPost]
        [Route("POrderRequest")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string POrderRequest(JsonObject jsonObject)
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            string responseSale = "";
            string responseOrderBook = "";
            try
            {

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestPlaceOrderRequest(_PlaceOrderRequest, jsonObject.ToString());
                ///////Log Write in txt
                Logger.LogWrite("PlaceOrderRequest function data fetched." + response);

                var jsonData = JObject.Parse(response);
                var objJsonData = jsonData["body"];
                JsonObj jsData = JsonConvert.DeserializeObject<JsonObj>(objJsonData.ToString());
                if (jsData.Message.ToLower() == "success" && (jsData.BrokerOrderID != null || jsData.BrokerOrderID != 0))
                {
                    //var Request = JsonConvert.SerializeObject(new
                    //{
                    //    head = new { key = _apiKey },
                    //    body = new
                    //    {
                    //        ClientCode = _clientCode,
                    //        OrdStatusReqList = new[] {
                    //    new {Exch = jsData.Exch , RemoteOrderID = jsData.RemoteOrderID}
                    //    }}
                    //});

                    var Request = JsonConvert.SerializeObject(new
                    {
                        head = new { key = _apiKey },
                        body = new
                        {
                            ClientCode = _clientCode

                        }
                    });
                    String result = Request.Replace("^\"|\"$", "");

                    //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                    responseOrderBook = ApiRequest.SendApiRequestOrderBook(_OrderBook, result);

                    ///////Log Write in txt
                    Logger.LogWrite("OrderBookResponse : " + responseOrderBook);

                    //var jsonOrderBook = JObject.Parse(response);
                    //var objOrderBook = jsonOrderBook["body"]["OrderBookDetail"];
                    dynamic objOrderBook = JObject.Parse(response);
                    var objOrderBk = objOrderBook["body"]["OrderBookDetail"];

                    List<OrderBook> objJsonOrderBook = JsonConvert.DeserializeObject<List<OrderBook>>(objOrderBk);

                    var OBbrokerId = objJsonOrderBook.Find(s => Convert.ToInt32(s.BrokerOrderId) == Convert.ToInt32(jsData.BrokerOrderID));
                    int OBbrokerids = Convert.ToInt32(OBbrokerId.BrokerOrderId);
                    var OrderId = Convert.ToInt64(OBbrokerId.ExchOrderID);
                    var OrderStatus = OBbrokerId.OrderStatus;

                    if ((OBbrokerids != null || OBbrokerids != 0) && OrderStatus.ToString().ToLower() == "complete")
                    {
                        
                        //PORequest jsondataPOSell = JsonConvert.DeserializeObject<PORequest>(jsonObject.ToString());
                        //jsondataPOSell.OrderType = "Sell";
                        //var jsonString = JsonConvert.SerializeObject(jsondataPOSell);

                        Root jsondataPOSell = JsonConvert.DeserializeObject<Root>(jsonObject.ToString());
                        jsondataPOSell.body.OrderType = "Sell";
                        var jsonString = JsonConvert.SerializeObject(jsondataPOSell);

                        Logger.LogWrite("Exit Order Request : " + jsonString);

                        responseSale = ApiRequest.SendApiRequestPlaceOrderRequest(_PlaceOrderRequest, jsonString);

                        Logger.LogWrite("Exit Order Response : " + responseSale);
                    }
                    else if ((OBbrokerids != null || OBbrokerids != 0) && (OrderId != null || OrderId != 0) && OrderStatus.ToString().ToLower() == "pending")
                    {
                        Logger.LogWrite("Order Cancel Request : " + OrderId.ToString());
                        responseSale = ApiRequest.SendApiRequestOrderCancel(_OrderCancel, OrderId.ToString());
                        Logger.LogWrite("Order Cancel Request : " + responseSale);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        [HttpGet]
        [Route("OrderCancel")]
        public string OrderCancel([FromQuery] string exchOrderID)
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ExchOrderID = exchOrderID }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestOrderCancel(_OrderCancel, result);

                ///////Log Write in txt
                Logger.LogWrite("OrderCancel function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        [HttpGet]
        [Route("OrderBook")]
        public string OrderBook()
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestOrderBook(_OrderBook, result);

                ///////Log Write in txt
                Logger.LogWrite("OrderBook function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        [HttpGet]
        [Route("TradeBook")]
        public string TradeBook()
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestTradeBook(_TradeBook, result);

                ///////Log Write in txt
                Logger.LogWrite("TradeBook function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        [HttpGet]
        [Route("TradeBookHistory")]
        public string TradeBookHistory([FromQuery] string _exchOrderID)
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode, ExchOrderID = _exchOrderID }

                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestTradeBookHistory(_TradeBookHistory, result);

                ///////Log Write in txt
                Logger.LogWrite("TradeBookHistory function data fetched" + DateTime.Now);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
        [HttpGet]
        [Route("OrderStatus")]
        public string OrderStatus([FromQuery] string _Exch,string _RemoteOrderID)
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {

                var Request = JsonConvert.SerializeObject(new
                {
                    head = new { key = _apiKey },
                    body = new { ClientCode = _clientCode,
                    OrdStatusReqList = new[] {
                        new {Exch = _Exch , RemoteOrderID = _RemoteOrderID}                        
                    }}
                });
                String result = Request.Replace("^\"|\"$", "");

                //response = ApiRequest.SendApiRequestCookies(_Holding, result);
                response = ApiRequest.SendApiRequestOrderStatus(_OrderStatus, result);

                ///////Log Write in txt
                Logger.LogWrite("OrderStatus function data fetched" + response);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        [HttpGet]
        [Route("WebsocketAPi")]
        public string WebsocketAPi(string Exch,string ExchType ,int ScripCode)
        {
            ResponseModel objResponseModel = new ResponseModel();
            try
            {
                WebsocketMarketFeedDataListReq websokectMarket = new WebsocketMarketFeedDataListReq();
                websokectMarket.Exch = Exch;
                websokectMarket.ExchType = ExchType;
                websokectMarket.ScripCode = ScripCode;

                List<WebsocketMarketFeedDataListReq> websokectMarketList = new List<WebsocketMarketFeedDataListReq>();

                websokectMarketList.Add(websokectMarket);

                var dataStringSession = JsonConvert.SerializeObject(new
                {
                    Method = "MarketFeedV3",
                    Operation = "Subscribe",
                    ClientCode = _clientCode,
                    MarketFeedData = websokectMarketList,
                });

                WebsocketServer.Connect(_WS , _clientCode, dataStringSession);

            }
            catch (Exception)
            {
                throw;
            }
            var ws = "WebSocket Connected fetch data in Text File";
            return ws;
        }

        [HttpGet]
        [Route("historical")]
        public string historical([FromQuery] string _Exch, string _ExchType, int ScripCode, string Interval, string FromDate, string EndDate)
        {
            ResponseModel objResponseModel = new ResponseModel();
            string response = "";
            try
            {
                string URL = _Historical + "V2/historical" + "/" + _Exch + "/" + _ExchType + "/" + ScripCode + "/" + Interval + "?" + "from" + "=" + FromDate + "&" + "end" + "=" + EndDate;

                response = ApiRequest.SendApiRequesthistorical(URL);

                ///////Log Write in txt
                 Logger.LogWriteInfo("historical function data fetched" + response);

                ///////data is saved in db
                //Employee employee = new Employee();
                //try
                //{
                //    employee.flag = "insert";
                //    employeeDAL.Empdml(employee, out msg, connectionString);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }
    }
}
