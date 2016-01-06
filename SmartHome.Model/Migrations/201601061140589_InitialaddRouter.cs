namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialaddRouter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SmartRouters",
                c => new
                    {
                        SmartRouterId = c.Int(nullable: false, identity: true),
                        BrokerIp = c.String(),
                        BrokerMac = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.SmartRouterId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SmartRouters", "Home_HomeId", "dbo.Homes");
            DropIndex("dbo.SmartRouters", new[] { "Home_HomeId" });
            DropTable("dbo.SmartRouters");
        }
    }
}
