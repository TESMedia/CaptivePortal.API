using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CaptivePortal.API.Controllers
{
    public class GenerateHtmlCodeController : Controller
    {

        CPDBContext db = new CPDBContext();
        [HttpPost]
        public string GenerateHtmlForLoginTest(GeneratingHtmlViewModel htmlViewModel)
        {
            var formResult = db.Form.Where(m => m.SiteId == htmlViewModel.SiteId).ToList();
            string html = null;
            HtmlWeb web = new HtmlWeb();
            string urlAddress = htmlViewModel.LoginUrl;
            HtmlDocument document = web.Load(urlAddress);

            var body = document.DocumentNode.Descendants()
                                .Where(n => n.Id == "usr_password")
                                .FirstOrDefault();
            
            var checkBoxValue = formResult[0].IsPasswordRequire;
            if (checkBoxValue == true)
            {
                body.Attributes.Add("style", "display:none");
             html = document.DocumentNode.OuterHtml;
            Form objForm = new Form
            {
                HtmlCodeForLogin= html,
                SiteId= htmlViewModel.SiteId
            };
            db.Form.Add(objForm);
            db.SaveChanges();
            }
            else
            {
                return html;
            }
            return html;
        }
        [HttpPost]
        public JsonResult GetLoginFormData(FormData formdata)
        {
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            ReturnLoginFormData objLoginFormData =new ReturnLoginFormData();
            objLoginFormData.SiteId = formdata.SiteId;
            objLoginFormData.BannerIcon = jsonFormData.BannerIcon;
            objLoginFormData.BackGroundColor = jsonFormData.BackGroundColor;
            objLoginFormData.IsPasswordRequire =jsonFormData.IsPasswordRequire;
            objLoginFormData.LoginWindowColor = jsonFormData.LoginWindowColor;
            objLoginFormData.LoginPageTitle = jsonFormData.LoginPageTitle;
            
            return Json(objLoginFormData);
        }
        [HttpPost]
        public JsonResult GetRegisterHtmlDynamicCode(FormData formdata)
        {
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            ReturnLoginFormData objLoginFormData = new ReturnLoginFormData();
            objLoginFormData.SiteId = formdata.SiteId;
            objLoginFormData.HtmlCodeForRegister = jsonFormData.HtmlCodeForLogin;
            string formatedHtml = objLoginFormData.HtmlCodeForRegister;
            formatedHtml = formatedHtml.Replace("\"","'");
            formatedHtml = Server.HtmlDecode(formatedHtml);
            return Json(formatedHtml);
        }




        // GET: GenerateHtmlCode
        public ActionResult Index()
        {
            return View();
        }
    }
}