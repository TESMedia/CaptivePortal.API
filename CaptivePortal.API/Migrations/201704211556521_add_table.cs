namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Term_conditions", c => c.String());
            AddColumn("dbo.Users", "promotional_email", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "promotional_email");
            DropColumn("dbo.Users", "Term_conditions");
        }
    }
}
