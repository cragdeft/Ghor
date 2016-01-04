using Repository.Pattern.Ef6;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.ModelDataContext
{
    public partial class SmartHomeDataContext : DataContext, ISmartHomeDataContext
    {
        static SmartHomeDataContext()
        {
            Database.SetInitializer<SmartHomeDataContext>(null);
        }
        public SmartHomeDataContext()
            : base("SmartHome")
        {
        }

        

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public IDbSet<ServerResponce> ServerResponces { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //public void ExecuteCommand(string command, params object[] parameters)
        //{
        //    base.Database.ExecuteSqlCommand(command, parameters);
        //}
    }
}
