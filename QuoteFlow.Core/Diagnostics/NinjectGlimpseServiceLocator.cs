﻿using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Framework;
using Ninject;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Diagnostics
{
    public class NinjectGlimpseServiceLocator : IServiceLocator
    {
        public ICollection<T> GetAllInstances<T>() where T : class
        {
            var ninjectResources = Container.Kernel.GetAll<T>().ToList();

            // Glimpse interprets an empty collection to mean: I'm overriding your defaults and telling you NOT to load anythig
            // However, we want an empty collection to indicate to Glimpse that it should use the default. Returning null does that.
            return !ninjectResources.Any() ? null : ninjectResources;
        }

        public T GetInstance<T>() where T : class
        {
            return Container.Kernel.TryGet<T>();
        }
    }
}