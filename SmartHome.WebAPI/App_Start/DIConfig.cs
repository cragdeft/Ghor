using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.Web.Http.Dependencies;

namespace SmartHome.WebAPI
{

    //public class UnityDependencyResolver : IDependencyResolver
    //{
    //    private readonly IUnityContainer _container;
    //    public UnityDependencyResolver(IUnityContainer container)
    //    {
    //        this._container = container;
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        try
    //        {
    //            return _container.Resolve(serviceType);
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        try
    //        {
    //            return _container.ResolveAll(serviceType);
    //        }
    //        catch
    //        {
    //            return new List<object>();
    //        }
    //    }
    //}


    public class UnityResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }

}