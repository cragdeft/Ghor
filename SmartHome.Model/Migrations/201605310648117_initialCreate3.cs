namespace SmartHome.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserInfoes", "DateOfBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserInfoes", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
