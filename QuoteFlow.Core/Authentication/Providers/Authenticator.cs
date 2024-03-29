﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Owin;
using QuoteFlow.Api.Authentication.Providers;
using QuoteFlow.Core.Configuration;

namespace QuoteFlow.Core.Authentication.Providers
{
    public abstract class Authenticator
    {
        private static readonly Regex NameShortener = new Regex(@"^(?<shortname>[A-Za-z0-9_]*)Authenticator$");
        private const string AuthPrefix = "Auth.";

        public AuthenticatorConfiguration BaseConfig { get; private set; }

        public virtual string Name
        {
            get { return GetName(GetType()); }
        }

        protected Authenticator()
        {
            BaseConfig = CreateConfigObject();
        }

        public void Startup(ConfigurationService config, IAppBuilder app)
        {
            Configure(config);

            if (BaseConfig.Enabled)
            {
                AttachToOwinApp(config, app);
            }
        }

        protected virtual void AttachToOwinApp(ConfigurationService config, IAppBuilder app) { }

        // Configuration Logic
        public virtual void Configure(ConfigurationService config)
        {
            BaseConfig = config.ResolveConfigObject(BaseConfig, AuthPrefix + Name + ".");
        }

        public static string GetName(Type authenticator)
        {
            var name = authenticator.Name;
            var match = NameShortener.Match(name);
            if (match.Success)
            {
                name = match.Groups["shortname"].Value;
            }
            return name;
        }

        public static IEnumerable<Authenticator> GetAllAvailable()
        {
            // Find all available auth providers
            return GetAllAvailable(typeof(Authenticator)
                .Assembly
                .GetExportedTypes());
        }

        public static IEnumerable<Authenticator> GetAllAvailable(IEnumerable<Type> typesToSearch)
        {
            // Find all available auth providers
            var configTypes =
                typesToSearch
                .Where(t => !t.IsAbstract && typeof(Authenticator).IsAssignableFrom(t))
                .ToList();
            var providers = configTypes
                .Select(t => (Authenticator)Activator.CreateInstance(t))
                .ToList();
            return providers;
        }

        protected internal virtual AuthenticatorConfiguration CreateConfigObject()
        {
            return new AuthenticatorConfiguration();
        }

        public virtual AuthenticatorUI GetUI()
        {
            return null;
        }

        public virtual ActionResult Challenge(string redirectUrl)
        {
            return new HttpUnauthorizedResult();
        }
    }

    public abstract class Authenticator<TConfig> : Authenticator
        where TConfig : AuthenticatorConfiguration, new()
    {
        public TConfig Config { get; private set; }

        protected internal override AuthenticatorConfiguration CreateConfigObject()
        {
            Config = new TConfig();
            return Config;
        }
    }
}