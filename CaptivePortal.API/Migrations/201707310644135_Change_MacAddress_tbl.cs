namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_MacAddress_tbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MacAddresses", "IsRegisterInRtls", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MacAddresses", "IsRegisterInRtls");
        }
    }
}
