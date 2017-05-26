namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedpendingmigrationfiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "RtlsUrl", c => c.String());
            AddColumn("dbo.Sites", "DashboardUrl", c => c.String());
            DropColumn("dbo.Sites", "FileName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "FileName", c => c.String());
            DropColumn("dbo.Sites", "DashboardUrl");
            DropColumn("dbo.Sites", "RtlsUrl");
        }
    }
}
