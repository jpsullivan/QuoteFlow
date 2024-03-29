﻿using System.Web.Mvc;
using Hangfire;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Areas.Admin.ViewModels.Indexing;
using QuoteFlow.Infrastructure.Attributes;

namespace QuoteFlow.Areas.Admin.Controllers
{
    public class IndexingController : AdminControllerBase
    {
        #region DI

        public IIndexLifecycleManager IndexLifecycleManager { get; protected set; }
        public IIndexPathManager IndexPathManager { get; protected set; }

        public IndexingController(IIndexLifecycleManager indexLifecycleManager, IIndexPathManager indexPathManager)
        {
            IndexLifecycleManager = indexLifecycleManager;
            IndexPathManager = indexPathManager;
        }

        #endregion

        [QuoteFlowRoute("admin/indexing", Name = "Admin-Indexing")]
        public ActionResult Index()
        {
            var indexLocation = IndexPathManager.IndexRootPath;
            var model = new IndexingViewModel(indexLocation);
            return View(model);
        }

        [QuoteFlowRoute("admin/reindex", HttpVerbs.Post, Name = "Admin-DoReindex")]
        public ActionResult DoReindex(ReindexViewModel model)
        {
            if (model.IndexingStrategy == "background")
            {
                //BackgroundJob.Enqueue(() => IndexLifecycleManager.ReIndexAllAssetsInBackground(true));
                IndexLifecycleManager.ReIndexAllAssetsInBackground(false);
            }
            else
            {
                IndexLifecycleManager.ReIndexAll();
            }

            return View();
        }
    }
}