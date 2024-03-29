﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using QuoteFlow.Infrastructure.Helpers;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> self, Expression<Func<TModel, TEnum?>> expression, IEnumerable<TEnum> values, string emptyItemText)
            where TEnum : struct // Can't do ": enum" but this is close
        {
            Debug.Assert(typeof(TEnum).IsEnum, "Expected an Enum Type!");

            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var items = new[] { 
                new SelectListItem { Text = emptyItemText, Value = "" },
            }.Concat(
                values.Select(value => new SelectListItem
                {
                    Text = EnumHelpers.GetDescription(value),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                }));

            return self.DropDownListFor(expression, items, new { @class = "select" });
        }

        public static IHtmlString PreFormattedText(this HtmlHelper self, string text)
        {
            return self.Raw(HttpUtility.HtmlEncode(text).Replace("\n", "<br />").Replace("  ", "&nbsp; "));
        }

        public static bool ContainsValidationErrors(this HtmlHelper html, string key)
        {
            var toRemove = html.ViewData.ModelState.Keys
                .Where(k => !String.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var copy = new ModelStateDictionary(html.ViewData.ModelState);
            foreach (var k in toRemove)
            {
                html.ViewData.ModelState.Remove(k);
            }
            return html.ValidationSummary().ToString().Length > 1;
        }

        public static IHtmlString ValidationSummaryFor(this HtmlHelper html, string key)
        {
            var toRemove = html.ViewData.ModelState.Keys
                .Where(k => !String.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var copy = new ModelStateDictionary(html.ViewData.ModelState);
            foreach (var k in toRemove)
            {
                html.ViewData.ModelState.Remove(k);
            }
            var str = html.ValidationSummary();

            // Restore the old model state
            foreach (var k in toRemove)
            {
                html.ViewData.ModelState[k] = copy[k];
            }

            return str;
        }
    }
}