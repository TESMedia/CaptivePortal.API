namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedNullabletoSiteIdofUserTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "SiteId", "dbo.Sites");
            DropIndex("dbo.Users", new[] { "SiteId" });
            AlterColumn("dbo.Users", "SiteId", c => c.Int());
            CreateIndex("dbo.Users", "SiteId");
            AddForeignKey("dbo.Users", "SiteId", "dbo.Sites", "SiteId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "SiteId", "dbo.Sites");
            DropIndex("dbo.Users", new[] { "SiteId" });
            AlterColumn("dbo.Users", "SiteId", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "SiteId");
            AddForeignKey("dbo.Users", "SiteId", "dbo.Sites", "SiteId", cascadeDelete: true);
        }
    }
}
