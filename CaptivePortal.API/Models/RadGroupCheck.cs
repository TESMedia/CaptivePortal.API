using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class RadGroupCheck
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Attribute { get; set; }
        public char op { get; set; }
        public string Value { get; set; }
    }
}