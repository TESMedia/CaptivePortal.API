namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changes_in_Site_and_User_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "Term_conditions", c => c.String());
            AddColumn("dbo.Sites", "TermsAndCondDoc", c => c.String());
            AddColumn("dbo.Sites", "FileName", c => c.String());
            AddColumn("dbo.Users", "ThirdPartyOptIn", c => c.Boolean());
            AddColumn("dbo.Users", "UserOfDataOptIn", c => c.Boolean());
            AddColumn("dbo.Users", "Status", c => c.String());
            AlterColumn("dbo.Sites", "AutoLogin", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sites", "AutoLogin", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "Status");
            DropColumn("dbo.Users", "UserOfDataOptIn");
            DropColumn("dbo.Users", "ThirdPartyOptIn");
            DropColumn("dbo.Sites", "FileName");
            DropColumn("dbo.Sites", "TermsAndCondDoc");
            DropColumn("dbo.Sites", "Term_conditions");
        }
    }
}
