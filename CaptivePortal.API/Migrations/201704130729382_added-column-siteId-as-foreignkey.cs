namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcolumnsiteIdasforeignkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Forms", "SiteId", c => c.Int(nullable: false));
            CreateIndex("dbo.Forms", "SiteId");
            AddForeignKey("dbo.Forms", "SiteId", "dbo.Sites", "SiteId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Forms", "SiteId", "dbo.Sites");
            DropIndex("dbo.Forms", new[] { "SiteId" });
            DropColumn("dbo.Forms", "SiteId");
        }
    }
}
