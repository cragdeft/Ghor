using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Repository.Pattern.DataContext;
using SmartHome.Model.ModelDataContext;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Ef6;
using SmartHome.Service.Interfaces;
using SmartHome.Service;
using Repository.Pattern.Repositories;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using System.Web.Http;

namespace SmartHome.WebAPI.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);

            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here

            container
               .RegisterType<IDataContextAsync, SmartHomeDataContext>(new PerRequestLifetimeManager())
               .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerRequestLifetimeManager())
               .RegisterType<IVersionService, VersionService>()
               .RegisterType<IRepositoryAsync<Model.Models.Version>, Repository<Model.Models.Version>>()
               .RegisterType<IUserInfoService, UserInfoService>()
               .RegisterType<IRepositoryAsync<Model.Models.UserInfo>, Repository<Model.Models.UserInfo>>();

            #region Dependency resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container); 
            #endregion

        }
    }
}
