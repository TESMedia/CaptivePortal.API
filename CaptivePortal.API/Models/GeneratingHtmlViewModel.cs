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
        public int CompanyId { get; set; }
    }

    public class ReturnLoginFormData
    {
        public int SiteId { get; set; }
        public string BannerIcon { get; set; }
        public string BackGroundColor { get; set; }
        public string LoginWindowColor { get; set; }
        public bool IsPasswordRequire { get; set; }
        public string LoginPageTitle { get; set; }
        public string ControllerIP { get; set; }

        //public string HtmlCodeForRegister { get; set; }
    }

    public class ReturnRegisterFormData
    {
        // public int SiteId { get; set; }
        public string ColumnName { get; set; }
        public string LabelNameToDisplay { get; set; }
        public bool IsMandetory { get; set; }
        //public bool IsPasswordRequired { get; set; }
    }

    public class ReturnRegisterFormListData
    {

        public ReturnRegisterFormListData()
        {
            ReteurnRegisterFormList = new List<ReturnRegisterFormData>();
        }
        public List<ReturnRegisterFormData> ReteurnRegisterFormList { get; set; }
    }


    public class ReturnPromationalData
    {
        public string SuccessPageOption { get; set; }
        public string WebPageURL { get; set; }
        public string OptionalPictureForSuccessPage { get; set; }
    }

}