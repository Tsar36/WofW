using System;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using WorldofWords.Utils;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Validation;
using WorldOfWords.Validation.Classes;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldofWords.Hubs;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;

namespace WorldofWords.App_Start
{
    public class SignalRUnityDependencyResolver : DefaultDependencyResolver
    {
        private IUnityContainer _container;

        public SignalRUnityDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            if (_container.IsRegistered(serviceType)) return _container.Resolve(serviceType);
            else return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            if (_container.IsRegistered(serviceType)) return _container.ResolveAll(serviceType);
            else return base.GetServices(serviceType);
        }

    }
}