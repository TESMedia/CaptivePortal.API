namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_GroupTable_And_GroupId_FK_in_UserTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        Rule = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            AddColumn("dbo.Users", "GroupId", c => c.Int());
            AddColumn("dbo.Companies", "CompanyIcon", c => c.String());
            CreateIndex("dbo.Users", "GroupId");
            AddForeignKey("dbo.Users", "GroupId", "dbo.Groups", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "GroupId", "dbo.Groups");
            DropIndex("dbo.Users", new[] { "GroupId" });
            DropColumn("dbo.Companies", "CompanyIcon");
            DropColumn("dbo.Users", "GroupId");
            DropTable("dbo.Groups");
        }
    }
}
