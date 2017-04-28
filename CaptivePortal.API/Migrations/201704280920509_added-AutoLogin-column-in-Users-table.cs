namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAutoLogincolumninUserstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AutoLogin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AutoLogin");
        }
    }
}
