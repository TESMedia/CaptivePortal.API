using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CaptivePortal.API.Models
{
    public class UsersDeviceData
    {
        [Key]
        public int UsersDeviceDataId { get; set; }

        [MaxLength(50)]
        public string MacAddress { get; set; }

        [MaxLength(50)]
        public string OperatingSystem { get; set; }

        public bool IsMobile { get; set; }

        [MaxLength(50)]
        public string Browser { get; set; }

        [MaxLength(50)]
        public string UserAgentName { get; set; }
    }
}