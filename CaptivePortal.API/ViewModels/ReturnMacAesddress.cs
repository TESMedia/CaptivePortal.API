using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.ViewModels
{
    public class ReturnMacAesddress
    {
        public ReturnMacAesddress()
        {
            MacAddressList = new List<MacAddesses>();
        }

        public List<MacAddesses> MacAddressList { get; set; }
        public int returncode { get; set; }
        public string type { get; set; }
        public string msg { get; set; }
    }
}