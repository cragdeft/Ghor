namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        Zone = c.String(),
                        Block = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        ZipCode = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
            CreateTable(
                "dbo.Homes",
                c => new
                    {
                        HomeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TimeZone = c.String(),
                        Comment = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        MeshMode = c.Int(nullable: false),
                        Phone = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.HomeId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RoomNumber = c.Int(nullable: false),
                        Comment = c.String(),
                        IsMasterRoom = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Home_HomeId = c.Int(),
                        UserInfo_UserInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId)
                .Index(t => t.Home_HomeId)
                .Index(t => t.UserInfo_UserInfoId);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        DeviceId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DeviceName = c.String(),
                        DeviceHash = c.String(),
                        DeviceVersion = c.String(),
                        IsDeleted = c.Int(nullable: false),
                        Mac = c.String(),
                        DType = c.String(),
                        DeviceType = c.Int(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Room_RoomId = c.Int(),
                    })
                .PrimaryKey(t => t.DeviceId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .Index(t => t.Room_RoomId);
            
            CreateTable(
                "dbo.Channels",
                c => new
                    {
                        ChannelId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DId = c.Int(nullable: false),
                        ChannelNo = c.Int(nullable: false),
                        LoadName = c.String(),
                        Status = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                        LoadType = c.Int(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Device_DeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.ChannelId)
                .ForeignKey("dbo.Devices", t => t.Device_DeviceId)
                .Index(t => t.Device_DeviceId);
            
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
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Device_DeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.DeviceStatusId)
                .ForeignKey("dbo.Devices", t => t.Device_DeviceId)
                .Index(t => t.Device_DeviceId);
            
            CreateTable(
                "dbo.SyncStatus",
                c => new
                    {
                        SyncStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Room_RoomId = c.Int(),
                        UserProfile_UserInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.SyncStatusId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .ForeignKey("dbo.UserInfoes", t => t.UserProfile_UserInfoId)
                .Index(t => t.Room_RoomId)
                .Index(t => t.UserProfile_UserInfoId);
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserInfoId = c.Int(nullable: false, identity: true),
                        LocalId = c.String(),
                        Password = c.String(),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        AccNo = c.String(),
                        CellPhone = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.String(),
                        Email = c.String(),
                        ExpireDate = c.DateTime(),
                        OldAcc = c.String(),
                        SocialSecurityNumber = c.String(),
                        IsEmailRecipient = c.Boolean(nullable: false),
                        IsLoggedIn = c.Boolean(),
                        IsSMSRecipient = c.Boolean(nullable: false),
                        LastLogIn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserInfoId);
            
            CreateTable(
                "dbo.UserStatus",
                c => new
                    {
                        UserStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        UserInfo_UserInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.UserStatusId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId)
                .Index(t => t.UserInfo_UserInfoId);
            
            CreateTable(
                "dbo.UserTypes",
                c => new
                    {
                        UserTypeId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        UserInfo_UserInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.UserTypeId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId)
                .Index(t => t.UserInfo_UserInfoId);
            
            CreateTable(
                "dbo.SmartRouters",
                c => new
                    {
                        SmartRouterId = c.Int(nullable: false, identity: true),
                        IP = c.String(),
                        MacAddress = c.String(),
                        Port = c.String(),
                        RouterUserName = c.String(),
                        RouterPassword = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.SmartRouterId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
            CreateTable(
                "dbo.VersionDetails",
                c => new
                    {
                        VersionDetailId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        VId = c.String(),
                        HardwareVersion = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
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
                        Id = c.String(),
                        AppName = c.String(),
                        AppVersion = c.String(),
                        AuthCode = c.String(),
                        PassPhrase = c.String(),
                        MAC = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.VersionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VersionDetails", "Version_VersionId", "dbo.Versions");
            DropForeignKey("dbo.Addresses", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.SmartRouters", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.UserTypes", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.UserStatus", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.SyncStatus", "UserProfile_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.Rooms", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.SyncStatus", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.Devices", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.DeviceStatus", "Device_DeviceId", "dbo.Devices");
            DropForeignKey("dbo.Channels", "Device_DeviceId", "dbo.Devices");
            DropIndex("dbo.VersionDetails", new[] { "Version_VersionId" });
            DropIndex("dbo.SmartRouters", new[] { "Home_HomeId" });
            DropIndex("dbo.UserTypes", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.UserStatus", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.SyncStatus", new[] { "UserProfile_UserInfoId" });
            DropIndex("dbo.SyncStatus", new[] { "Room_RoomId" });
            DropIndex("dbo.DeviceStatus", new[] { "Device_DeviceId" });
            DropIndex("dbo.Channels", new[] { "Device_DeviceId" });
            DropIndex("dbo.Devices", new[] { "Room_RoomId" });
            DropIndex("dbo.Rooms", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.Rooms", new[] { "Home_HomeId" });
            DropIndex("dbo.Addresses", new[] { "Home_HomeId" });
            DropTable("dbo.Versions");
            DropTable("dbo.VersionDetails");
            DropTable("dbo.SmartRouters");
            DropTable("dbo.UserTypes");
            DropTable("dbo.UserStatus");
            DropTable("dbo.UserInfoes");
            DropTable("dbo.SyncStatus");
            DropTable("dbo.DeviceStatus");
            DropTable("dbo.Channels");
            DropTable("dbo.Devices");
            DropTable("dbo.Rooms");
            DropTable("dbo.Homes");
            DropTable("dbo.Addresses");
        }
    }
}