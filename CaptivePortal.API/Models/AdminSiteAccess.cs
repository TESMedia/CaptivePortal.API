﻿using System;
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
        public string UserId { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public string DefaultSiteName { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; }
    }
}