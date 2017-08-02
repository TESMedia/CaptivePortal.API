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
        private DbContext db = new DbContext();
        [HttpPost]
        [Route("LoginFormData")]
        public HttpResponseMessage LoginFormData(FormData formdata)
        {
            ReturnLoginFormData objLoginFormData = new ReturnLoginFormData();
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            try
            {
                var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
                var jsonFormData = formResult[0];
                objLoginFormData.SiteId = formdata.SiteId;
                objLoginFormData.BannerIcon = jsonFormData.BannerIcon;
                objLoginFormData.BackGroundColor = jsonFormData.BackGroundColor;
                objLoginFormData.IsPasswordRequire = jsonFormData.IsPasswordRequire;
                objLoginFormData.LoginWindowColor = jsonFormData.LoginWindowColor;
                objLoginFormData.LoginPageTitle = jsonFormData.LoginPageTitle;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

            string TandD = db.Site.FirstOrDefault(m => m.SiteId == formdata.SiteId).TermsAndCondDoc;
            string BannerIcon = jsonFormData.BannerIcon;
            string RegisterPageTitle = jsonFormData.RegistrationPageTitle;

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
            return Request.CreateResponse(HttpStatusCode.OK, new { formJsonResult, IsPasswordRequire, BannerIcon, RegisterPageTitle }, JsonMediaTypeFormatter.DefaultMediaType);

        }

        [HttpPost]
        [Route("TermAndConditionContent")]
        public HttpResponseMessage GetTermAndConditionContent(FormData formdata)
        {
            string TandD = null;
            try
            {
                var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
                var jsonFormData = formResult[0];
                var formControlResult = db.FormControl.Where(m => m.FormId == jsonFormData.FormId).ToList();
                TandD = db.Site.FirstOrDefault(m => m.SiteId == formdata.SiteId).TermsAndCondDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { TandD }, JsonMediaTypeFormatter.DefaultMediaType);
        }

        [HttpPost]
        [Route("ManagePromotion")]
        public HttpResponseMessage GetPromotionalData(FormData formdata)
        {
            ReturnPromationalData PromotinalData = new ReturnPromationalData();
            if (formdata.SiteId != 0)
            {
                var formResult = db.ManagePromotion.Where(m => m.SiteId == formdata.SiteId).ToList();
                var jsonFormData = formResult[0];

                PromotinalData.SuccessPageOption = jsonFormData.SuccessPageOption;
                PromotinalData.WebPageURL = jsonFormData.WebPageURL;
                PromotinalData.OptionalPictureForSuccessPage = jsonFormData.OptionalPictureForSuccessPage;
                return Request.CreateResponse(HttpStatusCode.OK, new { PromotinalData }, JsonMediaTypeFormatter.DefaultMediaType);
            }
            else
            {
                string err = "SiteId required.";
                return Request.CreateResponse(HttpStatusCode.OK, err);
            }

        }
    }
}
