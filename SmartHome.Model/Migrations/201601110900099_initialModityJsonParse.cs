namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialModityJsonParse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channels", "Device_DeviceId", c => c.Int());
            CreateIndex("dbo.Channels", "Device_DeviceId");
            AddForeignKey("dbo.Channels", "Device_DeviceId", "dbo.Devices", "DeviceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Channels", "Device_DeviceId", "dbo.Devices");
            DropIndex("dbo.Channels", new[] { "Device_DeviceId" });
            DropColumn("dbo.Channels", "Device_DeviceId");
        }
    }
}
