namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_PasswordColumnInUsertable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Password", c => c.String());
           
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "UserPassword", c => c.String());
            DropColumn("dbo.Users", "Password");
        }
    }
}
