namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedinusertabletoallownull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "AutoLogin", c => c.Boolean());
            AlterColumn("dbo.Users", "promotional_email", c => c.Boolean());
            AlterColumn("dbo.Users", "IntStatus", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "IntStatus", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "promotional_email", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Users", "AutoLogin", c => c.Boolean(nullable: false));
        }
    }
}
