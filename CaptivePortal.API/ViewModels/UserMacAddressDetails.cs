using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CaptivePortal.API.Models;

namespace CaptivePortal.API.ViewModels
{
    public class UserMacAddressDetails
    {
        public ApplicationUser objUser { get; set; }
        public MacAddress objMacAddress { get; set; }
        public UsersAddress objAddress { get; set; }
    }
}