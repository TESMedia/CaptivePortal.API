namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeIn_identitytable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "Custom1");
            DropColumn("dbo.Users", "Custom2");
            DropColumn("dbo.Users", "Custom3");
            DropColumn("dbo.Users", "Custom4");
            DropColumn("dbo.Users", "Custom5");
            DropColumn("dbo.Users", "Custom6");
            DropColumn("dbo.Users", "UniqueUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "UniqueUserId", c => c.String());
            AddColumn("dbo.Users", "Custom6", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom5", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom4", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom3", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom2", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom1", c => c.String(maxLength: 50));
        }
    }
}
