namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletecolumfromformtable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Forms", "HtmlCodeForLogin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Forms", "HtmlCodeForLogin", c => c.String());
        }
    }
}
