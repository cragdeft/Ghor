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
            //Database.SetInitializer<SmartHomeDataContext>(null);
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SmartHomeDataContext>());
        }
        public SmartHomeDataContext()
            : base("SmartHome")
        {

        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public IDbSet<Models.Version> Versions { get; set; }
        public IDbSet<VersionDetail> VersionDetails { get; set; }
        public IDbSet<SmartDevice> SmartDevices { get; set; }        
        public IDbSet<DeviceStatus> DeviceStatuses { get; set; }
        public IDbSet<Home> Homes { get; set; }
        public IDbSet<RouterInfo> RouterInfos { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<Room> Rooms { get; set; }
        public IDbSet<UserInfo> UserInfos { get; set; }
        public IDbSet<UserType> UserTypes { get; set; }
        public IDbSet<UserStatus> UserStatuses { get; set; }
        public IDbSet<WebPagesRole> WebPagesRoles { get; set; }
        public IDbSet<CommandJson> CommandJsons { get; set; }
        public IDbSet<NextAssociatedDevice> NextAssociatedDevices { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           modelBuilder.Entity<SmartDevice>()
          .Map<SmartSwitch>(m => m.Requires("Discriminator").HasValue("SmartSwitch"))
          .Map<SmartRouter>(m => m.Requires("Discriminator").HasValue("SmartRouter"))
          .Map<SmartRainbow>(m => m.Requires("Discriminator").HasValue("SmartRainbow"));

            //modelBuilder.Entity<Room>()
            // .HasMany(u => u.SmartDevices)
            // .WithOptional()
            // .WillCascadeOnDelete(true);

            //modelBuilder.Entity<SmartDevice>()
            // .HasMany(u => u.DeviceStatus)
            // .WithOptional()
            // .WillCascadeOnDelete(true);

            //modelBuilder.Entity<SmartRainbow>()
            // .HasMany(u => u.RgbwStatuses)
            // .WithOptional()
            // .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Channel>()
            // .HasMany(u => u.ChannelStatuses)
            // .WithOptional()
            // .WillCascadeOnDelete(true);

            //modelBuilder.Entity<SmartSwitch>()
            // .HasMany(u => u.Channels)
            // .WithOptional()
            // .WillCascadeOnDelete(true);

            //modelBuilder.Entity<SmartSwitch>()
            // .HasMany(u => u.DeviceStatus)
            // .WithOptional()
            // .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public void ExecuteCommand(string command, params object[] parameters)
        {
            base.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
