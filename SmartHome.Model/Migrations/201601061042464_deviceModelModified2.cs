namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deviceModelModified2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "DeviceType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "DeviceType");
        }
    }
}
