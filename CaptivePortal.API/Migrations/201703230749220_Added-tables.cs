namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedtables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
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
                "dbo.Nas",
                c => new
                    {
                        NasId = c.Int(nullable: false, identity: false),
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
                "dbo.RadGroupChecks",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: false),
                        GroupName = c.String(),
                        Attribute = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        SiteId = c.Int(nullable: false, identity: true),
                        SiteName = c.String(),
                        UserId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SiteId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        CreationBy = c.String(),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),

                        CompanyId = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
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
            DropForeignKey("dbo.Sites", "UserId", "dbo.Users");
            DropForeignKey("dbo.Sites", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.UsersAddresses", new[] { "UserId" });
            DropIndex("dbo.Sites", new[] { "CompanyId" });
            DropIndex("dbo.Sites", new[] { "UserId" });
            DropIndex("dbo.Companies", new[] { "OrganisationId" });
            DropTable("dbo.UsersAddresses");
            DropTable("dbo.Users");
            DropTable("dbo.Sites");
            DropTable("dbo.RadGroupChecks");
            DropTable("dbo.Nas");
            DropTable("dbo.Organisations");
            DropTable("dbo.Companies");
        }
    }
}
