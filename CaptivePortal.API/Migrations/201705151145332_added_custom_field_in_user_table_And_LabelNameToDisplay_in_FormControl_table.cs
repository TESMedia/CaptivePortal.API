namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_custom_field_in_user_table_And_LabelNameToDisplay_in_FormControl_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormControls", "LabelNameToDisplay", c => c.String());
            AddColumn("dbo.Users", "Custom1", c => c.String());
            AddColumn("dbo.Users", "Custom2", c => c.String());
            AddColumn("dbo.Users", "Custom3", c => c.String());
            AddColumn("dbo.Users", "Custom4", c => c.String());
            AddColumn("dbo.Users", "Custom5", c => c.String());
            AddColumn("dbo.Users", "Custom6", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Custom6");
            DropColumn("dbo.Users", "Custom5");
            DropColumn("dbo.Users", "Custom4");
            DropColumn("dbo.Users", "Custom3");
            DropColumn("dbo.Users", "Custom2");
            DropColumn("dbo.Users", "Custom1");
            DropColumn("dbo.FormControls", "LabelNameToDisplay");
        }
    }
}
