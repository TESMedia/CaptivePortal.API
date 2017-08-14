using CaptivePortal.API.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Context
{

    public class DbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, int,
        ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public DbContext()
            : base("DbContext")
        {
        }

        //public DbSet<WifiUser> WifiUsers { get; set; }
        public DbSet<UsersAddress> UsersAddress { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Nas> Nas { get; set; }
        public DbSet<Organisation> Organisation { get; set; }
        public DbSet<Site> Site { get; set; }
        public DbSet<RadGroupCheck> RadGroupCheck { get; set; }
        public DbSet<Radacct> Radacct { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<FormControl> FormControl { get; set; }
        public DbSet<Age> Age { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<MacAddress> MacAddress { get; set; }
        public DbSet<ApiAccessUserSession> UserSession { get; set; }
        public DbSet<AdminSiteAccess> AdminSiteAccess { get; set; }
        public DbSet<ManagePromotion> ManagePromotion { get; set; }

        //static DbContext()
        //{
        //    Database.SetInitializer<DbContext>(new ApplicationDbInitializer());
        //}

        public static DbContext Create()
        {
            return new DbContext();
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "dbo");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles", "dbo");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles", "dbo");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims", "dbo");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins", "dbo");

            //modelBuilder.Entity<Users>().ToTable("Users").Property(p => p.Id).HasColumnName("UserId");
            //modelBuilder.Entity<Users>().ToTable("Users").Property(p => p.PhoneNumber).HasColumnName("DefaultSite");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles").HasKey(m => m.UserId).HasKey(m => m.RoleId).Property(p => p.UserId).HasColumnName("UserId");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins").HasKey(m => m.UserId).HasKey(m => m.ProviderKey).HasKey(m => m.LoginProvider);
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims").Property(p => p.Id).HasColumnName("UserClaimId");
            //modelBuilder.Entity<IdentityRole>().ToTable("Roles").Property(p => p.Id).HasColumnName("RoleId");
        }
    }
}
