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
using System.Web.Script.Serialization;
namespace CaptivePortal.API.Controllers
{
    public class GetFormDataController : Controller
    {
        DbContext db = new DbContext();
     
        [HttpPost]
        public JsonResult GetSiteDetails(FormData formdata)
        {
            FormViewModel objFormViewModel = new FormViewModel();
            try
            {
                var formResult = db.Site.Where(m => m.SiteId == formdata.SiteId).ToList();
                var jsonFormData = formResult[0];
                objFormViewModel.SiteName = db.Site.FirstOrDefault(m => m.SiteId == formdata.SiteId).SiteName;
                objFormViewModel.OrganisationName = jsonFormData.Company.Organisation.OrganisationName;
                objFormViewModel.CompanyName = jsonFormData.Company.CompanyName;
            }
            catch(Exception ex)
            {
                throw (ex);
            }
            return Json(objFormViewModel);
        }


        [HttpPost]
        public string GetSiteDetailsTest()
        {
            ReturnSiteDetails siteList = new ReturnSiteDetails();
            string orgjson = string.Empty;
            try
            {
                siteList.Sites = db.Site.ToList();
                orgjson = new JavaScriptSerializer().Serialize(siteList.Sites);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return orgjson;
        }

        [HttpGet]
        public string GetOrgDetailsTest()
        {
            ReturnSiteDetails orgList = new ReturnSiteDetails();
            string orgjson = string.Empty;
            try
            {
                orgList.Organisations = db.Organisation.ToList();
                orgjson = new JavaScriptSerializer().Serialize(orgList.Organisations);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return orgjson;
        }


        /// <summary>
        /// Get Json data for Login Form to append newly created login form by admin.
        /// </summary>
        /// <param name="formdata"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetLoginFormData(FormData formdata)
        {
            var formResult = db.Form.Where(m => m.SiteId == formdata.SiteId).ToList();
            var jsonFormData = formResult[0];
            ReturnLoginFormData objLoginFormData = new ReturnLoginFormData();
            objLoginFormData.SiteId = formdata.SiteId;
            objLoginFormData.BannerIcon = jsonFormData.BannerIcon;
            objLoginFormData.BackGroundColor = jsonFormData.BackGroundColor;
            objLoginFormData.IsPasswordRequire = jsonFormData.IsPasswordRequire;
            objLoginFormData.LoginWindowColor = jsonFormData.LoginWindowColor;
            objLoginFormData.LoginPageTitle = jsonFormData.LoginPageTitle;
            return Json(objLoginFormData);
        }

        /// <summary>
        /// Get Html code of newly created register form by admin.
        /// </summary>
        /// <param name="formdata"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRegisterHtmlDynamicCode(FormData formdata)
        {
            var formResult = db.Form.FirstOrDefault(m => m.SiteId == formdata.SiteId);
            string formatedHtml = String.Empty;
            foreach (var item in db.FormControl.Where(m => m.FormId == formResult.FormId))
            {
                item.HtmlString = item.HtmlString.Replace("\"", "'");
                formatedHtml += item.HtmlString;
            }
            //ReturnLoginFormData objLoginFormData = new ReturnLoginFormData();
            //objLoginFormData.SiteId = formdata.SiteId;
            //objLoginFormData.HtmlCodeForRegister = jsonFormData;
            //string formatedHtml = objLoginFormData.HtmlCodeForRegister;
            formatedHtml = formatedHtml.Replace("\"", "'");
            formatedHtml = Server.HtmlDecode(formatedHtml);
            return Json(formatedHtml);
        }
    }
}