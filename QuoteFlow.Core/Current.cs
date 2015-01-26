using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using System.Web;
using QuoteFlow.Api.Infrastructure.Elmah;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Infrastructure.DB;
using StackExchange.Profiling;

namespace QuoteFlow.Core
{
    /// <summary>
    /// Helper class that provides quick access to common objects used across a single request.
    /// </summary>
    public class Current
    {
        #region IoC

        public Current() { }

        #endregion

        private static DeploymentTier? _tier;
        private const string DisposeConnectionKey = "dispose_connections";

        private static void RegisterConnectionForDisposal(DbConnection connection)
        {
            if (Context == null) return;

            var connections = Context.Items[DisposeConnectionKey] as List<DbConnection>;
            if (connections == null)
            {
                Context.Items[DisposeConnectionKey] = connections = new List<DbConnection>();
            }

            connections.Add(connection);
        }

        public static void DisposeRegisteredConnections()
        {
            var connections = Context.Items[DisposeConnectionKey] as List<DbConnection>;
            if (connections == null) return;

            Context.Items[DisposeConnectionKey] = null;

            foreach (var connection in connections)
            {
                try
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        QuietLog.LogHandledException(new Exception("Connection was not in a closed state."));
                    }

                    connection.Dispose();
                }
                catch
                {
                    /* don't care, nothing we can do */
                }
            }
        }

        /// <summary>
        /// Shortcut to HttpContext.Current.
        /// </summary>
        public static HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        /// <summary>
        /// Shortcut to HttpContext.Current.Request.
        /// </summary>
        public static HttpRequest Request
        {
            get { return Context.Request; }
        }

        /// <summary>
        /// Gets the single data context for this current request.
        /// </summary>
        public static QuoteFlowDatabase DB
        {
            get
            {
                QuoteFlowDatabase result;
                if (Context != null)
                {
                    result = Context.Items["DB"] as QuoteFlowDatabase;
                }
                else
                {
                    // unit tests
                    result = CallContext.GetData("DB") as QuoteFlowDatabase;
                }

                if (result != null)
                    return result;

                var cnn = DbConnectionFactory.CreateConnection(ConfigurationManager.ConnectionStrings["QuoteFlow.SqlServer"].ConnectionString);

                cnn.Open();
                RegisterConnectionForDisposal(cnn);
                result = QuoteFlowDatabase.Init(cnn, 30);

                if (Context != null)
                {
                    Context.Items["DB"] = result;
                }
                else
                {
                    CallContext.SetData("DB", result);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets where this code is running, e.g. Prod, Dev
        /// </summary>
        public static DeploymentTier Tier
        {
            get
            {
                if (!_tier.HasValue)
                {
                    _tier = DeploymentTier.Local;
                }
                //_tier = (DeploymentTier) Enum.Parse(typeof(DeploymentTier), Site.Tier, true);

                return _tier.Value;
            }
        }

        /// <summary>
        /// Allows end of reqeust code to clean up this request's DB.
        /// </summary>
        public static void DisposeDB()
        {
            QuoteFlowDatabase db;
            if (Context != null)
            {
                db = Context.Items["DB"] as QuoteFlowDatabase;
            }
            else
            {
                db = CallContext.GetData("DB") as QuoteFlowDatabase;
            }

            if (db == null)
            {
                return;
            }

            db.Dispose();
            if (Context != null)
            {
                Context.Items["DB"] = null;
            }
            else
            {
                CallContext.SetData("DB", null);
            }
        }

        public static MiniProfiler Profiler
        {
            get { return MiniProfiler.Current; }
        }
    }

    public enum DeploymentTier
    {
        Prod,
        Dev,
        Local
    }
}