namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Sites",
                c => new
                    {
                        SiteId = c.Int(nullable: false, identity: true),
                        SiteName = c.String(),
                        CompanyId = c.Int(),
                        AutoLogin = c.Boolean(nullable: false),
                        ControllerIpAddress = c.String(),
                        MySqlIpAddress = c.String(),
                    })
                .PrimaryKey(t => t.SiteId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.FormControls",
                c => new
                    {
                        FormControlId = c.Int(nullable: false, identity: true),
                        FormId = c.Int(nullable: false),
                        ControlType = c.String(),
                        LabelName = c.String(),
                        SiteUrl = c.String(),
                        HtmlString = c.String(),
                    })
                .PrimaryKey(t => t.FormControlId)
                .ForeignKey("dbo.Forms", t => t.FormId, cascadeDelete: true)
                .Index(t => t.FormId);
            
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
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserRoleId)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        CreationBy = c.String(),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        Gender = c.String(),
                        Age = c.String(),
                        AutoLogin = c.Boolean(nullable: false),
                        Term_conditions = c.String(),
                        promotional_email = c.Boolean(nullable: false),
                        IntStatus = c.Int(nullable: false),
                        MacAddress = c.String(),
                        SiteId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Sites", t => t.SiteId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.UsersAddresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Addresses = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        Zip = c.String(),
                        Notes = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.FormControls", "FormId", "dbo.Forms");
            DropForeignKey("dbo.Forms", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.Sites", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.UsersAddresses", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "SiteId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.FormControls", new[] { "FormId" });
            DropIndex("dbo.Sites", new[] { "CompanyId" });
            DropIndex("dbo.Forms", new[] { "SiteId" });
            DropIndex("dbo.Companies", new[] { "OrganisationId" });
            DropTable("dbo.UsersAddresses");
            DropTable("dbo.Users");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.RadGroupChecks");
            DropTable("dbo.Radaccts");
            DropTable("dbo.Nas");
            DropTable("dbo.FormControls");
            DropTable("dbo.Sites");
            DropTable("dbo.Forms");
            DropTable("dbo.Organisations");
            DropTable("dbo.Companies");
        }
    }
}
