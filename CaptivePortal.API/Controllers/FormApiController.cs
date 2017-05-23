using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Script.Serialization;

namespace CaptivePortal.API.Controllers
{
    [RoutePrefix("api/form")]
    public class FormApiController : ApiController
    {
        private CPDBContext db = new CPDBContext();
        [HttpPost]
        [Route("LoginFormData")]
        public HttpResponseMessage LoginFormData(FormData formdata)
        {
            var json = "";
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            ReturnLoginFormData objLoginFormData = new ReturnLoginFormData();
            objLoginFormData.SiteId = formdata.SiteId;
            objLoginFormData.BannerIcon = jsonFormData.BannerIcon;
            objLoginFormData.BackGroundColor = jsonFormData.BackGroundColor;
            objLoginFormData.IsPasswordRequire = jsonFormData.IsPasswordRequire;
            objLoginFormData.LoginWindowColor = jsonFormData.LoginWindowColor;
            objLoginFormData.LoginPageTitle = jsonFormData.LoginPageTitle;
            //json = JsonConvert.SerializeObject(objLoginFormData);
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objLoginFormData), Encoding.UTF8, "application/json")
            };
        }

        [HttpPost]
        [Route("RegisterFormData")]
        public HttpResponseMessage RegisterHtmlDynamicCode(FormData formdata)
        {
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            var formControlResult = db.FormControl.Where(m => m.FormId == jsonFormData.FormId).ToList();

            string TandD= db.Site.FirstOrDefault(m => m.SiteId == formdata.SiteId).TermsAndCondDoc;


            ReturnRegisterFormListData objReturnRegisterFormListData = new ReturnRegisterFormListData();
            objReturnRegisterFormListData.ReteurnRegisterFormList = new List<ReturnRegisterFormData>();
            var jsonRegisterFormData = (from item in formControlResult
                                        select new ReturnRegisterFormData()
                                        {
                                            //SiteId = formdata.SiteId,
                                            ColumnName = item.LabelName,
                                            LabelNameToDisplay = item.LabelNameToDisplay,
                                            IsMandetory = item.IsMandetory,
                                            //TandCfile=fileString
                                            //IsPasswordRequired = db.Form.FirstOrDefault(m => m.FormId == jsonFormData.FormId).IsPasswordRequire
                                        }).ToList();
            objReturnRegisterFormListData.ReteurnRegisterFormList.AddRange(jsonRegisterFormData);
            string json = JsonConvert.SerializeObject(objReturnRegisterFormListData);
            bool IsPasswordRequire = jsonFormData.IsPasswordRequire;
            var Request = new HttpRequestMessage();
            Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            Request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var formJsonResult = JsonConvert.DeserializeObject(json);
            return Request.CreateResponse(HttpStatusCode.OK, new { formJsonResult, IsPasswordRequire , TandD}, JsonMediaTypeFormatter.DefaultMediaType);
            
        }


        [HttpPost]
        [Route("file")]
        public HttpResponseMessage Post()
        {
            var path = @"D:\PlanetsBrainProfile.docx";
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }
        //[HttpPost]
        //[Route("convert")]
        //public static byte[] ConvertStringToByteArray(ReturnRegisterFormData uu)
        //{

        //    string stringToConvert = uu.TandCfile;
        //    return (new UnicodeEncoding()).GetBytes(stringToConvert);
        //}

    }
}
