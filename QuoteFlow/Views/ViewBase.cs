using System;
using System.Web.Mvc;
using QuoteFlow.Configuration;
using QuoteFlow.Controllers;
using QuoteFlow.Models;

namespace QuoteFlow.Views
{
    public abstract class ViewBase : WebViewPage
    {
        private readonly Lazy<QuoteFlowContext> _context;

        public QuoteFlowContext QuoteFlowContext
        {
            get { return _context.Value; }
        }

        public ConfigurationService Config
        {
            get { return QuoteFlowContext.Config; }
        }

        public User CurrentUser
        {
            get { return QuoteFlowContext.CurrentUser; }
        }

        protected ViewBase()
        {
            _context = new Lazy<QuoteFlowContext>(GetContextThunk(this));
        }

        internal static Func<QuoteFlowContext> GetContextThunk(WebViewPage self)
        {
            return () =>
            {
                var ctrl = self.ViewContext.Controller as AppController;
                if (ctrl == null)
                {
                    throw new InvalidOperationException("Viewbase should only be used on views for actions on AppControllers");
                }
                return ctrl.QuoteFlowContext;
            };
        }
    }

    public abstract class ViewBase<T> : WebViewPage<T>
    {
        private Lazy<QuoteFlowContext> _context;

        public QuoteFlowContext QuoteFlowContext
        {
            get { return _context.Value; }
        }

        public ConfigurationService Config
        {
            get { return QuoteFlowContext.Config; }
        }

        public User CurrentUser
        {
            get { return QuoteFlowContext.CurrentUser; }
        }

        public Organization CurrentOrganization
        {
            get { return QuoteFlowContext.CurrentOrganization; }
        }

        protected ViewBase()
        {
            _context = new Lazy<QuoteFlowContext>(ViewBase.GetContextThunk(this));
        }
    }
}