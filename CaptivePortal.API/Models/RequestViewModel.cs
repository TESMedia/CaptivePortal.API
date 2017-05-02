using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class RequestViewModel
    {
        public int SiteId { get; set; }

        public string MacAddress { get; set; }
    }
}