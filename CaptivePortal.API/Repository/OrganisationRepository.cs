using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Repository
{
    public class OrganisationRepository
    {
        private int RetCode { get; set; }
        private string RetMsg { get; set; }
        private DbContext db = null;

        public OrganisationRepository()
        {
            db = new DbContext();
        }

        public bool CreateOrganisation(FormViewModel inputData)
        {
            try
            {
                int orgId = inputData.OrganisationDdl;

                if (inputData.OrganisationName != null)
                {
                    Organisation objOrganisation = new Organisation
                    {
                        OrganisationName = inputData.OrganisationName
                    };
                    db.Organisation.Add(objOrganisation);
                    db.SaveChanges();
                    orgId = Convert.ToInt32(objOrganisation.OrganisationId);
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