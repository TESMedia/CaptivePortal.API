namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAutoLogincolumninSiteTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "AutoLogin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "AutoLogin");
        }
    }
}
