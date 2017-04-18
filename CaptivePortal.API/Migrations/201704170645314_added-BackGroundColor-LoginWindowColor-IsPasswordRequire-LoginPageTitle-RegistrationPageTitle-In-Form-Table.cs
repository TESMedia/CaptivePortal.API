namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBackGroundColorLoginWindowColorIsPasswordRequireLoginPageTitleRegistrationPageTitleInFormTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Forms", "BackGroundColor", c => c.String());
            AddColumn("dbo.Forms", "LoginWindowColor", c => c.String());
            AddColumn("dbo.Forms", "IsPasswordRequire", c => c.Boolean(nullable: false));
            AddColumn("dbo.Forms", "LoginPageTitle", c => c.String());
            AddColumn("dbo.Forms", "RegistrationPageTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Forms", "RegistrationPageTitle");
            DropColumn("dbo.Forms", "LoginPageTitle");
            DropColumn("dbo.Forms", "IsPasswordRequire");
            DropColumn("dbo.Forms", "LoginWindowColor");
            DropColumn("dbo.Forms", "BackGroundColor");
        }
    }
}
