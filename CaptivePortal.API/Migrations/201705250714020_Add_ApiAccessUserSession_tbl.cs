namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ApiAccessUserSession_tbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApiAccessUserSessions",
                c => new
                    {
                        UserSessionId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SessionId = c.String(),
                    })
                .PrimaryKey(t => t.UserSessionId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Roles", "Session", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApiAccessUserSessions", "UserId", "dbo.Users");
            DropIndex("dbo.ApiAccessUserSessions", new[] { "UserId" });
            DropColumn("dbo.Roles", "Session");
            DropTable("dbo.ApiAccessUserSessions");
        }
    }
}
