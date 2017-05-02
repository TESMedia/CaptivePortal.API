namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Site_tbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "ControllerIpAddress", c => c.String());
            AddColumn("dbo.Sites", "MySqlIpAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "MySqlIpAddress");
            DropColumn("dbo.Sites", "ControllerIpAddress");
        }
    }
}
