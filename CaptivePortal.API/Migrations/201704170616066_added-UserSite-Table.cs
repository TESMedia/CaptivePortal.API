namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUserSiteTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSites",
                c => new
                    {
                        UserSiteId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SiteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserSiteId)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSites", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserSites", "SiteId", "dbo.Sites");
            DropIndex("dbo.UserSites", new[] { "SiteId" });
            DropIndex("dbo.UserSites", new[] { "UserId" });
            DropTable("dbo.UserSites");
        }
    }
}
