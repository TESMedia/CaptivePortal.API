namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_adminSiteAccess_teble : DbMigration
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
                        DefaultSiteName = c.String(),
                    })
                .PrimaryKey(t => t.AdminSiteAccessId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdminSiteAccesses", "UserId", "dbo.Users");
            DropIndex("dbo.AdminSiteAccesses", new[] { "UserId" });
            DropTable("dbo.AdminSiteAccesses");
        }
    }
}
