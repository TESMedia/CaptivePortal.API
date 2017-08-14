using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class ApplicationUserStore :
     UserStore<ApplicationUser, ApplicationRole, int,
     ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, int>, IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(System.Data.Entity.DbContext context)
            : base(context)
        {
        }

        public override Task CreateAsync(ApplicationUser user)
        {
            return base.CreateAsync(user);
        }
    }
}