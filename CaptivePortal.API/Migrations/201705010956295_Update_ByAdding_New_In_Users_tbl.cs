namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_ByAdding_New_In_Users_tbl : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Users", "SiteId");
            AddForeignKey("dbo.Users", "SiteId", "dbo.Sites", "SiteId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "SiteId", "dbo.Sites");
            DropIndex("dbo.Users", new[] { "SiteId" });
        }
    }
}
