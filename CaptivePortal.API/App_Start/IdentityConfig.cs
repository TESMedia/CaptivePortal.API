using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using CaptivePortal.API.Models;
using CaptivePortal.API.Context;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Net.Mail;
using System.Configuration;
using System;
using System.Data.Entity;
using System.Web;


namespace CaptivePortal.API
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    // *** PASS IN TYPE ARGUMENT TO BASE CLASS:

    /// <summary>
    /// 
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            string senderID = "tls@tes.media";
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.avecsys.net", // smtp server address here…
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential("user@smtp.avecsys.net", "ema1ls3rv3r"),
                Timeout = 30000,
            };
            MailMessage mailMessage = new MailMessage(senderID, message.Destination, message.Subject, message.Body);
            mailMessage.IsBodyHtml = true;
            smtp.Send(mailMessage);
            return Task.FromResult(0);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        //CustomPasswordHasher objHasspassword;
        //// *** ADD INT TYPE ARGUMENT TO CONSTRUCTOR CALL:
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
            //objHasspassword = new CustomPasswordHasher();
        }

        //public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        //{
        //    user.PasswordHash = objHasspassword.HashPassword(password);
        //    return await base.CreateAsync(user);
        //}


        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            // *** PASS CUSTOM APPLICATION USER STORE AS CONSTRUCTOR ARGUMENT:
            var manager = new ApplicationUserManager(
                new ApplicationUserStore(context.Get<Context.DbContext>()));

           
            // Configure validation logic for usernames

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. 
            // This application uses Phone and Emails as a step of receiving a 
            // code for verifying the user You can write your own provider and plug in here.

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.RegisterTwoFactorProvider("PhoneCode",
                new PhoneNumberTokenProvider<ApplicationUser, int>
                {
                    MessageFormat = "Your security code is: {0}"
                });

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.RegisterTwoFactorProvider("EmailCode",
                new EmailTokenProvider<ApplicationUser, int>
                {
                    Subject = "SecurityCode",
                    BodyFormat = "Your security code is {0}"
                });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, int>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }


    // PASS CUSTOM APPLICATION ROLE AND INT AS TYPE ARGUMENTS TO BASE:
    public class ApplicationRoleManager : RoleManager<ApplicationRole, int>
    {
        // PASS CUSTOM APPLICATION ROLE AND INT AS TYPE ARGUMENTS TO CONSTRUCTOR:
        public ApplicationRoleManager(IRoleStore<ApplicationRole, int> roleStore)
            : base(roleStore)
        {
        }

        // PASS CUSTOM APPLICATION ROLE AS TYPE ARGUMENT:
        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<CaptivePortal.API.Context.DbContext>()));
        }
    }


    //// This is useful if you do not want to tear down the database each time you run the application.
    //// public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    //// This example shows you how to create a new database if the Model changes
    //public class ApplicationDbInitializer : CreateDatabaseIfNotExists<Context.DbContext>
    //{
    //    //protected override void Seed(Context.DbContext context)
    //    //{
    //    //    InitializeIdentityForEF(context);
    //    //    base.Seed(context);
    //    //}

    //    //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
    //    public static void InitializeIdentityForEF(Context.DbContext db)
    //    {
    //        var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
    //        var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
    //        const string name = "admin@airloc8.com";
    //        const string password = "Tes@123";
    //        const string roleName = "GlobalAdmin";

    //        var user = new ApplicationUser();
    //        user.UserName = "admin@airloc8.com";
    //        user.Email = "admin@airloc8.com";
    //        user.CreationDate = DateTime.Now;
    //        user.UpdateDate = DateTime.Now;
    //        user.EmailConfirmed = true;
    //        user.AccessFailedCount = 0;
    //        user.LockoutEnabled = false;
    //        user.TwoFactorEnabled = false;
    //        user.PhoneNumberConfirmed = true;


    //        //Create Role Admin if it does not exist
    //        var role = roleManager.FindByName(roleName);
    //        if (role == null)
    //        {
    //            role = new ApplicationRole(roleName);
    //            var roleresult = roleManager.Create(role);
    //        }

    //        var User = userManager.FindByName(name);
    //        if (User == null)
    //        {
    //            //user = new ApplicationUser { UserName = name, Email = name };
    //            var result = userManager.Create(user, password);
    //            result = userManager.SetLockoutEnabled(user.Id, false);
    //        }

    //        // Add user admin to Role Admin if not already added
    //        var rolesForUser = userManager.GetRoles(user.Id);
    //        if (!rolesForUser.Contains(role.Name))
    //        {
    //            var result = userManager.AddToRole(user.Id, role.Name);
    //        }
    //    }
    //}


    public class ApplicationSignInManager : SignInManager<ApplicationUser, int>
    {
        //CustomPasswordHasher objHasspassword;
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        {
            //objHasspassword = new CustomPasswordHasher(); 
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}