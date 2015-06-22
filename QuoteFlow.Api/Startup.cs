using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace QuoteFlow.Api
{
    public static class Startup
    {
        public static void ConfigureRazorEngine(string baseTemplatePath)
        {
            var config = new TemplateServiceConfiguration();
            config.Debug = true;
            config.TemplateManager = new QuoteFlowTemplateManager(baseTemplatePath);
            Engine.Razor = RazorEngineService.Create(config);
        }
    }
}