namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreateDBForHome : DbMigration
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
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
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
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Comment = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HomeId);
            
            CreateTable(
                "dbo.CommTypes",
                c => new
                    {
                        CommTypeId = c.Int(nullable: false, identity: true),
                        Comm = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.CommTypeId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
            CreateTable(
                "dbo.HomeLinks",
                c => new
                    {
                        HomeLinkId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                        IPAddress = c.String(),
                        Port = c.Int(nullable: false),
                        VideoQuality = c.String(),
                        ChannelCount = c.Int(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.HomeLinkId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
            CreateTable(
                "dbo.LinkTypes",
                c => new
                    {
                        LinkTypeId = c.Int(nullable: false, identity: true),
                        Link = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        HomeLink_HomeLinkId = c.Int(),
                    })
                .PrimaryKey(t => t.LinkTypeId)
                .ForeignKey("dbo.HomeLinks", t => t.HomeLink_HomeLinkId)
                .Index(t => t.HomeLink_HomeLinkId);
            
            CreateTable(
                "dbo.SyncStatus",
                c => new
                    {
                        SyncStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        HomeLink_HomeLinkId = c.Int(),
                        Room_RoomId = c.Int(),
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.SyncStatusId)
                .ForeignKey("dbo.HomeLinks", t => t.HomeLink_HomeLinkId)
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.HomeLink_HomeLinkId)
                .Index(t => t.Room_RoomId)
                .Index(t => t.UserProfile_UserProfileId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RoomNumber = c.Int(nullable: false),
                        Comment = c.String(),
                        IsMasterRoom = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        LocalID = c.String(),
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
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
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
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.UserTypeId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId)
                .Index(t => t.UserProfile_UserProfileId);
            
            CreateTable(
                "dbo.HomeStatus",
                c => new
                    {
                        HomeStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AuditField_InsertedBy = c.String(),
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.HomeStatusId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
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
                        AuditField_InsertedDateTime = c.DateTime(nullable: false),
                        AuditField_LastUpdatedBy = c.String(),
                        AuditField_LastUpdatedDateTime = c.DateTime(nullable: false),
                        Home_HomeId = c.Int(),
                    })
                .PrimaryKey(t => t.HomeVersionId)
                .ForeignKey("dbo.Homes", t => t.Home_HomeId)
                .Index(t => t.Home_HomeId);
            
            AddColumn("dbo.Devices", "Room_RoomId", c => c.Int());
            CreateIndex("dbo.Devices", "Room_RoomId");
            AddForeignKey("dbo.Devices", "Room_RoomId", "dbo.Rooms", "RoomId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HomeVersions", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.HomeStatus", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.UserTypes", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserStatus", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SyncStatus", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Addresses", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.SyncStatus", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.Devices", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.SyncStatus", "HomeLink_HomeLinkId", "dbo.HomeLinks");
            DropForeignKey("dbo.LinkTypes", "HomeLink_HomeLinkId", "dbo.HomeLinks");
            DropForeignKey("dbo.HomeLinks", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.CommTypes", "Home_HomeId", "dbo.Homes");
            DropForeignKey("dbo.Addresses", "Home_HomeId", "dbo.Homes");
            DropIndex("dbo.HomeVersions", new[] { "Home_HomeId" });
            DropIndex("dbo.HomeStatus", new[] { "Home_HomeId" });
            DropIndex("dbo.UserTypes", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.UserStatus", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.UserProfiles", new[] { "Room_RoomId" });
            DropIndex("dbo.Devices", new[] { "Room_RoomId" });
            DropIndex("dbo.Rooms", new[] { "Home_HomeId" });
            DropIndex("dbo.SyncStatus", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.SyncStatus", new[] { "Room_RoomId" });
            DropIndex("dbo.SyncStatus", new[] { "HomeLink_HomeLinkId" });
            DropIndex("dbo.LinkTypes", new[] { "HomeLink_HomeLinkId" });
            DropIndex("dbo.HomeLinks", new[] { "Home_HomeId" });
            DropIndex("dbo.CommTypes", new[] { "Home_HomeId" });
            DropIndex("dbo.Addresses", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.Addresses", new[] { "Home_HomeId" });
            DropColumn("dbo.Devices", "Room_RoomId");
            DropTable("dbo.HomeVersions");
            DropTable("dbo.HomeStatus");
            DropTable("dbo.UserTypes");
            DropTable("dbo.UserStatus");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.Rooms");
            DropTable("dbo.SyncStatus");
            DropTable("dbo.LinkTypes");
            DropTable("dbo.HomeLinks");
            DropTable("dbo.CommTypes");
            DropTable("dbo.Homes");
            DropTable("dbo.Addresses");
        }
    }
}
