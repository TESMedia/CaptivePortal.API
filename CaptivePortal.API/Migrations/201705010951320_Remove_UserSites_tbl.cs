namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_UserSites_tbl : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserSites", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.UserSites", "UserId", "dbo.Users");
            DropIndex("dbo.UserSites", new[] { "UserId" });
            DropIndex("dbo.UserSites", new[] { "SiteId" });
            AddColumn("dbo.Users", "SiteId", c => c.Int(nullable: false));
            DropTable("dbo.UserSites");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserSites",
                c => new
                    {
                        UserSiteId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SiteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserSiteId);
            
            DropColumn("dbo.Users", "SiteId");
            CreateIndex("dbo.UserSites", "SiteId");
            CreateIndex("dbo.UserSites", "UserId");
            AddForeignKey("dbo.UserSites", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.UserSites", "SiteId", "dbo.Sites", "SiteId", cascadeDelete: true);
        }
    }
}
