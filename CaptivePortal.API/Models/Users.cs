using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public bool AutoLogin { get; set; }
        public string Term_conditions {get;set;}
        public bool promotional_email { get; set; }
        public int IntStatus { get; set; }
        public string MacAddress { get; set; }
        public int ? SiteId { get; set; }

        [ForeignKey("SiteId")]
        public Site Site { get; set; }

    }

    public enum DeviceStatus
    {
      Athenticate=0,
      Authorize=1,
      Active=2,
      InActive=3,
      Lock=4
    }
}