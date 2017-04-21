using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class FormViewModel
    {
        public FormViewModel()
        {
            FormControls = new List<FormControl>();
        }
        public int SiteId { get; set; }
        public int FormId { get; set; }
        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string OrganisationName { get; set; }
        public int organisationDdl { get; set; }
        public int CompanyDdl { get; set; }
        public string BannerIcon { get; set; }
        public string BackGroundColor { get; set; }
        public string LoginWindowColor { get; set; }
        public bool IsPasswordRequire { get; set; }
        public string LoginPageTitle { get; set; }
        public string RegistrationPageTitle { get; set; }

        public string [] dataType { get; }
        public string [] controlType { get;}
        public string [] fieldlabel { get; }

        public List<FormControl> FormControls { get; set; }
    }

  


}