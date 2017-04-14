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

        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
    }
}