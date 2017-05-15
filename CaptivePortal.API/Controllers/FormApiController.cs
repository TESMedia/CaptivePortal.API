using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CaptivePortal.API.Controllers
{
    [RoutePrefix("api/form")]
    public class FormApiController : ApiController
    {
        private CPDBContext db = new CPDBContext();
        [HttpPost]
        [Route("LoginFormData")]
        public string LoginFormData(FormData formdata)
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
            json = JsonConvert.SerializeObject(objLoginFormData);
            json = json.Replace("\"", "'");
            return json;
        }

        [HttpPost]
        [Route("RegisterFormData")]
        public object RegisterHtmlDynamicCode(FormData formdata)
        {
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            string formatedHtml = String.Empty;
            foreach (var item in db.FormControl.Where(m => m.FormId == jsonFormData.FormId))
            {
                item.HtmlString = item.HtmlString.Replace("\"", "'");
                formatedHtml += item.HtmlString;
            }
            formatedHtml = formatedHtml.Replace("\"", "'");
            // formatedHtml = Server.HtmlDecode(formatedHtml);
            // return formatedHtml;
            string IsPasswordRequired = jsonFormData.IsPasswordRequire.ToString();
            //var listString = new List<string>() { "isPaswordRequired", jsonFormData.IsPasswordRequire.ToString()};

            return new { formatedHtml, IsPasswordRequired };
        }
    }
}
