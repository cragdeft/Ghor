namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NextAssociatedDevices",
                c => new
                    {
                        NextAssociatedDeviceId = c.Int(nullable: false, identity: true),
                        NextDeviceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NextAssociatedDeviceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NextAssociatedDevices");
        }
    }
}
