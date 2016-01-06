namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deviceModelModified : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ChannelConfigs", newName: "Channels");
            AddColumn("dbo.Devices", "SMART_SWITCH_6G", c => c.Int(nullable: false));
            AddColumn("dbo.Devices", "SMART_RAINBOW_12", c => c.Int(nullable: false));
            AddColumn("dbo.Devices", "NO_LOAD", c => c.Int(nullable: false));
            AddColumn("dbo.Channels", "NO_LOAD", c => c.Int(nullable: false));
            AddColumn("dbo.Channels", "NON_DIMMABLE_BULB", c => c.Int(nullable: false));
            DropColumn("dbo.Devices", "DeviceType");
            DropColumn("dbo.Channels", "LoadType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Channels", "LoadType", c => c.Int(nullable: false));
            AddColumn("dbo.Devices", "DeviceType", c => c.String());
            DropColumn("dbo.Channels", "NON_DIMMABLE_BULB");
            DropColumn("dbo.Channels", "NO_LOAD");
            DropColumn("dbo.Devices", "NO_LOAD");
            DropColumn("dbo.Devices", "SMART_RAINBOW_12");
            DropColumn("dbo.Devices", "SMART_SWITCH_6G");
            RenameTable(name: "dbo.Channels", newName: "ChannelConfigs");
        }
    }
}
