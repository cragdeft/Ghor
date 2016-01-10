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
                        City = c.String(),
                        OfficePhone = c.String(),
                        WorkPhone = c.String(),
                        ZipCode = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Home_HomeId = c.Int(),
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.Home_HomeId)
                .Index(t => t.UserProfile_UserProfileId);
            
            CreateTable(
                "dbo.Homes",
                c => new
                    {
                        HomeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Zone = c.String(),
                        Block = c.String(),
                        TimeZone = c.String(),
                        RegistrationKey = c.String(),
                        HardwareId = c.String(),
                        TrialCount = c.Int(nullable: false),
                        Comment = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.HomeId);
            
            CreateTable(
                "dbo.HomeVersions",
                c => new
                    {
                        HomeVersionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        LaunchDate = c.DateTime(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.HomeVersionId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
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
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
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
                        DeviceType = c.Int(nullable: false),
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
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.SyncStatusId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.Room_RoomId)
                .Index(t => t.UserProfile_UserProfileId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        LocalId = c.String(),
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
                        Room_RoomId = c.Int(),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .Index(t => t.Room_RoomId);
            
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
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.UserStatusId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.UserProfile_UserProfileId);
            
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
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.UserTypeId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.UserProfile_UserProfileId);
            
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
                "dbo.ChannelConfigs",
                c => new
                    {
                        ChannelId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        DId = c.Int(nullable: false),
                        ChannelNo = c.Int(nullable: false),
                        LoadName = c.String(),
                        Status = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                        LoadType = c.Int(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ChannelId);
            
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
                        Id = c.Int(nullable: false),
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
            DropForeignKey("dbo.SmartRouters", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.UserTypes", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserStatus", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SyncStatus", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Addresses", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SyncStatus", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.Devices", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.DeviceStatus", "Device_DeviceId", "dbo.Devices");
            DropForeignKey("dbo.HomeVersions", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.Addresses", "Home_HomeId", "dbo.Homes");
            DropIndex("dbo.VersionDetails", new[] { "Version_VersionId" });
            DropIndex("dbo.SmartRouters", new[] { "Home_HomeId" });
            DropIndex("dbo.UserTypes", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.UserStatus", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.UserProfiles", new[] { "Room_RoomId" });
            DropIndex("dbo.SyncStatus", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.SyncStatus", new[] { "Room_RoomId" });
            DropIndex("dbo.DeviceStatus", new[] { "Device_DeviceId" });
            DropIndex("dbo.Devices", new[] { "Room_RoomId" });
            DropIndex("dbo.Rooms", new[] { "Home_HomeId" });
            DropIndex("dbo.HomeVersions", new[] { "Home_HomeId" });
            DropIndex("dbo.Addresses", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.Addresses", new[] { "Home_HomeId" });
            DropTable("dbo.Versions");
            DropTable("dbo.VersionDetails");
            DropTable("dbo.ChannelConfigs");
            DropTable("dbo.SmartRouters");
            DropTable("dbo.UserTypes");
            DropTable("dbo.UserStatus");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.SyncStatus");
            DropTable("dbo.DeviceStatus");
            DropTable("dbo.Devices");
            DropTable("dbo.Rooms");
            DropTable("dbo.HomeVersions");
            DropTable("dbo.Homes");
            DropTable("dbo.Addresses");
        }
    }
}
