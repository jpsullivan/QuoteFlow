using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Models.Search.Jql.Values;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
    /// <summary>
    /// Class to create the <seealso cref="SearchHandler"/> for the <see cref="ManufacturerSystemField"/>.
    /// 
    /// @since v4.0
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