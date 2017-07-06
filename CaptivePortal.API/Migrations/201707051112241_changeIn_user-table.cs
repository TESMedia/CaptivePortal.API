namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeIn_usertable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "AutoLogin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AutoLogin", c => c.Boolean());
        }
    }
}
