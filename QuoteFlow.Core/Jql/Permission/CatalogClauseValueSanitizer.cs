using System;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Jql.Permission
{
    /// <summary>
    /// Sanitises clauses which have Catalog names or ids as their values.
    /// </summary>
    public class CatalogClauseValueSanitizer : IClauseSanitizer
    {
        public ICatalogService CatalogService { get; protected set; }
        public IJqlOperandResolver JqlOperandResolver { get; protected set; }
        public INameResolver<Catalog> CatalogResolver { get; protected set; }

        public CatalogClauseValueSanitizer(ICatalogService catalogService, IJqlOperandResolver jqlOperandResolver, INameResolver<Catalog> catalogResolver)
		{
		    if (catalogService == null)
		    {
		        throw new ArgumentNullException("catalogService");
		    }

		    if (jqlOperandResolver == null)
		    {
		        throw new ArgumentNullException("jqlOperandResolver");
		    }

		    if (catalogResolver == null)
		    {
		        throw new ArgumentNullException("catalogResolver");
		    }

            CatalogService = catalogService;
            JqlOperandResolver = jqlOperandResolver;
            CatalogResolver = catalogResolver;
		}

        /// <summary>
        /// Important note: we are making a big assumption here that the <see cref="ProjectOperandSanitisingVisitor"/>
        /// will always return the same kind of operand back after sanitising. This is because project literals can never
        /// expand to more than one index value for a named literal. Therefore, the multiplicity of the operand does not
        /// change after sanitising. Because of this, we blindly reuse the original operator from the input clause.
        /// 
        /// If this assumption ever changes, we will need to revisit this code.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clause"> the clause to sanitise </param>
        /// <returns> the sanitised clause; never null. </returns>
        public IClause Sanitize(User user, ITerminalClause clause)
        {
            ProjectOperandSanitisingVisitor visitor = CreateOperandVisitor(user, clause);
            var originalOperand = clause.Operand;
            var sanitisedOperand = originalOperand.Accept(visitor);

            if (originalOperand.Equals(sanitisedOperand))
            {
                return clause;
            }
            
            return new TerminalClause(clause.Name, clause.Operator, sanitisedOperand);
        }

		internal virtual ProjectOperandSanitisingVisitor CreateOperandVisitor(User user, ITerminalClause terminalClause)
		{
			return new ProjectOperandSanitisingVisitor(JqlOperandResolver, CatalogResolver, CatalogService, user, terminalClause);
		}

		internal class ProjectOperandSanitisingVisitor : AbstractLiteralSanitizingVisitor
		{
			internal readonly INameResolver<Catalog> CatalogResolver;
			internal readonly ICatalogService CatalogService;
			internal readonly User User;

			internal ProjectOperandSanitisingVisitor(IJqlOperandResolver jqlOperandResolver, INameResolver<Catalog> catalogResolver, ICatalogService catalogService, User user, ITerminalClause terminalClause) 
                : base(jqlOperandResolver, user, terminalClause)
			{
				CatalogResolver = catalogResolver;
				CatalogService = catalogService;
				User = user;
			}

			protected override ILiteralSanitizer CreateLiteralSanitizer()
			{
				return new CatalogLiteralSanitizer(CatalogResolver, User);
			}
		}
    }
}