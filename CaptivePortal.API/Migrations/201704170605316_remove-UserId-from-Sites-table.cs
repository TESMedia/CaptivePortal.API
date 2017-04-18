namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUserIdfromSitestable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sites", "UserId", "dbo.Users");
            DropIndex("dbo.Sites", new[] { "UserId" });
            DropColumn("dbo.Sites", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sites", "UserId");
            AddForeignKey("dbo.Sites", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
        }
    }
}
