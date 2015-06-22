using System;
using System.IO;
using RazorEngine.Templating;

namespace QuoteFlow.Api.Asset.Search.Searchers.Renderer
{
    public class QuoteFlowTemplateManager : ITemplateManager
    {
        private readonly string _baseTemplatePath;

        public QuoteFlowTemplateManager(string baseTemplatePath)
        {
            _baseTemplatePath = baseTemplatePath;
        }

        public ITemplateSource Resolve(ITemplateKey key)
        {
            var template = key.Name;
            var path = Path.Combine(_baseTemplatePath, string.Format("{0}{1}", template, ".cshtml"));
            var content = File.ReadAllText(path);
            return new LoadedTemplateSource(content, path);
        }

        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new NameOnlyTemplateKey(name, resolveType, context);
        }

        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotImplementedException("Dynamic types are not supported");
        }
    }
}