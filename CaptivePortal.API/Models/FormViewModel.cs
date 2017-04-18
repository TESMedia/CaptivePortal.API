using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class FormViewModel
    {
        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string OrganisationName { get; set; }
        public string BannerIcon { get; set; }
        public string BackGroundColor { get; set; }
        public string LoginWindowColor { get; set; }
        public bool IsPasswordRequire { get; set; }
        public string LoginPageTitle { get; set; }
        public string RegistrationPageTitle { get; set; }
    }
}