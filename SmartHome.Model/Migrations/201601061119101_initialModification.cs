namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialModification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channels", "LoadType", c => c.Int(nullable: false));
            DropColumn("dbo.Devices", "NO_LOAD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "NO_LOAD", c => c.Int(nullable: false));
            DropColumn("dbo.Channels", "LoadType");
        }
    }
}
