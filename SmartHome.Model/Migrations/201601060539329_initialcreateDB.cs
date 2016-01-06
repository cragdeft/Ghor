namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChannelConfigs",
                c => new
                    {
                        ChannelConfigId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DId = c.Int(nullable: false),
                        ChannelNo = c.Int(nullable: false),
                        LoadType = c.Int(nullable: false),
                        LoadName = c.String(),
                        Status = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Device_DeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.ChannelConfigId)
                .ForeignKey("dbo.Devices", t => t.Device_DeviceId)
                .Index(t => t.Device_DeviceId);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        DeviceId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DeviceName = c.String(),
                        DeviceType = c.String(),
                        DeviceHash = c.String(),
                        DeviceVersion = c.String(),
                        IsDeleted = c.Int(nullable: false),
                        Mac = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceId);
            
            CreateTable(
                "dbo.DeviceStatus",
                c => new
                    {
                        DeviceStatusId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DId = c.Int(nullable: false),
                        StatusType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Device_DeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.DeviceStatusId)
                .ForeignKey("dbo.Devices", t => t.Device_DeviceId)
                .Index(t => t.Device_DeviceId);
            
            CreateTable(
                "dbo.ServerResponces",
                c => new
                    {
                        ServerResponceId = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Topic = c.String(),
                        MessageId = c.String(),
                    })
                .PrimaryKey(t => t.ServerResponceId);
            
            CreateTable(
                "dbo.VersionDetails",
                c => new
                    {
                        VersionDetailId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        VId = c.String(),
                        HardwareVersion = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Version_VersionId = c.Int(),
                    })
                .PrimaryKey(t => t.VersionDetailId)
                .ForeignKey("dbo.Versions", t => t.Version_VersionId)
                .Index(t => t.Version_VersionId);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        VersionId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        AppName = c.String(),
                        AppVersion = c.String(),
                        AuthCode = c.String(),
                        PassPhrase = c.String(),
                        MAC = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VersionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VersionDetails", "Version_VersionId", "dbo.Versions");
            DropForeignKey("dbo.DeviceStatus", "Device_DeviceId", "dbo.Devices");
            DropForeignKey("dbo.ChannelConfigs", "Device_DeviceId", "dbo.Devices");
            DropIndex("dbo.VersionDetails", new[] { "Version_VersionId" });
            DropIndex("dbo.DeviceStatus", new[] { "Device_DeviceId" });
            DropIndex("dbo.ChannelConfigs", new[] { "Device_DeviceId" });
            DropTable("dbo.Versions");
            DropTable("dbo.VersionDetails");
            DropTable("dbo.ServerResponces");
            DropTable("dbo.DeviceStatus");
            DropTable("dbo.Devices");
            DropTable("dbo.ChannelConfigs");
        }
    }
}
