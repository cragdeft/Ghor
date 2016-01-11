namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialModityEnum2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Channels", "LoadType", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Channels", "LoadType", c => c.Int(nullable: false));
        }
    }
}
