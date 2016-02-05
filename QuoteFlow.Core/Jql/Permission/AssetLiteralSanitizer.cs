using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Permission
{
    /// <summary>
    /// Sanitize the asset SKUs or IDs stored in <see cref="QueryLiteral"/>s.
    /// The strategy is to sanitize only those assets which both exist and the user does 
    /// not have permission to browse. The sanitized form of the operand replaces the SKU
    /// representation with the ID representation.
    /// </summary>
    public class AssetLiteralSanitizer : ILiteralSanitizer
    {
        private readonly IJqlAssetSupport _jqlAssetSupport;
        private readonly User _user;

        public AssetLiteralSanitizer(IJqlAssetSupport jqlAssetSupport, User user)
        {
            if (jqlAssetSupport == null) throw new ArgumentNullException(nameof(jqlAssetSupport));
            if (user == null) throw new ArgumentNullException(nameof(user));

            _jqlAssetSupport = jqlAssetSupport;
            _user = user;
        }

        /// <summary>
        /// Asset SKUs are not guaranteed to be 1-1, so this method might actually return more
        /// <see cref="QueryLiteral"/>s than what with started with. Therefore, callers of this
        /// method need to be aware that the type of <see cref="IOperand"/> to use might need
        /// to change.
        /// </summary>
        /// <param name="literals">The literals to sanitize; must not be null.</param>
        /// <returns>The result object containing the modification status and the resulting literals.</returns>
        public LiteralSanitizerResult SanitiseLiterals(IEnumerable<QueryLiteral> literals)
        {
            if (literals == null) throw new ArgumentNullException(nameof(literals));

            bool isModified = false;

            // keep a set of literals: if we're going to sanitize the literal, we may as well optimize
            // and remove duplicates.
            var resultantLiterals = new HashSet<QueryLiteral>();
            foreach (var literal in literals)
            {
                var assets = GetAssets(literal);
                var badIds = new List<int>(assets.Count);
//                foreach (var asset in assets)
//                {
//                    // todo Permission check
//                }

                // if every asset had no permission, then we need to sanitize the literal to be all the IDs resolved.
                // if there were no resolved assets, then the literals didn't exist anyway, so don't need to sanitize
                if ((assets.Count == badIds.Count) && assets.Any())
                {
                    foreach (var badId in badIds)
                    {
                        resultantLiterals.Add(new QueryLiteral(literal.SourceOperand, badId));
                    }
                    isModified = true;
                }
                else
                {
                    resultantLiterals.Add(literal);
                }
            }

            return new LiteralSanitizerResult(isModified, new List<QueryLiteral>(resultantLiterals));
        }

        private List<IAsset> GetAssets(QueryLiteral literal)
        {
            // todo asset lookup by SKU
//            if (literal.StringValue != null)
//            {
//                var asset = _jqlAssetSupport.GetAsset(literal.StringValue);
//                if (asset != null)
//                {
//                    return new List<IAsset>() { asset };
//                }
//            }

            if (literal.IntValue != null)
            {
                var asset = _jqlAssetSupport.GetAsset(literal.IntValue.Value);
                if (asset != null)
                {
                    return new List<IAsset> { asset };
                }
            }

            return new List<IAsset>();
        }
    }
}