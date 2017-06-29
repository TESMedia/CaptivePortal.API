using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class Users : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Users> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [MaxLength(50)]

        public string FirstName { get; set; }



        [MaxLength(50)]

        public string LastName { get; set; }

        public DateTime CreationDate { get; set; }



        public DateTime UpdateDate { get; set; }



      //  [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]

        public DateTime ? BirthDate { get; set; }

       

        public int? MobileNumer { get; set; }



        public int? GenderId { get; set; }



        public int? AgeId { get; set; }



        public int? SiteId { get; set; }



        public bool? AutoLogin { get; set; }



        public bool? promotional_email { get; set; }



        public bool? ThirdPartyOptIn { get; set; }



        public bool? UserOfDataOptIn { get; set; }



        [MaxLength(50)]

        public string Status { get; set; }



        [MaxLength(50)]

        public string Custom1 { get; set; }



        [MaxLength(50)]

        public string Custom2 { get; set; }



        [MaxLength(50)]

        public string Custom3 { get; set; }



        [MaxLength(50)]

        public string Custom4 { get; set; }



        [MaxLength(50)]

        public string Custom5 { get; set; }



        [MaxLength(50)]

        public string Custom6 { get; set; }



        public string UniqueUserId { get; set; }



        [ForeignKey("AgeId")]

        public virtual Age Ages { get; set; }



        [ForeignKey("GenderId")]

        public virtual Gender Genders { get; set; }



        public virtual Site Sites { get; set; }
    }
}