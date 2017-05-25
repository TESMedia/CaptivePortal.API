namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New_changes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Roles", "Session");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "Session", c => c.String());
        }
    }
}
