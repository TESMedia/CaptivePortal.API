namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Users_tbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IntStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "MacAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "MacAddress");
            DropColumn("dbo.Users", "IntStatus");
        }
    }
}
