namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceStatus", "Value", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceStatus", "Value");
        }
    }
}
