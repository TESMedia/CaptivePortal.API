namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Radaccttable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Radaccts", "AcctstopTime", c => c.String());
            AlterColumn("dbo.Radaccts", "AcctstartTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Radaccts", "AcctstartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Radaccts", "AcctstopTime", c => c.DateTime(nullable: false));
        }
    }
}
