using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptivePortal.API.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }
   
        public string FormName { get; set; }
        //Foreign key
        public int SiteId { get; set; }

        public string BannerIcon { get; set; }
        public string BackGroundColor { get; set; }
        public string LoginWindowColor { get; set; }
        public bool IsPasswordRequire { get; set; }
        public string LoginPageTitle { get; set; }
        public string RegistrationPageTitle { get; set; }
       
       

        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
    }
}