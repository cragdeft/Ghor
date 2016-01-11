namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialModityEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Devices", "DeviceType", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Devices", "DeviceType", c => c.Int(nullable: false));
        }
    }
}
