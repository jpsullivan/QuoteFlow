using System.Collections.Generic;
using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using BundleTransformer.Core.Translators;
using BundleTransformer.TypeScript.Translators;
using BundleTransformer.UglifyJs.Minifiers;
using BundleTransformer.Yui.Minifiers;

namespace QuoteFlow
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssTransformer = new CssTransformer(new YuiCssMinifier());
            var translators = new List<ITranslator> {new TypeScriptTranslator()};
            var jsTransformer = new JsTransformer(new UglifyJsMinifier(), translators);
            var nullOrderer = new NullOrderer();

            // Lib CSS
            Bundle libCss = new Bundle("~/bundles/css_lib")
                .IncludeDirectory("~/Content/css/lib/aui", "*.css")
                .Include("~/Content/themes/minified/jquery-ui.min.css");
            //libCss.Transforms.Add(cssTransformer);
            bundles.Add(libCss);

            // App LESS
            Bundle appCss = new Bundle("~/bundles/css_app").Include("~/Content/less/bootstrap.less", new CssRewriteUrlTransform());
            appCss.Transforms.Add(cssTransformer);
            bundles.Add(appCss);

            // Lib JS
            Bundle libJs = new Bundle("~/bundles/js_lib").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Content/js/lib/json2.min.js",
                "~/Content/js/lib/handlebars.min.js",
                "~/Content/js/lib/underscore.min.js",
                "~/Content/js/lib/backbone.min.js",
                "~/Content/js/lib/aui/aui-all.js",
                "~/Content/js/lib/moment.min.js");
            libJs.Transforms.Add(jsTransformer);
            libJs.Orderer = nullOrderer;
            bundles.Add(libJs);

            // App JS
            Bundle appJs = new Bundle("~/bundles/js_app").Include(
                "~/Content/js/app/quoteflow.ts",
                "~/Content/js/app/views.js");
            appJs.IncludeDirectory("~/Content/js/app/components", "*.js", true);
            appJs.IncludeDirectory("~/Content/js/app/models", "*.js", true);
            appJs.IncludeDirectory("~/Content/js/app/collections", "*.js", true);
            appJs.IncludeDirectory("~/Content/js/app/ui", "*.js", true);
            appJs.IncludeDirectory("~/Content/js/app/helpers", "*.js", true);
            bundles.Add(appJs);

#if (DEBUG)
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}