namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ManagePromotion_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManagePromotions",
                c => new
                    {
                        ManagePromotionId = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        SuccessPageOption = c.String(),
                        WebPageURL = c.String(),
                        OptionalPictureForSuccessPage = c.String(),
                    })
                .PrimaryKey(t => t.ManagePromotionId)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ManagePromotions", "SiteId", "dbo.Sites");
            DropIndex("dbo.ManagePromotions", new[] { "SiteId" });
            DropTable("dbo.ManagePromotions");
        }
    }
}
