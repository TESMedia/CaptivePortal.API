using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptivePortal.API.Models
{
    public class OperatingSystem
    {
        [Key()]
        public int OSId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
    }
}