using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public bool AutoLogin { get; set; }
        public string AgeRange { get; set; }
        public bool PromotionEmailOptIn { get; set; }
        public bool ThirdPartyOptIn { get; set; }
        public bool UserOfDataOptIn { get; set; }
        public string Term_conditions { get; set; }
        public string Password { get; set; }
        public Status Status { get; set; }

    }

    public enum Status
    {
        Active,
        Locked,
    }

    public class UserLabelModalViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public string AgeRange { get; set; }
        public string Mobilenumber { get; set; }

        public string Emailaddress { get; set; }
        public string Use_of_data_opt_in { get; set; }
        public string Promotion_opt_in { get; set; }
        public string Third_party_opt_in { get; set; }
        public string Address_line_1 { get; set; }
        public string Address_line_2 { get; set; }

        public string PostTown { get; set; }
        public string PostCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Birthdate { get; set; }


        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public string Custom6 { get; set; }

    }


    public class UserlistViewModel
    {
        public UserlistViewModel()
        {
            UserViewlist = new List<UserViewModel>();
            UserLabelModalViewList = new SelectList(new List<UserLabelModalViewModel>());
        }
        public List<UserViewModel> UserViewlist { get; set; }
        public UserViewModel UserView { get; set; }
        public SelectList UserLabelModalViewList { get; set; }
    }

    public enum dataControlType
    {
        Text = 1,
        Dropdown = 2,
        Checkbox = 3
    }
}