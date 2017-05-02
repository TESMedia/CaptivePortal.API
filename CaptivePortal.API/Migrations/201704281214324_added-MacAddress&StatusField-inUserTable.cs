namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedMacAddressStatusFieldinUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "MacAddress", c => c.String());
            AddColumn("dbo.Users", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Status");
            DropColumn("dbo.Users", "MacAddress");
        }
    }
}
