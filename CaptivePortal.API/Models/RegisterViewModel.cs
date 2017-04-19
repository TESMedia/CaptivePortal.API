using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class RegisterViewModel
    {
        public string HostName { get; set; }

        public string HostPassword { get; set; }

        public string UserName { get; set; }
       
        public string UserPassword { get; set; }

        public string CreationBy { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
      
        public string Email { get; set; }

        public DateTime CreationDate { get; set; }
        
        public DateTime UpdateDate { get; set; }

        public string SiteName { get; set; }

        public string Addresses { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Zip { get; set; }

        public string Notes { get; set; }

        public string Gender { get; set; }

        public string Age { get; set; }

        public string Term_conditions { get; set; }

        public bool promotional_email { get; set; }

        
    }
}