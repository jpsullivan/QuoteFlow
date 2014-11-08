﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
//    public sealed class CatalogSearchHandlerFactory : SimpleSearchHandlerFactory
//    {
//        public CatalogSearchHandlerFactory(
//            CatalogClauseQueryFactory clauseFactory, 
//            CatalogValidator caluseValidator, 
//            FieldClausePermissionChecker.Factory clausePermissionFactory, 
//            PermissionManager permissionManager, 
//            IJqlOperandResolver jqlOperandResolver, 
//            ProjectResolver projectResolver, 
//            MultiClauseDecoratorContextFactory.Factory multiFactory)
//            : base(componentFactory, SystemSearchConstants.ForCatalog(), typeof(ProjectSearcher), clauseFactory, caluseValidator, clausePermissionFactory, multiFactory.create(new ProjectClauseContextFactory(jqlOperandResolver, projectResolver, permissionManager)), new ProjectClauseValuesGenerator(permissionManager), new ProjectClauseValueSanitiser(permissionManager, jqlOperandResolver, projectResolver))
//        {
//        }
//    }
}