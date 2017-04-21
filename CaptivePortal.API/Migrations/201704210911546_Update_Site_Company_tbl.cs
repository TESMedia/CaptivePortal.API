namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Site_Company_tbl : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Sites", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Companies", new[] { "OrganisationId" });
            DropIndex("dbo.Sites", new[] { "CompanyId" });
            AlterColumn("dbo.Companies", "OrganisationId", c => c.Int());
            AlterColumn("dbo.Sites", "CompanyId", c => c.Int());
            CreateIndex("dbo.Companies", "OrganisationId");
            CreateIndex("dbo.Sites", "CompanyId");
            AddForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations", "OrganisationId");
            AddForeignKey("dbo.Sites", "CompanyId", "dbo.Companies", "CompanyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sites", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.Sites", new[] { "CompanyId" });
            DropIndex("dbo.Companies", new[] { "OrganisationId" });
            AlterColumn("dbo.Sites", "CompanyId", c => c.Int(nullable: false));
            AlterColumn("dbo.Companies", "OrganisationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sites", "CompanyId");
            CreateIndex("dbo.Companies", "OrganisationId");
            AddForeignKey("dbo.Sites", "CompanyId", "dbo.Companies", "CompanyId", cascadeDelete: true);
            AddForeignKey("dbo.Companies", "OrganisationId", "dbo.Organisations", "OrganisationId", cascadeDelete: true);
        }
    }
}
