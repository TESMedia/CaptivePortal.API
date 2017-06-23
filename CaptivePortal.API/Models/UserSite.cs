using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class UserSite
    {
        public int UserSiteId { get; set; }

        //Foreign key
        public String UserId { get; set; }
        public int SiteId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }

        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
    }
}