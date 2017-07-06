namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddwifiUser_table : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MacAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersAddresses", "UserId", "dbo.Users");
            DropIndex("dbo.MacAddresses", new[] { "UserId" });
            DropIndex("dbo.UsersAddresses", new[] { "UserId" });
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
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Ages", t => t.AgeId)
                .ForeignKey("dbo.Genders", t => t.GenderId)
                .ForeignKey("dbo.Sites", t => t.SiteId)
                .Index(t => t.GenderId)
                .Index(t => t.AgeId)
                .Index(t => t.SiteId);
            
            AlterColumn("dbo.MacAddresses", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.UsersAddresses", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.MacAddresses", "UserId");
            CreateIndex("dbo.UsersAddresses", "UserId");
            AddForeignKey("dbo.MacAddresses", "UserId", "dbo.WifiUsers", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.UsersAddresses", "UserId", "dbo.WifiUsers", "UserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersAddresses", "UserId", "dbo.WifiUsers");
            DropForeignKey("dbo.MacAddresses", "UserId", "dbo.WifiUsers");
            DropForeignKey("dbo.WifiUsers", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.WifiUsers", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.WifiUsers", "AgeId", "dbo.Ages");
            DropIndex("dbo.UsersAddresses", new[] { "UserId" });
            DropIndex("dbo.WifiUsers", new[] { "SiteId" });
            DropIndex("dbo.WifiUsers", new[] { "AgeId" });
            DropIndex("dbo.WifiUsers", new[] { "GenderId" });
            DropIndex("dbo.MacAddresses", new[] { "UserId" });
            AlterColumn("dbo.UsersAddresses", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.MacAddresses", "UserId", c => c.String(maxLength: 128));
            DropTable("dbo.WifiUsers");
            CreateIndex("dbo.UsersAddresses", "UserId");
            CreateIndex("dbo.MacAddresses", "UserId");
            AddForeignKey("dbo.UsersAddresses", "UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.MacAddresses", "UserId", "dbo.Users", "UserId");
        }
    }
}
