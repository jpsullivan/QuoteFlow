﻿using System;

namespace QuoteFlow.Infrastructure.Attributes
{
    /// <summary>
    /// Defines a prefix shared by all the routes defined in this controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RoutePrefixAttribute : Attribute
    {
        /// <summary>
        /// Defines a prefix shared by all the routes defined in this controller.
        /// The url prefix will be the name of the controller without the "Controller" suffix.
        /// </summary>
        private RoutePrefixAttribute() { }

        /// <summary>
        /// Defines a prefix shared by all the routes defined in this controller.
        /// </summary>
        /// <param name="url">The url prefix to apply to the routes</param>
        public RoutePrefixAttribute(string url)
            : this()
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            Url = url;
        }

        /// <summary>
        /// The url prefix to apply to the routes.
        /// </summary>
        public string Url { get; private set; }
    }
}