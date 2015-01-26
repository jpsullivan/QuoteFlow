using System.Collections.Generic;
using System.Text;
using QuoteFlow.Api.Infrastructure.Extensions;

namespace QuoteFlow.Api.Infrastructure.Helpers
{
    public class PageNumber
    {
        public string HRef { get; set; }
        public string CssClass { get; set; }
        public string DivId { get; set; }
        public int PageCount { get; set; }
        public int PageCurrent { get; set; }

        private const int Cellcount = 6;
        private const string Prev = "Prev";
        private const string Next = "Next";
        const string PagerDots = "&hellip;";

        public PageNumber(string href, int pageCount, int pageCurrent, string cssClass)
        {
            HRef = href;
            CssClass = cssClass;
            PageCount = pageCount;
            PageCurrent = pageCurrent;
        }
        public PageNumber(string href, int pageCount, int pageCurrent, string cssClass, string divId)
        {
            HRef = href;
            CssClass = cssClass;
            DivId = divId;
            PageCount = pageCount;
            PageCurrent = pageCurrent;
        }

        public override string ToString()
        {
            if (PageCount <= 1) return "";

            var curPage = PageCurrent + 1;
            var pages = new List<string>();

            // handle simplest case first: only a few pages!
            if (PageCount <= Cellcount)
            {
                for (int i = 1; i <= PageCount; i++)
                    pages.Add(i.ToString());
            }
            else
            {
                if (curPage < Cellcount - 1)
                {
                    // we're near the start
                    for (int i = 1; i < Cellcount; i++)
                        pages.Add(i.ToString());
                    pages.Add(PagerDots);
                    pages.Add(PageCount.ToString());
                }
                else if (curPage > PageCount - Cellcount + 2)
                {
                    // we're near the end
                    pages.Add("1");
                    pages.Add(PagerDots);
                    for (int i = PageCount - Cellcount + 2; i <= PageCount; i++)
                        pages.Add(i.ToString());
                }
                else
                {
                    // we're in the middle, somewhere
                    pages.Add("1");
                    pages.Add(PagerDots);
                    const int range = Cellcount - 4;
                    for (int i = curPage - range; i <= curPage + range; i++)
                        pages.Add(i.ToString());
                    pages.Add(PagerDots);
                    pages.Add(PageCount.ToString());
                }
            }

            var sb = new StringBuilder(1024);

            if (CssClass.HasValue() || DivId.HasValue())
            {
                sb.Append("<ol ");
                if (DivId.HasValue())
                {
                    sb.Append("id=\"");
                    sb.Append(DivId);
                    sb.Append("\" ");
                }
                if (CssClass.HasValue())
                {
                    sb.Append("class=\"");
                    sb.Append(CssClass);
                    sb.Append("\" ");
                }
                sb.AppendLine(">");
            }

            if (curPage > 1)
                WriteCell(sb, Prev, "page-numbers");
            foreach (string page in pages)
                WriteCell(sb, page, "page-numbers");
            if (curPage < PageCount)
                WriteCell(sb, Next, "page-numbers");

            if (CssClass.HasValue() || DivId.HasValue())
                sb.AppendLine("</ol>");

            return sb.ToString();
        }

        private void WriteCell(StringBuilder sb, string pageText, string cssClass)
        {
            string linktext = pageText;
            string rel = null;
            bool createLink = true;

            switch (pageText) {
                case PagerDots:
                    cssClass += " dots";
                    createLink = false;
                    break;
                case Prev:
                    cssClass += " aui-nav-previous";
                    rel = "prev";
                    pageText = PageCurrent.ToString();
                    break;
                case Next:
                    cssClass += " aui-nav-next";
                    rel = "next";
                    pageText = (PageCurrent + 2).ToString();
                    break;
                default:
                    // currently selected page
                    if (pageText == (PageCurrent + 1).ToString())
                    {
                        cssClass += " aui-nav-selected";
                        createLink = false;
                    }
                    break;
            }

            sb.Append(@"<li class=""");
            sb.Append(cssClass);
            sb.Append(@""">");

            if (createLink) {
                sb.Append(@"<a href=""");
                sb.Append(HRef.Replace("page=-1", "page=" + pageText));
                sb.Append(@""" title=""Go to page ");
                sb.Append(pageText);
                sb.Append(@"""");
                if (rel != null)
                    sb.Append(@" rel=""" + rel + @"""");
                sb.Append(">");
            }
            
            sb.Append(linktext);

            if (createLink) {
                sb.Append("</a>");
            }
            
            sb.Append("</li>");
            sb.AppendLine();
        }
    }
}