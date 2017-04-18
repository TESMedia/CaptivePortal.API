namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedColumnHtmlCodeForLogininFormTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Forms", "HtmlCodeForLogin", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Forms", "HtmlCodeForLogin");
        }
    }
}
