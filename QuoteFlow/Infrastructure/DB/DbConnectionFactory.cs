using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.Configuration;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace QuoteFlow.Infrastructure.DB
{
    public static class DbConnectionFactory
    {
        public static DbConnection CreateConnection(string connectionString)
        {
            DbConnection cnn;

            // using the WebConfigurationManager instead of the ConfigurationService due 
            // to the fact that the ConfigurationService relies on the following code to work
            // for fetching settings that require a database connection.
            var diagnosticsEnabled = Convert.ToBoolean(WebConfigurationManager.AppSettings["DiagnosticsEnabled"]);
            if (diagnosticsEnabled)
            {
                // Client-side diagnostics enabled; Show glimpse
                var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                cnn = factory.CreateConnection();
                cnn.ConnectionString = connectionString;
            }
            else
            {
                // standard miniprofiler profiling enabled
                cnn = new SqlConnection(connectionString);
                if (Current.Profiler != null)
                {
                    cnn = new ProfiledDbConnection(cnn, MiniProfiler.Current);
                }
            }

            return cnn;
        }
    }
}