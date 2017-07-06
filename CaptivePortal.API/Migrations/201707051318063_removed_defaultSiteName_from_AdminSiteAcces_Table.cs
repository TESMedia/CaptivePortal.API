namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_defaultSiteName_from_AdminSiteAcces_Table : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AdminSiteAccesses", "DefaultSiteName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdminSiteAccesses", "DefaultSiteName", c => c.String());
        }
    }
}
