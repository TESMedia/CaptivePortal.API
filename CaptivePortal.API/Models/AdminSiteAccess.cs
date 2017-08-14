using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class AdminSiteAccess
    {
        [Key]
        public int AdminSiteAccessId { get; set; }
        //foreign key
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}