namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Watt", c => c.String());
            AddColumn("dbo.VersionDetails", "DType", c => c.String());
            AddColumn("dbo.VersionDetails", "DeviceType", c => c.Int());
            DropTable("dbo.TestTables");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TestTables",
                c => new
                    {
                        TestTableId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TestTableId);
            
            DropColumn("dbo.VersionDetails", "DeviceType");
            DropColumn("dbo.VersionDetails", "DType");
            DropColumn("dbo.Devices", "Watt");
        }
    }
}
