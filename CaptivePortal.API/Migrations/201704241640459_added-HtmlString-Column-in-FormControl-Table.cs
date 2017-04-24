namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedHtmlStringColumninFormControlTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormControls", "HtmlString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormControls", "HtmlString");
        }
    }
}
