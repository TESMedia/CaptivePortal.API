using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptivePortal.API.Models
{
    public class MacAddress
    {
        [Key()]
        public int MacId { get; set; }

        [MaxLength(20)]
        //[Index("Index_MacUnique", 1, IsUnique = true)]
        public string MacAddressValue { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }

        public string BrowserName { get; set; }

        public string OperatingSystem { get; set; }

        public bool IsMobile { get; set; }

        public string UserAgentName { get; set; }

    }
}