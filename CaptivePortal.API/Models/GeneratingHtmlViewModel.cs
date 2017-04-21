using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class GeneratingHtmlViewModel
    {
        public int SiteId { get; set; }
        public string RegisterUrl { get; set; }
        public string LoginUrl { get; set; }
    }
    public class FormData
    {
        public int SiteId { get; set; }
    }

    public class ReturnLoginFormData
    {
        public int SiteId { get; set; }
        public string BannerIcon { get; set; }
        public string BackGroundColor { get; set; }
        public string LoginWindowColor { get; set; }
        public bool IsPasswordRequire { get; set; }
        public string LoginPageTitle { get; set; }
        public string HtmlCodeForRegister { get; set; }
    }
}