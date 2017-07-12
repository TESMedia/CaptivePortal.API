using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class AdminViewModel
    {
        public string OrganisationName { get; set; }

        public string CompanyName { get; set; }
        public string SiteName { get; set; }
        public string RtlsUrl { get; set; }
        public string DashboardUrl { get; set; }
        public int SiteId { get; set; }
        public string MySqlIpAddress { get; set; }
        public string DefaultSite { get; set; }

    }
    public class AdminlistViewModel
    {
        public AdminlistViewModel()
        {
            AdminViewlist = new List<AdminViewModel>();
          
        }
        public List<AdminViewModel> AdminViewlist { get; set; }

    }
}