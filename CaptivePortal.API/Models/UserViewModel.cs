using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreationDate { get; set; }
        public string MacAddress { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string AgeRange { get; set; }
        public string PromotionEmailOptIn { get; set; }
        public string ThirdPartyOptIn  { get; set; }
        public string Term_conditions { get; set; }
        public string Password { get; set; }
    }


    public class UserlistViewModel
    {
        public UserlistViewModel()
        {
            UserViewlist = new List<UserViewModel>();
        }
        public List<UserViewModel> UserViewlist { get; set; }
        public UserViewModel UserView { get; set; }
    }
}