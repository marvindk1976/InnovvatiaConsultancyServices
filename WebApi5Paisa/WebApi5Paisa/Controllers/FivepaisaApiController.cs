using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace WebApi5Paisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FivepaisaApiController : ControllerBase
    {
        private string _apiKey = "C67swLy6gPrdmUhNQA8JcrTRtPAvwDA5";
        private string EncryptionKey= "cyJ5ThTtO5uM0OSxgEqtoVSYuUVUCshR";
        private string encryptUserId= "7kSEQSA4FIS";
        private string _clientCode = "50084790";
        Token Token { get; set; }

        private readonly JsonData _JsonData;
        private readonly string  _OpenAPIURL,_Holding, _MarketStatus, _LoginCheck, _AuthToken;
        public FivepaisaApiController(IConfiguration _iConfig)
        {
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), "APICredentials.json");
            var JSON = System.IO.File.ReadAllText(folderDetails);

            _JsonData = JsonConvert.DeserializeObject<JsonData>(JSON);

            _OpenAPIURL = _iConfig.GetValue<string>("APIDetails:OpenAPIURL");
            _Holding = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:Holding");
            _MarketStatus = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:MarketStatus");
            _LoginCheck = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:TOTPLogin");
            _AuthToken = _OpenAPIURL + _iConfig.GetValue<string>("APIDetails:AuthToken");
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

            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }
    }
}
