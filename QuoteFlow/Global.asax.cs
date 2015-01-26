using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using QuoteFlow.Core;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace QuoteFlow
{
    public class MvcApplication : HttpApplication
    {
        public static string AppRevision
        {
            get
            {
                var appRevision = HttpContext.Current.Application["AppRevision"] as string;

                if (!string.IsNullOrEmpty(appRevision)) return appRevision;

                appRevision = Assembly.GetAssembly(typeof(MvcApplication)).GetName().Version.ToString(4);
                HttpContext.Current.Application["AppRevision"] = appRevision;
                return appRevision;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Application["LastError"] = Server.GetLastError();
#if DEBUG
            Exception lastError = Server.GetLastError();
            string sql = null;

            try
            {
                sql = lastError.Data["SQL"] as string;
            }
            catch
            {
                // skip it
            }

            if (sql == null) return;

            var ex =
                new HttpUnhandledException(
                    "An unhandled exception occurred during the execution of the current web request. Please review the stack trace for more information about the error and where it originated in the code.",
                    lastError);

            Server.ClearError();

            var html = ex.GetHtmlErrorMessage();
            const string traceNode = "<b>Stack Trace:</b>";
            html = html.Replace(traceNode, @"<b>Sql:</b><br><br>
    <table width='100%' bgcolor='#ffffccc'>
    <tbody><tr><td><code><pre>" + sql + @"</pre></code></td></tr></tbody>
    </table><br>" + traceNode);

            HttpContext.Current.Response.Write(html);
            HttpContext.Current.Response.StatusCode = 500;
            HttpContext.Current.Response.Status = "Internal Server Error";
            HttpContext.Current.Response.End();
#endif
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Current.DisposeDB();
            Current.DisposeRegisteredConnections();
        }
    }
}