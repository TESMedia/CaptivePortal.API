namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeinmodelDatatype : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "CompanyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CompanyId", c => c.String());
        }
    }
}
