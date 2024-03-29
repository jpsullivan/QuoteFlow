﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

namespace QuoteFlow.Core.DependencyResolution
{
    public static class Container
    {
        private static readonly Lazy<IKernel> LazyKernel = new Lazy<IKernel>(GetKernel);

        public static IKernel Kernel => LazyKernel.Value;

        private static IKernel GetKernel()
        {
            var kernel = new StandardKernel(GetModules().ToArray());
            // Used for suppressing any IntPtr fatal exceptions
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            return kernel;
        }

        private static IEnumerable<NinjectModule> GetModules()
        {
            yield return new ContainerBindings();
            yield return new ServiceBindings();
            yield return new DiagnosticsNinjectModule();
            yield return new AuthNinjectModule();
        }
    }
}