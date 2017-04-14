namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcolumnSiteUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormControls", "SiteUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormControls", "SiteUrl");
        }
    }
}
