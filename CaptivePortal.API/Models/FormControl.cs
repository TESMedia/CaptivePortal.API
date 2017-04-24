using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class FormControl
    {
        [Key]
        public int FormControlId { get; set; }
        //Foreign key
        public int FormId { get; set; }
        public string ControlType { get; set; }
        public string LabelName { get; set; }
        public string SiteUrl { get; set; }
        public string HtmlString { get; set; }

        [ForeignKey("FormId")]
        public virtual Form Forms { get; set; }
    }
}