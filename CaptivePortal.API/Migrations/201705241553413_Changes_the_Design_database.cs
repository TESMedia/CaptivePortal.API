namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes_the_Design_database : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ages",
                c => new
                    {
                        AgeId = c.Int(nullable: false, identity: true),
                        Value = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.AgeId);
            
            CreateTable(
                "dbo.Genders",
                c => new
                    {
                        GenderId = c.Int(nullable: false, identity: true),
                        Value = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.GenderId);
            
            CreateTable(
                "dbo.MacAddresses",
                c => new
                    {
                        MacId = c.Int(nullable: false, identity: true),
                        MacAddressValue = c.String(maxLength: 20),
                        UserId = c.Int(nullable: false),
                        BrowserName = c.String(),
                        OperatingSystem = c.String(),
                        IsMobile = c.Boolean(nullable: false),
                        UserAgentName = c.String(),
                    })
                .PrimaryKey(t => t.MacId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "BirthDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "MobileNumer", c => c.Int());
            AddColumn("dbo.Users", "GenderId", c => c.Int());
            AddColumn("dbo.Users", "AgeId", c => c.Int());
            AddColumn("dbo.Users", "UniqueUserId", c => c.String());
            AlterColumn("dbo.Users", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "UserName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Password", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Status", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Custom1", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Custom2", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Custom3", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Custom4", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Custom5", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Custom6", c => c.String(maxLength: 50));
            CreateIndex("dbo.Users", "GenderId");
            CreateIndex("dbo.Users", "AgeId");
            AddForeignKey("dbo.Users", "AgeId", "dbo.Ages", "AgeId");
            AddForeignKey("dbo.Users", "GenderId", "dbo.Genders", "GenderId");
            DropColumn("dbo.Users", "CreationBy");
            DropColumn("dbo.Users", "UpdatedBy");
            DropColumn("dbo.Users", "Gender");
            DropColumn("dbo.Users", "Age");
            DropColumn("dbo.Users", "Term_conditions");
            DropColumn("dbo.Users", "IntStatus");
            DropColumn("dbo.Users", "MacAddress");
            DropColumn("dbo.Users", "OperatingSystem");
            DropColumn("dbo.Users", "IsMobile");
            DropColumn("dbo.Users", "Browser");
            DropColumn("dbo.Users", "UserAgentName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "UserAgentName", c => c.String());
            AddColumn("dbo.Users", "Browser", c => c.String());
            AddColumn("dbo.Users", "IsMobile", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "OperatingSystem", c => c.String());
            AddColumn("dbo.Users", "MacAddress", c => c.String());
            AddColumn("dbo.Users", "IntStatus", c => c.Int());
            AddColumn("dbo.Users", "Term_conditions", c => c.String());
            AddColumn("dbo.Users", "Age", c => c.String());
            AddColumn("dbo.Users", "Gender", c => c.String());
            AddColumn("dbo.Users", "UpdatedBy", c => c.String());
            AddColumn("dbo.Users", "CreationBy", c => c.String());
            DropForeignKey("dbo.MacAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.Users", "AgeId", "dbo.Ages");
            DropIndex("dbo.Users", new[] { "AgeId" });
            DropIndex("dbo.Users", new[] { "GenderId" });
            DropIndex("dbo.MacAddresses", new[] { "UserId" });
            AlterColumn("dbo.Users", "Custom6", c => c.String());
            AlterColumn("dbo.Users", "Custom5", c => c.String());
            AlterColumn("dbo.Users", "Custom4", c => c.String());
            AlterColumn("dbo.Users", "Custom3", c => c.String());
            AlterColumn("dbo.Users", "Custom2", c => c.String());
            AlterColumn("dbo.Users", "Custom1", c => c.String());
            AlterColumn("dbo.Users", "Status", c => c.String());
            AlterColumn("dbo.Users", "Password", c => c.String());
            AlterColumn("dbo.Users", "UserName", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "LastName", c => c.String());
            AlterColumn("dbo.Users", "FirstName", c => c.String());
            DropColumn("dbo.Users", "UniqueUserId");
            DropColumn("dbo.Users", "AgeId");
            DropColumn("dbo.Users", "GenderId");
            DropColumn("dbo.Users", "MobileNumer");
            DropColumn("dbo.Users", "BirthDate");
            DropTable("dbo.MacAddresses");
            DropTable("dbo.Genders");
            DropTable("dbo.Ages");
        }
    }
}
