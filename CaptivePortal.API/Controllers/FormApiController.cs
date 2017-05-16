using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public HttpResponseMessage RegisterHtmlDynamicCode(FormData formdata)
        {
            //string json = "";
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            var formControlResult = db.FormControl.Where(m => m.FormId == jsonFormData.FormId).ToList();

            //var jsonFormControlData = formControlResult[0];

            ReturnRegisterFormListData objReturnRegisterFormListData = new ReturnRegisterFormListData();
            objReturnRegisterFormListData.ReteurnRegisterFormList = new List<ReturnRegisterFormData>();
            var jsonRegisterFormData = (from item in formControlResult
                                        select new ReturnRegisterFormData()
                                        {
                                            SiteId = formdata.SiteId,
                                            ColumnName = item.LabelName,
                                            LabelNameToDisplay = item.LabelNameToDisplay,
                                            IsMandetory = item.IsMandetory,
                                            IsPasswordRequired = db.Form.FirstOrDefault(m => m.FormId == jsonFormData.FormId).IsPasswordRequire
                                        }).ToList();
            objReturnRegisterFormListData.ReteurnRegisterFormList.AddRange(jsonRegisterFormData);
            string json = JsonConvert.SerializeObject(objReturnRegisterFormListData);
            //json = json.Replace("\"", "");
            //return json;

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            //string columnName = jsonFormControlData.LabelName;
            //string LabelNameToDisplay = jsonFormControlData.LabelNameToDisplay;
            //string IsMandetory = jsonFormControlData.IsMandetory.ToString();
            //string IsPasswordRequired = jsonFormData.IsPasswordRequire.ToString();
            //return new { columnName, IsMandetory, IsPasswordRequired , LabelNameToDisplay };

            //string formatedHtml = String.Empty;
            //foreach (var item in db.FormControl.Where(m => m.FormId == jsonFormData.FormId))
            //{
            //    item.HtmlString = item.HtmlString.Replace("\"", "'");
            //    formatedHtml += item.HtmlString;
            //}
            //formatedHtml = formatedHtml.Replace("\"", "'");
            // formatedHtml = Server.HtmlDecode(formatedHtml);
            // return formatedHtml;
            //string IsPasswordRequired = jsonFormData.IsPasswordRequire.ToString();
            //var listString = new List<string>() { "isPaswordRequired", jsonFormData.IsPasswordRequire.ToString()};

            //return new { formatedHtml, IsPasswordRequired };
        }
    }
}
