namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_WifiUsers_tbl : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WifiUsers", "AgeId", "dbo.Ages");
            DropForeignKey("dbo.WifiUsers", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.WifiUsers", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.MacAddresses", "UserId", "dbo.WifiUsers");
            DropForeignKey("dbo.UsersAddresses", "UserId", "dbo.WifiUsers");
            DropIndex("dbo.WifiUsers", new[] { "GenderId" });
            DropIndex("dbo.WifiUsers", new[] { "AgeId" });
            DropIndex("dbo.WifiUsers", new[] { "SiteId" });
            AddColumn("dbo.Users", "AutoLogin", c => c.Boolean());
            AddColumn("dbo.Users", "Custom1", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom2", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom3", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom4", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom5", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "Custom6", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "UniqueUserId", c => c.String());
            AddForeignKey("dbo.MacAddresses", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UsersAddresses", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            DropTable("dbo.WifiUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WifiUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        UserName = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        CreationDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        BirthDate = c.DateTime(),
                        MobileNumer = c.Int(),
                        GenderId = c.Int(),
                        AgeId = c.Int(),
                        SiteId = c.Int(),
                        AutoLogin = c.Boolean(),
                        promotional_email = c.Boolean(),
                        ThirdPartyOptIn = c.Boolean(),
                        UserOfDataOptIn = c.Boolean(),
                        Status = c.String(maxLength: 50),
                        Custom1 = c.String(maxLength: 50),
                        Custom2 = c.String(maxLength: 50),
                        Custom3 = c.String(maxLength: 50),
                        Custom4 = c.String(maxLength: 50),
                        Custom5 = c.String(maxLength: 50),
                        Custom6 = c.String(maxLength: 50),
                        UniqueUserId = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            DropForeignKey("dbo.UsersAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.MacAddresses", "UserId", "dbo.Users");
            DropColumn("dbo.Users", "UniqueUserId");
            DropColumn("dbo.Users", "Custom6");
            DropColumn("dbo.Users", "Custom5");
            DropColumn("dbo.Users", "Custom4");
            DropColumn("dbo.Users", "Custom3");
            DropColumn("dbo.Users", "Custom2");
            DropColumn("dbo.Users", "Custom1");
            DropColumn("dbo.Users", "AutoLogin");
            CreateIndex("dbo.WifiUsers", "SiteId");
            CreateIndex("dbo.WifiUsers", "AgeId");
            CreateIndex("dbo.WifiUsers", "GenderId");
            AddForeignKey("dbo.UsersAddresses", "UserId", "dbo.WifiUsers", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.MacAddresses", "UserId", "dbo.WifiUsers", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.WifiUsers", "SiteId", "dbo.Sites", "SiteId");
            AddForeignKey("dbo.WifiUsers", "GenderId", "dbo.Genders", "GenderId");
            AddForeignKey("dbo.WifiUsers", "AgeId", "dbo.Ages", "AgeId");
        }
    }
}
