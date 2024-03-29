﻿using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Resolver;
using QuoteFlow.Core.Jql.Validator;
using QuoteFlow.Core.Jql.Values;

namespace QuoteFlow.Core.Asset.Search.Handlers
{
    /// <summary>
    /// Class to create the <see cref="SearchHandler"/> for the <see cref="ManufacturerSystemField"/>.
    /// </summary>
    public sealed class ManufacturerSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public ManufacturerSearchHandlerFactory(ManufacturerClauseQueryFactory clauseQueryFactory, 
            ManufacturerValidator clauseValidator, IManufacturerService manufacturerService, FieldClausePermissionChecker.IFactory clausePermissionFactory, 
            ManufacturerResolver issueTypeResolver, IJqlOperandResolver jqlOperandResolver, 
            MultiClauseDecoratorContextFactory.Factory multiFactory)
            : base(SystemSearchConstants.ForManufacturer(), typeof(ManufacturerSearcher), 
            clauseQueryFactory, clauseValidator, clausePermissionFactory, 
            multiFactory.Create(new ManufacturerClauseContextFactory(issueTypeResolver, jqlOperandResolver)), new ManufacturerClauseValuesGenerator(manufacturerService))
        {
        }
    }
}