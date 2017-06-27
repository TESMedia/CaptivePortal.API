namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rename_default_identirty_teble : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AspNetUsers", newName: "Users");
            RenameTable(name: "dbo.AspNetUserClaims", newName: "UserClaims");
            RenameTable(name: "dbo.AspNetUserLogins", newName: "UserLogins");
            RenameTable(name: "dbo.AspNetUserRoles", newName: "UserRoles");
            RenameTable(name: "dbo.AspNetRoles", newName: "Roles");
            RenameColumn(table: "dbo.Users", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.UserClaims", name: "Id", newName: "UserClaimId");
            RenameColumn(table: "dbo.Roles", name: "Id", newName: "RoleId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Roles", name: "RoleId", newName: "Id");
            RenameColumn(table: "dbo.UserClaims", name: "UserClaimId", newName: "Id");
            RenameColumn(table: "dbo.Users", name: "UserId", newName: "Id");
            RenameTable(name: "dbo.Roles", newName: "AspNetRoles");
            RenameTable(name: "dbo.UserRoles", newName: "AspNetUserRoles");
            RenameTable(name: "dbo.UserLogins", newName: "AspNetUserLogins");
            RenameTable(name: "dbo.UserClaims", newName: "AspNetUserClaims");
            RenameTable(name: "dbo.Users", newName: "AspNetUsers");
        }
    }
}
