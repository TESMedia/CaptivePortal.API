using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Repository
{
    public class SiteRepository
    {
        private int RetCode { get; set; }
        private string RetMsg { get; set; }
        private DbContext db = null;
        string compId = null;
        string TandD = null;


        public SiteRepository()
        {
            db = new DbContext();
        }

        public bool CreateSite(FormViewModel inputData)
        {
            try
            {
                if (inputData.CompanyName == null)
                {
                    compId = inputData.CompanyDdl;
                }
                else
                {
                    compId = db.Company.FirstOrDefault(m => m.CompanyName == inputData.CompanyName).CompanyId.ToString();
                }

                Site objSite = new Site
                {
                    SiteName = inputData.SiteName,
                    CompanyId = compId == null ? null : (int?)Convert.ToInt32(compId),
                    AutoLogin = inputData.AutoLogin,
                    ControllerIpAddress = inputData.ControllerIpAddress,
                    MySqlIpAddress = inputData.MySqlIpAddress,
                    Term_conditions = inputData.Term_conditions,
                    TermsAndCondDoc = inputData.TermsAndCondDoc,
                    DashboardUrl = inputData.DashboardUrl,
                    RtlsUrl = inputData.RtlsUrl

                };
                db.Site.Add(objSite);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public bool UpdateSite(Site inputData)
        {
            try
            {
                Site objSite = new Site
                {
                    SiteName = inputData.SiteName,
                    SiteId = inputData.SiteId,
                    CompanyId = compId == null ? null : (int?)Convert.ToInt32(compId),
                    AutoLogin = inputData.AutoLogin,
                    ControllerIpAddress = inputData.ControllerIpAddress,
                    MySqlIpAddress = inputData.MySqlIpAddress,
                    Term_conditions = inputData.Term_conditions,
                    TermsAndCondDoc = TandD,
                    DashboardUrl = inputData.DashboardUrl,
                    RtlsUrl = inputData.RtlsUrl
                };

                db.Entry(objSite).State = System.Data.Entity.EntityState.Modified;
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