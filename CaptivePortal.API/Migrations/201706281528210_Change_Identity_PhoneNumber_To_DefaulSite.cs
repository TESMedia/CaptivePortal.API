namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Identity_PhoneNumber_To_DefaulSite : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "PhoneNumber", newName: "DefaultSite");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Users", name: "DefaultSite", newName: "PhoneNumber");
        }
    }
}
