using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CaptivePortal.API.Models
{
    public class Age
    {
        [Key]
        public int AgeId { get; set; }

        [MaxLength(50)]
        public string  Value { get; set; }

        //public virtual Users Users { get; set; }
    }
}