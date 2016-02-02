namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialJsonModelUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChannelStatus", "Channel_ChannelId", "dbo.Channels");
            DropIndex("dbo.ChannelStatus", new[] { "Channel_ChannelId" });
            RenameColumn(table: "dbo.ChannelStatus", name: "Channel_ChannelId", newName: "ChannelId");
            AddColumn("dbo.Devices", "DId", c => c.Int(nullable: false));
            AlterColumn("dbo.Devices", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ChannelStatus", "ChannelId", c => c.Int(nullable: false));
            CreateIndex("dbo.ChannelStatus", "ChannelId");
            AddForeignKey("dbo.ChannelStatus", "ChannelId", "dbo.Channels", "ChannelId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChannelStatus", "ChannelId", "dbo.Channels");
            DropIndex("dbo.ChannelStatus", new[] { "ChannelId" });
            AlterColumn("dbo.ChannelStatus", "ChannelId", c => c.Int());
            AlterColumn("dbo.Devices", "IsDeleted", c => c.Int(nullable: false));
            DropColumn("dbo.Devices", "DId");
            RenameColumn(table: "dbo.ChannelStatus", name: "ChannelId", newName: "Channel_ChannelId");
            CreateIndex("dbo.ChannelStatus", "Channel_ChannelId");
            AddForeignKey("dbo.ChannelStatus", "Channel_ChannelId", "dbo.Channels", "ChannelId");
        }
    }
}
