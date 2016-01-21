using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.ModelDataContext
{

    public class SmartHomeDataContextDbInitializer : DropCreateDatabaseAlways<SmartHomeDataContext>
    {
        protected override void Seed(SmartHomeDataContext context)
        {
            //context.Customers.Add(new Customer() { CustomerName = "Test Customer" });

            // context.TestTables.AddOrUpdate(p => p.Name, new TestTable { Name = "test" });

            //context.WebPagesRoles.AddOrUpdate(p => p.RoleName, new WebPagesRole { RoleName = "Admin" });

            //context.UserInfos.AddOrUpdate(p => p.FirstName,
            //    new UserInfo { UserName = "a", Email = "admin@ymail.com", FirstName = "Admin", LastName = "User", IsSMSRecipient = false, IsEmailRecipient = false, DateOfBirth = DateTime.Now, Password = "123", IsActive = true }     );
            // user1.WebPagesRoles.Add(role1);

            //context.SaveChanges();
            //base.Seed(context);


            #region MyRegion


            //WebPagesRole role1 = new WebPagesRole { RoleName = "Admin" }; DbMigrationsConfiguration
            // WebPagesRole role2 = new WebPagesRole { RoleName = "User" };
            //UserInfo user1 = new UserInfo { UserName = "a", Email = "admin@ymail.com", FirstName = "Admin", LastName = "User", IsSMSRecipient = false, IsEmailRecipient = false, DateOfBirth = DateTime.Now, Password = "123", IsActive = true };



            context.TestTables.AddOrUpdate(p => p.Name, new TestTable { Name = "test", ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added });

            //context.WebPagesRoles.AddOrUpdate(p => p.RoleName, new WebPagesRole { RoleName = "Admin" });

            //context.UserInfos.AddOrUpdate(p => p.FirstName,
            //    new UserInfo { UserName = "a", Email = "admin@ymail.com", FirstName = "Admin", LastName = "User", IsSMSRecipient = false, IsEmailRecipient = false, DateOfBirth = DateTime.Now, Password = "123", IsActive = true }     );
            // user1.WebPagesRoles.Add(role1);

            context.SaveChanges();

            #endregion
        }
    }
}
