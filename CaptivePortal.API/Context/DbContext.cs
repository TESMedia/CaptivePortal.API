using CaptivePortal.API.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Context
{
    public class DbContext: IdentityDbContext<Users>
    {
        public DbContext() : base("name=DbContext")
        {
             
        }
        //public DbSet<Users> User { get; set; }
        public DbSet<UsersAddress> UsersAddress { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Nas> Nas { get; set; }
        public DbSet<Organisation> Organisation { get; set; }
        public DbSet<Site> Site { get; set; }
        public DbSet<RadGroupCheck> RadGroupCheck { get; set; }
        public DbSet<Radacct> Radacct { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet <Form> Form { get; set; }
        public DbSet <FormControl> FormControl { get; set; }
        public DbSet<Age> Age { get; set; }
        public DbSet<Gender> Gender { get; set; }

        //public DbSet<UsersDeviceData> UsersDeviceDatas { get; set; }
        public DbSet<MacAddress> MacAddress { get; set; }

        public DbSet<ApiAccessUserSession> UserSession { get; set; }
        public DbSet<AdminSiteAccess> AdminSiteAccess { get; set; }


        //public DbSet<UserSite> UserSite { get; set; }
        public static DbContext Create()
        {
            return new DbContext();
        }

    }
}