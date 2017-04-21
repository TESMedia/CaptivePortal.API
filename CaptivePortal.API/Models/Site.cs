using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class Site
    {
        [Key]
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        // Foreign key 
        //public int UserId { get; set; }
        public  int ? CompanyId { get; set; }

        //[ForeignKey("UserId")]
        //public virtual Users Users { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

    }
}