namespace SmartHome.Model.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SmartHome.Model.ModelDataContext.SmartHomeDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SmartHome.Model.ModelDataContext.SmartHomeDataContext context)
        {           
        }
    }
}
