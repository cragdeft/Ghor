namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialJsonParse : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ChannelConfigs", newName: "Channels");
            AddColumn("dbo.Homes", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.Devices", "DType", c => c.String());
            AlterColumn("dbo.Versions", "Id", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Versions", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.Devices", "DType");
            DropColumn("dbo.Homes", "IsDefault");
            RenameTable(name: "dbo.Channels", newName: "ChannelConfigs");
        }
    }
}
