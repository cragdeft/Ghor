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

        public IDbSet<Models.Version> Versions { get; set; }
        public IDbSet<VersionDetail> VersionDetails { get; set; }

        public IDbSet<Device> Devices { get; set; }
        public IDbSet<DeviceStatus> DeviceStatuses { get; set; }
        public IDbSet<Channel> Channels { get; set; }

        public IDbSet<Home> Homes { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        // public IDbSet<HomeVersion> HomeVersions { get; set; }

        public IDbSet<Room> Rooms { get; set; }
        public IDbSet<UserInfo> UserInfos { get; set; }
        public IDbSet<UserType> UserTypes { get; set; }
        public IDbSet<UserStatus> UserStatuses { get; set; }
        public IDbSet<WebPagesRole> WebPagesRoles { get; set; }

        public IDbSet<CommandJson> CommandJsons { get; set; }

        public IDbSet<RgbwStatus> RgbwStatuses { get; set; }




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>()
              .HasMany(u => u.WebPagesRoles)
              .WithMany(r => r.UserInfos)
              .Map(m =>
              {
                  m.ToTable("UserRoles");
                  m.MapLeftKey("UserInfoId");
                  m.MapRightKey("RoleId");
              });


            //modelBuilder.Entity<UserInfo>()
            // .HasMany(u => u.Rooms)
            // .WithMany(r => r.UserInfos)
            // .Map(m =>
            // {
            //     m.ToTable("UserRooms");
            //     m.MapLeftKey("RoomId");
            //     m.MapRightKey("UserInfoId");
            // });

            //modelBuilder.Entity<UserInfo>()
            //.HasMany(u => u.Homes)
            //.WithMany(r => r.UserInfos)
            //.Map(m =>
            //{
            //    m.ToTable("UserInfoHomes");               
            //    m.MapLeftKey("HomeId");
            //    m.MapRightKey("UserInfoId");
            //    m.MapRightKey("UserInfoId");
            //});

            //modelBuilder.Entity<UserInfo>().HasKey(e => new { e.UserId, e.BadgeId });

            base.OnModelCreating(modelBuilder);

            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SmartHomeDataContext>());
        }

        public void ExecuteCommand(string command, params object[] parameters)
        {
            base.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
