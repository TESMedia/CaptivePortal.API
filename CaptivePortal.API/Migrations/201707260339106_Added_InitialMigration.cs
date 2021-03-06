namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminSiteAccesses",
                c => new
                    {
                        AdminSiteAccessId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SiteId = c.Int(nullable: false),
                        SiteName = c.String(),
                    })
                .PrimaryKey(t => t.AdminSiteAccessId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(maxLength: 50),
                        CreationDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        BirthDate = c.DateTime(),
                        MobileNumer = c.Int(),
                        GenderId = c.Int(),
                        AgeId = c.Int(),
                        SiteId = c.Int(),
                        promotional_email = c.Boolean(),
                        ThirdPartyOptIn = c.Boolean(),
                        UserOfDataOptIn = c.Boolean(),
                        Status = c.String(maxLength: 50),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ages", t => t.AgeId)
                .ForeignKey("dbo.Genders", t => t.GenderId)
                .ForeignKey("dbo.Sites", t => t.SiteId)
                .Index(t => t.GenderId)
                .Index(t => t.AgeId)
                .Index(t => t.SiteId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Ages",
                c => new
                    {
                        AgeId = c.Int(nullable: false, identity: true),
                        Value = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.AgeId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Genders",
                c => new
                    {
                        GenderId = c.Int(nullable: false, identity: true),
                        Value = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.GenderId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        SiteId = c.Int(nullable: false, identity: true),
                        SiteName = c.String(),
                        CompanyId = c.Int(),
                        AutoLogin = c.Boolean(),
                        ControllerIpAddress = c.String(),
                        MySqlIpAddress = c.String(),
                        Term_conditions = c.String(),
                        TermsAndCondDoc = c.String(),
                        RtlsUrl = c.String(),
                        DashboardUrl = c.String(),
                    })
                .PrimaryKey(t => t.SiteId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        OrganisationId = c.Int(),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        OrganisationName = c.String(),
                    })
                .PrimaryKey(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Forms",
                c => new
                    {
                        FormId = c.Int(nullable: false, identity: true),
                        FormName = c.String(),
                        SiteId = c.Int(nullable: false),
                        BannerIcon = c.String(),
                        BackGroundColor = c.String(),
                        LoginWindowColor = c.String(),
                        IsPasswordRequire = c.Boolean(nullable: false),
                        LoginPageTitle = c.String(),
                        RegistrationPageTitle = c.String(),
                    })
                .PrimaryKey(t => t.FormId)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.FormControls",
                c => new
                    {
                        FormControlId = c.Int(nullable: false, identity: true),
                        FormId = c.Int(nullable: false),
                        ControlType = c.String(),
                        LabelName = c.String(),
                        LabelNameToDisplay = c.String(),
                        IsMandetory = c.Boolean(nullable: false),
                        SiteUrl = c.String(),
                        HtmlString = c.String(),
                    })
                .PrimaryKey(t => t.FormControlId)
                .ForeignKey("dbo.Forms", t => t.FormId, cascadeDelete: true)
                .Index(t => t.FormId);
            
            CreateTable(
                "dbo.MacAddresses",
                c => new
                    {
                        MacId = c.Int(nullable: false, identity: true),
                        MacAddressValue = c.String(maxLength: 20),
                        UserId = c.Int(nullable: false),
                        BrowserName = c.String(),
                        OperatingSystem = c.String(),
                        IsMobile = c.Boolean(nullable: false),
                        UserAgentName = c.String(),
                    })
                .PrimaryKey(t => t.MacId)
                .ForeignKey("dbo.WifiUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.WifiUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        UserName = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        CreationDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        BirthDate = c.DateTime(),
                        MobileNumer = c.Int(),
                        GenderId = c.Int(),
                        AgeId = c.Int(),
                        SiteId = c.Int(),
                        AutoLogin = c.Boolean(),
                        promotional_email = c.Boolean(),
                        ThirdPartyOptIn = c.Boolean(),
                        UserOfDataOptIn = c.Boolean(),
                        Status = c.String(maxLength: 50),
                        Custom1 = c.String(maxLength: 50),
                        Custom2 = c.String(maxLength: 50),
                        Custom3 = c.String(maxLength: 50),
                        Custom4 = c.String(maxLength: 50),
                        Custom5 = c.String(maxLength: 50),
                        Custom6 = c.String(maxLength: 50),
                        UniqueUserId = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Ages", t => t.AgeId)
                .ForeignKey("dbo.Genders", t => t.GenderId)
                .ForeignKey("dbo.Sites", t => t.SiteId)
                .Index(t => t.GenderId)
                .Index(t => t.AgeId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.ManagePromotions",
                c => new
                    {
                        ManagePromotionId = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        SuccessPageOption = c.String(),
                        WebPageURL = c.String(),
                        OptionalPictureForSuccessPage = c.String(),
                    })
                .PrimaryKey(t => t.ManagePromotionId)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.Nas",
                c => new
                    {
                        NasId = c.Int(nullable: false, identity: true),
                        Nasname = c.String(maxLength: 128, unicode: false),
                        Shortname = c.String(maxLength: 32, unicode: false),
                        Type = c.String(maxLength: 30, unicode: false),
                        Ports = c.Int(nullable: false),
                        Secret = c.String(maxLength: 60, unicode: false),
                        Server = c.String(maxLength: 64, unicode: false),
                        Community = c.String(maxLength: 50, unicode: false),
                        Description = c.String(maxLength: 500, unicode: false),
                    })
                .PrimaryKey(t => t.NasId);
            
            CreateTable(
                "dbo.Radaccts",
                c => new
                    {
                        RadacctId = c.Long(nullable: false, identity: true),
                        AcctsessionId = c.String(maxLength: 64, unicode: false),
                        AcctuniqueId = c.String(maxLength: 32, unicode: false),
                        UserName = c.String(maxLength: 64, unicode: false),
                        GroupName = c.String(maxLength: 64, unicode: false),
                        NasipAddress = c.String(maxLength: 15, unicode: false),
                        NasportId = c.String(maxLength: 15, unicode: false),
                        NasPortType = c.String(maxLength: 32, unicode: false),
                        AcctstopTime = c.String(),
                        AcctstartTime = c.String(),
                        AcctsessionTime = c.Int(nullable: false),
                        Acctauthentic = c.String(maxLength: 32, unicode: false),
                        Connectinfo_Start = c.String(maxLength: 50, unicode: false),
                        Connectinfo_Stop = c.String(maxLength: 32, unicode: false),
                        AcctinputOctets = c.Long(nullable: false),
                        AcctoutputOctets = c.Long(nullable: false),
                        CalledstationId = c.String(maxLength: 50, unicode: false),
                        CallingstationId = c.String(maxLength: 50, unicode: false),
                        AcctterminateCause = c.String(maxLength: 50, unicode: false),
                        ServiceType = c.String(maxLength: 50, unicode: false),
                        FramedProtocol = c.String(maxLength: 50, unicode: false),
                        FramedIpAddress = c.String(maxLength: 50, unicode: false),
                        AcctStartDelay = c.Int(nullable: false),
                        AcctStopDelay = c.Int(nullable: false),
                        XascendSessionsvrKey = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.RadacctId);
            
            CreateTable(
                "dbo.RadGroupChecks",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        Attribute = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UsersAddresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        PostTown = c.String(),
                        County = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PostCode = c.String(),
                        Notes = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.WifiUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ApiAccessUserSessions",
                c => new
                    {
                        UserSessionId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SessionId = c.String(),
                    })
                .PrimaryKey(t => t.UserSessionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApiAccessUserSessions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersAddresses", "UserId", "dbo.WifiUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ManagePromotions", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.MacAddresses", "UserId", "dbo.WifiUsers");
            DropForeignKey("dbo.WifiUsers", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.WifiUsers", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.WifiUsers", "AgeId", "dbo.Ages");
            DropForeignKey("dbo.FormControls", "FormId", "dbo.Forms");
            DropForeignKey("dbo.Forms", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.AdminSiteAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.Sites", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "AgeId", "dbo.Ages");
            DropIndex("dbo.ApiAccessUserSessions", new[] { "UserId" });
            DropIndex("dbo.UsersAddresses", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ManagePromotions", new[] { "SiteId" });
            DropIndex("dbo.WifiUsers", new[] { "SiteId" });
            DropIndex("dbo.WifiUsers", new[] { "AgeId" });
            DropIndex("dbo.WifiUsers", new[] { "GenderId" });
            DropIndex("dbo.MacAddresses", new[] { "UserId" });
            DropIndex("dbo.FormControls", new[] { "FormId" });
            DropIndex("dbo.Forms", new[] { "SiteId" });
            DropIndex("dbo.Companies", new[] { "OrganisationId" });
            DropIndex("dbo.Sites", new[] { "CompanyId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "SiteId" });
            DropIndex("dbo.AspNetUsers", new[] { "AgeId" });
            DropIndex("dbo.AspNetUsers", new[] { "GenderId" });
            DropIndex("dbo.AdminSiteAccesses", new[] { "UserId" });
            DropTable("dbo.ApiAccessUserSessions");
            DropTable("dbo.UsersAddresses");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RadGroupChecks");
            DropTable("dbo.Radaccts");
            DropTable("dbo.Nas");
            DropTable("dbo.ManagePromotions");
            DropTable("dbo.WifiUsers");
            DropTable("dbo.MacAddresses");
            DropTable("dbo.FormControls");
            DropTable("dbo.Forms");
            DropTable("dbo.Organisations");
            DropTable("dbo.Companies");
            DropTable("dbo.Sites");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Genders");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Ages");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AdminSiteAccesses");
        }
    }
}
