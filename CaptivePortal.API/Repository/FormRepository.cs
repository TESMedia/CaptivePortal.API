using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Repository
{
    public class FormRepository
    {

        private int RetCode { get; set; }
        private string RetMsg { get; set; }
        private DbContext db = null;
        string bannerPath = null;

        public FormRepository()
        {
            db = new DbContext();
        }
        public bool CreateForm(FormViewModel inputData)
        {
            try
            {
                Form objForm = new Form
                {
                    SiteId = db.Site.FirstOrDefault(m => m.SiteName == inputData.SiteName).SiteId,
                    BannerIcon = bannerPath,
                    BackGroundColor = inputData.BackGroundColor,
                    LoginWindowColor = inputData.LoginWindowColor,
                    IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
                    LoginPageTitle = inputData.LoginPageTitle,
                    RegistrationPageTitle = inputData.RegistrationPageTitle,
                };
                db.Form.Add(objForm);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public bool UpdateForm(FormViewModel inputData)
        {
            try
            {
                Form objForm = new Form
                {
                    FormId = inputData.FormId,
                    SiteId = inputData.SiteId,
                    BannerIcon = bannerPath,
                    IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
                    BackGroundColor = inputData.BackGroundColor,
                    LoginWindowColor = inputData.LoginWindowColor,
                    LoginPageTitle = inputData.LoginPageTitle,
                    RegistrationPageTitle = inputData.RegistrationPageTitle
                };
                db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}