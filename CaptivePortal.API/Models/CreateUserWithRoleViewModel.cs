using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class CreateUserWithRoleViewModel
    {
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int SiteDdl { get; set; }

        public CreateUserWithRoleViewModel()
        {
            RestrictedSites = new List<RestrictedSite>();
        }
        public List<RestrictedSite> RestrictedSites { get; set; }
        public List<SiteViewModel> SiteViewlist { get; set; }
    }

    public class RestrictedSite
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public bool IsSelected { get; set; }
    }

}