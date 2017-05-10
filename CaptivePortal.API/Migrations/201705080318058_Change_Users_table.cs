namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Users_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "OperatingSystem", c => c.String());
            AddColumn("dbo.Users", "IsMobile", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "Browser", c => c.String());
            AddColumn("dbo.Users", "UserAgentName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UserAgentName");
            DropColumn("dbo.Users", "Browser");
            DropColumn("dbo.Users", "IsMobile");
            DropColumn("dbo.Users", "OperatingSystem");
        }
    }
}
