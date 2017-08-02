using CaptivePortal.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CaptivePortal.API.ViewModels
{
    public class AddMacModel
    {
        public string UserId { get; set; }
        public int SiteId { get; set; }
        public string SessionId { get; set; }
        public OperationType OperationType { get; set; }
        public List<MacAddesses> MacAddressList { get; set; }
    }
}