﻿using System;
using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Operand;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A clause validator that can be used for catalog clause types.
    /// </summary>
    public class CatalogValuesExistValidator : ValuesExistValidator
    {
        private readonly CatalogIndexInfoResolver catalogIndexInfoResolver;
		private readonly ICatalogService catalogService;

        internal CatalogValuesExistValidator(JqlOperandResolver operandResolver, CatalogIndexInfoResolver catalogIndexInfoResolver, ICatalogService catalogService)
            : base(operandResolver)
		{
            if (catalogIndexInfoResolver == null)
            {
                throw new ArgumentNullException(nameof(catalogIndexInfoResolver));
            }

            if (catalogService == null)
            {
                throw new ArgumentNullException(nameof(catalogService));
            }

            this.catalogIndexInfoResolver = catalogIndexInfoResolver;
			this.catalogService = catalogService;
		}

        internal override bool StringValueExists(User searcher, string value)
        {
            var ids = catalogIndexInfoResolver.GetIndexedValues(value);
            return CatalogExists(searcher, ids);
        }

        internal override bool IntValueExists(User searcher, int? value)
		{
			var ids = catalogIndexInfoResolver.GetIndexedValues(value);
			return CatalogExists(searcher, ids);
		}

		internal virtual bool CatalogExists(User searcher, IEnumerable<string> ids)
		{
			foreach (string sid in ids)
			{
                int? id = ConvertToInt(sid);
				if (id != null)
				{
					Catalog project = catalogService.GetCatalog((int) id);
					if (project != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		private int? ConvertToInt(string str)
		{
			try
			{
				return Convert.ToInt32(str);
			}
			catch (Exception e)
			{
				return null;
			}
		}
    }
}