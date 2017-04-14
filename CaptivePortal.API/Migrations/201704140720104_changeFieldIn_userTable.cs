namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeFieldIn_userTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Gender", c => c.String());
            AddColumn("dbo.Users", "Age", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Age");
            DropColumn("dbo.Users", "Gender");
        }
    }
}
