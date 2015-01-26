using System;

namespace QuoteFlow.Api.Jql
{
    /// <summary>
    /// Value returned by search renderer.
    /// </summary>
    public class SearchRendererValue
    {
        public string name { get; set; }
        public string jql { get; set; }
        public string viewHtml { get; set; }
        public string editHtml { get; set; }
        public bool validSearcher { get; set; }
        public bool isShown { get; set; }

        public SearchRendererValue()
        {
            name = null;
            jql = null;
            viewHtml = null;
            editHtml = null;
            validSearcher = false;
            isShown = true;
        }

        public SearchRendererValue(string name, string jql, string viewHtml, String editHtml, bool validSearcher, bool isShown)
        {
            this.name = name;
            this.editHtml = editHtml;
            this.jql = jql;
            this.validSearcher = validSearcher;
            this.viewHtml = viewHtml;
            this.isShown = isShown;
        }
    }
}