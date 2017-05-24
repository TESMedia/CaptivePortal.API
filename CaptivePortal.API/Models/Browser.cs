using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptivePortal.API.Models
{
    public class Browser
    {
        [Key()]
        public int BrowserId { get; set; }

        [MaxLength(50)]
        public string BrowserName { get; set; }

    }
}