namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBannerIconinFormTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Forms", "BannerIcon", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Forms", "BannerIcon");
        }
    }
}
