namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate3 : DbMigration
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
                        AppsHomeId = c.Int(nullable: false),
                        Name = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        Block = c.String(),
                        City = c.String(),
                        ZipCode = c.String(),
                        Country = c.String(),
                        TimeZone = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        MeshMode = c.Int(nullable: false),
                        Phone = c.String(),
                        PassPhrase = c.String(),
                        Zone = c.String(),
                        IsInternet = c.Boolean(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
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
                        AppsRoomId = c.Int(nullable: false),
                        AppsHomeId = c.String(),
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
                        Home_HomeId1 = c.Int(),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId1, cascadeDelete: true)
                .Index(t => t.Home_HomeId)
                .Index(t => t.Home_HomeId1);
            
            CreateTable(
                "dbo.SmartDevices",
                c => new
                    {
                        DeviceId = c.Int(nullable: false, identity: true),
                        AppsDeviceId = c.Int(nullable: false),
                        AppsRoomId = c.Int(nullable: false),
                        AppsBleId = c.Int(nullable: false),
                        DeviceName = c.String(),
                        DeviceHash = c.String(),
                        DeviceVersion = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Watt = c.String(),
                        IsSynced = c.Boolean(nullable: false),
                        DeviceType = c.Int(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        AppsRouterId = c.Int(),
                        LocalBrokerIp = c.String(),
                        LocalBrokerPort = c.String(),
                        MacAddress = c.String(),
                        Ssid = c.String(),
                        SsidPassword = c.String(),
                        LocalBrokerUsername = c.String(),
                        LocalBrokerPassword = c.String(),
                        Room_RoomId = c.Int(),
                        Parent_HomeId = c.Int(),
                        Room_RoomId1 = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DeviceId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .ForeignKey("dbo.Homes", t => t.Parent_HomeId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId1, cascadeDelete: true)
                .Index(t => t.Room_RoomId)
                .Index(t => t.Parent_HomeId)
                .Index(t => t.Room_RoomId1);
            
            CreateTable(
                "dbo.DeviceStatus",
                c => new
                    {
                        DeviceStatusId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DId = c.Int(nullable: false),
                        StatusType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Value = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        SmartDevice_DeviceId = c.Int(),
                        SmartDevice_DeviceId1 = c.Int(),
                    })
                .PrimaryKey(t => t.DeviceStatusId)
                .ForeignKey("dbo.SmartDevices", t => t.SmartDevice_DeviceId)
                .ForeignKey("dbo.SmartDevices", t => t.SmartDevice_DeviceId1, cascadeDelete: true)
                .Index(t => t.SmartDevice_DeviceId)
                .Index(t => t.SmartDevice_DeviceId1);
            
            CreateTable(
                "dbo.RgbwStatus",
                c => new
                    {
                        RgbwStatusId = c.Int(nullable: false, identity: true),
                        AppsRgbtStatusId = c.Int(nullable: false),
                        AppsDeviceId = c.Int(nullable: false),
                        RGBColorStatusType = c.Int(nullable: false),
                        IsPowerOn = c.Boolean(nullable: false),
                        ColorR = c.Int(nullable: false),
                        ColorG = c.Int(nullable: false),
                        ColorB = c.Int(nullable: false),
                        IsWhiteEnabled = c.Boolean(nullable: false),
                        DimmingValue = c.Int(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        SmartDevice_DeviceId = c.Int(),
                        SmartRainbow_DeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.RgbwStatusId)
                .ForeignKey("dbo.SmartDevices", t => t.SmartDevice_DeviceId)
                .ForeignKey("dbo.SmartDevices", t => t.SmartRainbow_DeviceId)
                .Index(t => t.SmartDevice_DeviceId)
                .Index(t => t.SmartRainbow_DeviceId);
            
            CreateTable(
                "dbo.Channels",
                c => new
                    {
                        ChannelId = c.Int(nullable: false, identity: true),
                        AppsChannelId = c.Int(nullable: false),
                        AppsDeviceTableId = c.Int(nullable: false),
                        ChannelNo = c.Int(nullable: false),
                        LoadName = c.String(),
                        LoadType = c.Int(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        SmartSwitch_DeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.ChannelId)
                .ForeignKey("dbo.SmartDevices", t => t.SmartSwitch_DeviceId, cascadeDelete: true)
                .Index(t => t.SmartSwitch_DeviceId);
            
            CreateTable(
                "dbo.ChannelStatus",
                c => new
                    {
                        ChannelStatusId = c.Int(nullable: false, identity: true),
                        AppsChannelStatusId = c.Int(nullable: false),
                        AppsChannelId = c.Int(nullable: false),
                        StatusType = c.Int(nullable: false),
                        StatusValue = c.Int(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Channel_ChannelId = c.Int(),
                        Channel_ChannelId1 = c.Int(),
                    })
                .PrimaryKey(t => t.ChannelStatusId)
                .ForeignKey("dbo.Channels", t => t.Channel_ChannelId)
                .ForeignKey("dbo.Channels", t => t.Channel_ChannelId1, cascadeDelete: true)
                .Index(t => t.Channel_ChannelId)
                .Index(t => t.Channel_ChannelId1);
            
            CreateTable(
                "dbo.UserRoomLinks",
                c => new
                    {
                        AppsRoomId = c.Int(nullable: false),
                        AppsUserId = c.Int(nullable: false),
                        AppsUserRoomLinkId = c.Int(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
                        Room_RoomId = c.Int(),
                        UserInfo_UserInfoId = c.Int(),
                        Room_RoomId1 = c.Int(),
                    })
                .PrimaryKey(t => new { t.AppsRoomId, t.AppsUserId })
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId1, cascadeDelete: true)
                .Index(t => t.Room_RoomId)
                .Index(t => t.UserInfo_UserInfoId)
                .Index(t => t.Room_RoomId1);
            
            CreateTable(
                "dbo.UserInfoes",
                c => new
                    {
                        UserInfoId = c.Int(nullable: false, identity: true),
                        AppsUserId = c.Int(nullable: false),
                        LocalId = c.String(),
                        Password = c.String(),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        AccNo = c.String(),
                        CellPhone = c.String(),
                        DateOfBirth = c.DateTime(),
                        Sex = c.String(),
                        Email = c.String(),
                        ExpireDate = c.DateTime(),
                        OldAcc = c.String(),
                        SocialSecurityNumber = c.String(),
                        IsEmailRecipient = c.Boolean(nullable: false),
                        IsLoggedIn = c.Boolean(),
                        IsSMSRecipient = c.Boolean(nullable: false),
                        LastLogIn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        Country = c.String(),
                        LoginStatus = c.Boolean(nullable: false),
                        RegStatus = c.Boolean(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserInfoId);
            
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
                        UserInfo_UserInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.SyncStatusId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .ForeignKey("dbo.UserInfoes", t => t.UserProfile_UserInfoId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId, cascadeDelete: true)
                .Index(t => t.Room_RoomId)
                .Index(t => t.UserProfile_UserInfoId)
                .Index(t => t.UserInfo_UserInfoId);
            
            CreateTable(
                "dbo.UserHomeLinks",
                c => new
                    {
                        AppsHomeId = c.Int(nullable: false),
                        AppsUserId = c.Int(nullable: false),
                        AppsUserHomeLinkId = c.Int(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
                        Home_HomeId = c.Int(),
                        UserInfo_UserInfoId = c.Int(),
                        Home_HomeId1 = c.Int(),
                    })
                .PrimaryKey(t => new { t.AppsHomeId, t.AppsUserId })
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId1, cascadeDelete: true)
                .Index(t => t.Home_HomeId)
                .Index(t => t.UserInfo_UserInfoId)
                .Index(t => t.Home_HomeId1);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserInfo_UserInfoId = c.Int(),
                        WebPagesRole_RoleId = c.Int(),
                    })
                .PrimaryKey(t => t.UserRoleId)
                .ForeignKey("dbo.UserInfoes", t => t.UserInfo_UserInfoId)
                .ForeignKey("dbo.WebPagesRoles", t => t.WebPagesRole_RoleId)
                .Index(t => t.UserInfo_UserInfoId)
                .Index(t => t.WebPagesRole_RoleId);
            
            CreateTable(
                "dbo.WebPagesRoles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
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
                "dbo.SmartRouterInfoes",
                c => new
                    {
                        SmartRouterId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        HId = c.String(),
                        IP = c.String(),
                        MacAddress = c.String(),
                        Port = c.String(),
                        Ssid = c.String(),
                        LocalBrokerUsername = c.String(),
                        LocalBrokerPassword = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsSynced = c.Boolean(nullable: false),
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
                "dbo.CommandJsons",
                c => new
                    {
                        CommandJsonId = c.Int(nullable: false, identity: true),
                        CommandId = c.Int(nullable: false),
                        Command = c.String(),
                        DeviceId = c.Int(nullable: false),
                        DeviceUUId = c.Int(nullable: false),
                        Response = c.Boolean(nullable: false),
                        DeviceVersion = c.String(),
                        Mac = c.String(),
                        EmailAddress = c.String(),
                        IsProcessed = c.Boolean(nullable: false),
                        CommandType = c.Int(nullable: false),
                        ProcessFailReason = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.CommandJsonId);
            
            CreateTable(
                "dbo.VersionDetails",
                c => new
                    {
                        VersionDetailId = c.Int(nullable: false, identity: true),
                        AppsVersionDetailId = c.Int(nullable: false),
                        AppsVersionId = c.Int(nullable: false),
                        HardwareVersion = c.String(),
                        DType = c.String(),
                        DeviceType = c.Int(),
                        IsSynced = c.Boolean(nullable: false),
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
                        AppsVersionId = c.Int(nullable: false),
                        AppName = c.String(),
                        AppVersion = c.String(),
                        AuthCode = c.String(),
                        PassPhrase = c.String(),
                        Mac = c.String(),
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
            DropForeignKey("dbo.UserHomeLinks", "Home_HomeId1", "dbo.Homes");
            DropForeignKey("dbo.SmartRouterInfoes", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.Rooms", "Home_HomeId1", "dbo.Homes");
            DropForeignKey("dbo.UserRoomLinks", "Room_RoomId1", "dbo.Rooms");
            DropForeignKey("dbo.UserTypes", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.UserStatus", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.UserRoomLinks", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.UserRoles", "WebPagesRole_RoleId", "dbo.WebPagesRoles");
            DropForeignKey("dbo.UserRoles", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.UserHomeLinks", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.UserHomeLinks", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.SyncStatus", "UserInfo_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.SyncStatus", "UserProfile_UserInfoId", "dbo.UserInfoes");
            DropForeignKey("dbo.SyncStatus", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.UserRoomLinks", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.SmartDevices", "Room_RoomId1", "dbo.Rooms");
            DropForeignKey("dbo.Channels", "SmartSwitch_DeviceId", "dbo.SmartDevices");
            DropForeignKey("dbo.ChannelStatus", "Channel_ChannelId1", "dbo.Channels");
            DropForeignKey("dbo.ChannelStatus", "Channel_ChannelId", "dbo.Channels");
            DropForeignKey("dbo.SmartDevices", "Parent_HomeId", "dbo.Homes");
            DropForeignKey("dbo.RgbwStatus", "SmartRainbow_DeviceId", "dbo.SmartDevices");
            DropForeignKey("dbo.RgbwStatus", "SmartDevice_DeviceId", "dbo.SmartDevices");
            DropForeignKey("dbo.SmartDevices", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.DeviceStatus", "SmartDevice_DeviceId1", "dbo.SmartDevices");
            DropForeignKey("dbo.DeviceStatus", "SmartDevice_DeviceId", "dbo.SmartDevices");
            DropForeignKey("dbo.Rooms", "Home_HomeId", "dbo.Homes");
            DropIndex("dbo.VersionDetails", new[] { "Version_VersionId" });
            DropIndex("dbo.SmartRouterInfoes", new[] { "Home_HomeId" });
            DropIndex("dbo.UserTypes", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.UserStatus", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.UserRoles", new[] { "WebPagesRole_RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.UserHomeLinks", new[] { "Home_HomeId1" });
            DropIndex("dbo.UserHomeLinks", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.UserHomeLinks", new[] { "Home_HomeId" });
            DropIndex("dbo.SyncStatus", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.SyncStatus", new[] { "UserProfile_UserInfoId" });
            DropIndex("dbo.SyncStatus", new[] { "Room_RoomId" });
            DropIndex("dbo.UserRoomLinks", new[] { "Room_RoomId1" });
            DropIndex("dbo.UserRoomLinks", new[] { "UserInfo_UserInfoId" });
            DropIndex("dbo.UserRoomLinks", new[] { "Room_RoomId" });
            DropIndex("dbo.ChannelStatus", new[] { "Channel_ChannelId1" });
            DropIndex("dbo.ChannelStatus", new[] { "Channel_ChannelId" });
            DropIndex("dbo.Channels", new[] { "SmartSwitch_DeviceId" });
            DropIndex("dbo.RgbwStatus", new[] { "SmartRainbow_DeviceId" });
            DropIndex("dbo.RgbwStatus", new[] { "SmartDevice_DeviceId" });
            DropIndex("dbo.DeviceStatus", new[] { "SmartDevice_DeviceId1" });
            DropIndex("dbo.DeviceStatus", new[] { "SmartDevice_DeviceId" });
            DropIndex("dbo.SmartDevices", new[] { "Room_RoomId1" });
            DropIndex("dbo.SmartDevices", new[] { "Parent_HomeId" });
            DropIndex("dbo.SmartDevices", new[] { "Room_RoomId" });
            DropIndex("dbo.Rooms", new[] { "Home_HomeId1" });
            DropIndex("dbo.Rooms", new[] { "Home_HomeId" });
            DropIndex("dbo.Addresses", new[] { "Home_HomeId" });
            DropTable("dbo.Versions");
            DropTable("dbo.VersionDetails");
            DropTable("dbo.CommandJsons");
            DropTable("dbo.SmartRouterInfoes");
            DropTable("dbo.UserTypes");
            DropTable("dbo.UserStatus");
            DropTable("dbo.WebPagesRoles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserHomeLinks");
            DropTable("dbo.SyncStatus");
            DropTable("dbo.UserInfoes");
            DropTable("dbo.UserRoomLinks");
            DropTable("dbo.ChannelStatus");
            DropTable("dbo.Channels");
            DropTable("dbo.RgbwStatus");
            DropTable("dbo.DeviceStatus");
            DropTable("dbo.SmartDevices");
            DropTable("dbo.Rooms");
            DropTable("dbo.Homes");
            DropTable("dbo.Addresses");
        }
    }
}
