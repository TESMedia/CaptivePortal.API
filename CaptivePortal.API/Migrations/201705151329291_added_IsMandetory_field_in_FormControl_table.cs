namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_IsMandetory_field_in_FormControl_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormControls", "IsMandetory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormControls", "IsMandetory");
        }
    }
}
