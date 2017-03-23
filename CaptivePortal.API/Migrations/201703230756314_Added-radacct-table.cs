namespace CaptivePortal.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedradaccttable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Radaccts",
                c => new
                    {
                        RadacctId = c.Long(nullable: false, identity: false),
                        AcctsessionId = c.String(maxLength: 64, unicode: false),
                        AcctuniqueId = c.String(maxLength: 32, unicode: false),
                        UserName = c.String(maxLength: 64, unicode: false),
                        GroupName = c.String(maxLength: 64, unicode: false),
                        NasipAddress = c.String(maxLength: 15, unicode: false),
                        NasportId = c.String(maxLength: 15, unicode: false),
                        NasPortType = c.String(maxLength: 32, unicode: false),
                        AcctstopTime = c.DateTime(nullable: false),
                        AcctstartTime = c.DateTime(nullable: false),
                        AcctsessionTime = c.Int(nullable: false),
                        Acctauthentic = c.String(maxLength: 32, unicode: false),
                        Connectinfo_Start = c.String(maxLength: 50, unicode: false),
                        Connectinfo_Stop = c.String(maxLength: 32, unicode: false),
                        AcctinputOctets = c.Long(nullable: false),
                        AcctoutputOctets = c.Long(nullable: false),
                        CalledstationId = c.String(maxLength: 50, unicode: false),
                        CallingstationId = c.String(maxLength: 50, unicode: false),
                        AcctterminateCause = c.String(maxLength: 50, unicode: false),
                        ServiceType = c.String(maxLength: 50, unicode: false),
                        FramedProtocol = c.String(maxLength: 50, unicode: false),
                        FramedIpAddress = c.String(maxLength: 50, unicode: false),
                        AcctStartDelay = c.Int(nullable: false),
                        AcctStopDelay = c.Int(nullable: false),
                        XascendSessionsvrKey = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.RadacctId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Radaccts");
        }
    }
}
