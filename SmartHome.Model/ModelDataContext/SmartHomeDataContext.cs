using Repository.Pattern.Ef6;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.ModelDataContext
{
    public partial class SmartHomeDataContext : DataContext, ISmartHomeDataContext
    {
        static SmartHomeDataContext()
        {
#if DEBUG
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SmartHomeDataContext>());
#else
            Database.SetInitializer<SmartHomeDataContext>(null);
#endif
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
        public IDbSet<WebBrokerInfo> WebBrokerInfoes { get; set; }
        
        public IDbSet<MessageLog> MessageLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<SmartDevice>()
          .Map<SmartSwitch>(m => m.Requires("Discriminator").HasValue("SmartSwitch"))
          .Map<SmartRouter>(m => m.Requires("Discriminator").HasValue("SmartRouter"))
          .Map<SmartRainbow>(m => m.Requires("Discriminator").HasValue("SmartRainbow"));

            modelBuilder.Entity<VersionDetail>()
            .HasRequired(t => t.Version)
            .WithMany(c => c.VersionDetails)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<SmartDevice>()
            .HasRequired(t => t.Room)
            .WithMany(c => c.SmartDevices)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<DeviceStatus>()
            .HasRequired(t => t.SmartDevice)
            .WithMany(c => c.DeviceStatus)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<RgbwStatus>()
            .HasRequired(t => t.SmartRainbow)
            .WithMany(c => c.RgbwStatuses)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<Channel>()
            .HasRequired(t => t.SmartSwitch)
            .WithMany(c => c.Channels)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChannelStatus>()
            .HasRequired(t => t.Channel)
            .WithMany(c => c.ChannelStatuses)
            .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public void ExecuteCommand(string command, params object[] parameters)
        {
            base.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
