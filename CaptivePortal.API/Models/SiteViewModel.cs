using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class SiteViewModel
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string OrgName { get; set; }

        public string CmpName { get; set; }
    }
}