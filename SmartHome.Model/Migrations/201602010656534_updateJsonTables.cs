namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateJsonTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChannelStatus", "ChannelId", "dbo.Channels");
            DropIndex("dbo.ChannelStatus", new[] { "ChannelId" });
            RenameColumn(table: "dbo.ChannelStatus", name: "ChannelId", newName: "Channel_ChannelId");
            AddColumn("dbo.ChannelStatus", "CId", c => c.Int(nullable: false));
            AlterColumn("dbo.ChannelStatus", "Channel_ChannelId", c => c.Int());
            CreateIndex("dbo.ChannelStatus", "Channel_ChannelId");
            AddForeignKey("dbo.ChannelStatus", "Channel_ChannelId", "dbo.Channels", "ChannelId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChannelStatus", "Channel_ChannelId", "dbo.Channels");
            DropIndex("dbo.ChannelStatus", new[] { "Channel_ChannelId" });
            AlterColumn("dbo.ChannelStatus", "Channel_ChannelId", c => c.Int(nullable: false));
            DropColumn("dbo.ChannelStatus", "CId");
            RenameColumn(table: "dbo.ChannelStatus", name: "Channel_ChannelId", newName: "ChannelId");
            CreateIndex("dbo.ChannelStatus", "ChannelId");
            AddForeignKey("dbo.ChannelStatus", "ChannelId", "dbo.Channels", "ChannelId", cascadeDelete: true);
        }
    }
}
