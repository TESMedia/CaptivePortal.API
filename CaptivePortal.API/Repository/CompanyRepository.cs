using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Repository
{
    public class CompanyRepository
    {
        private int RetCode { get; set; }
        private string RetMsg { get; set; }
        private DbContext db = null;

        public CompanyRepository()
        {
            db = new DbContext();
        }

        public bool CreateCompany(FormViewModel inputData)
        {
            int orgId = inputData.OrganisationDdl;
            string compId = inputData.CompanyDdl;

            try
            {
                if (inputData.CompanyName != null)
                {
                    Company objCompany = new Company
                    {
                        CompanyName = inputData.CompanyName,
                        OrganisationId = orgId == 0 ? null : (int?)Convert.ToInt32(orgId)
                    };
                    db.Company.Add(objCompany);
                    db.SaveChanges();
                    compId = objCompany.CompanyId.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}