using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// Clause validator for the "Asset ID" clause.
    /// </summary>
    public class AssetIdValidator : IClauseValidator
    {
        private const int BatchMaxSize = 1000;

        private readonly IJqlOperandResolver _operandResolver;
        private readonly SupportedOperatorsValidator _supportedOperatorsValidator;
        private readonly IJqlAssetIdSupport _assetIdSupport;
        private readonly IJqlAssetSupport _assetSupport;

        public AssetIdValidator(IJqlOperandResolver operandResolver,
            SupportedOperatorsValidator supportedOperatorsValidator, IJqlAssetIdSupport assetIdSupport,
            IJqlAssetSupport assetSupport)
        {
            if (operandResolver == null) throw new ArgumentNullException(nameof(operandResolver));
            if (supportedOperatorsValidator == null)
                throw new ArgumentNullException(nameof(supportedOperatorsValidator));
            if (assetIdSupport == null) throw new ArgumentNullException(nameof(assetIdSupport));
            if (assetSupport == null) throw new ArgumentNullException(nameof(assetSupport));

            _operandResolver = operandResolver;
            _supportedOperatorsValidator = supportedOperatorsValidator;
            _assetIdSupport = assetIdSupport;
            _assetSupport = assetSupport;
        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            if (terminalClause == null) throw new ArgumentNullException(nameof(terminalClause));

            var messages = _supportedOperatorsValidator.Validate(searcher, terminalClause);
            if (!messages.HasAnyErrors())
            {
                var operand = terminalClause.Operand;

                // This should not return null since the outside validation makes sure the operand
                // is valid before calling this method
                var values = _operandResolver.GetValues(searcher, operand, terminalClause).ToList();

                int batches = values.Count/BatchMaxSize + 1;
                for (int batchIndex = 0; batchIndex < batches; batchIndex++)
                {
                    var valuesbatch = values.GetRange(batchIndex*BatchMaxSize,
                        Math.Min((batchIndex + 1)*BatchMaxSize, values.Count));
                    ValidateBatch(searcher, terminalClause, valuesbatch, messages);
                }
            }

            return messages;
        }

        private void ValidateBatch(User searcher, ITerminalClause terminalClause, IEnumerable<QueryLiteral> values,
            IMessageSet messages)
        {
            var operand = terminalClause.Operand;
            var numericLiterals = new HashSet<int>();
            var stringLiterals = new HashSet<string>();
            foreach (QueryLiteral value in values)
            {
                if (!value.IsEmpty)
                {
                    if (value.IntValue != null)
                    {
                        numericLiterals.Add((int) value.IntValue);
                    }
                    else if (value.StringValue != null)
                    {
                        stringLiterals.Add(value.StringValue);
                    }
                    else
                    {
                        Console.WriteLine("Unknown QueryLiteral: " + value);
                    }
                }
                else
                {
                    ValidateEmptyOperand(messages, searcher, terminalClause, value.SourceOperand);
                }
            }
            if (numericLiterals.Any() || stringLiterals.Any())
            {
                if (numericLiterals.Any())
                {
                    ValidateAssetIdsBatch(messages, numericLiterals, searcher, terminalClause, operand);
                }
                if (stringLiterals.Any())
                {
                    ValidateAssetSkusBatch(messages, stringLiterals, searcher, terminalClause, operand);
                }
            }
        }

        private IMessageSet ValidateEmptyOperand(IMessageSet messageSet, User searcher, ITerminalClause clause,
            IOperand operand)
        {
            if (!_operandResolver.IsFunctionOperand(operand))
            {
                messageSet.AddErrorMessage("quoteflow.jql.clause.field.does.not.support.empty: " + clause.Name);
            }
            else
            {
                messageSet.AddErrorMessage("quoteflow.jql.clause.field.does.not.support.empty.from.func: " + clause.Name +
                                           ", " + operand.Name);
            }

            return messageSet;
        }

        private void ValidateAssetIdsBatch(IMessageSet messages, ISet<int> issueIds, User searcher,
            ITerminalClause clause, IOperand operand)
        {
            var missingIssues = _assetSupport.GetIdsOfMissingAssets(issueIds);
            foreach (int? missingIssue in missingIssues)
            {
                AddErrorAssetIdNotFound(messages, missingIssue, searcher, clause, operand);
            }
        }

        private void AddErrorAssetIdNotFound(IMessageSet messages, int? issueId, User searcher, ITerminalClause clause,
            IOperand operand)
        {
            if (!_operandResolver.IsFunctionOperand(operand))
            {
                messages.AddErrorMessage("quoteflow.jql.clause.no.value.for.id: " + clause.Name + ", " + issueId);
            }
            else
            {
                messages.AddErrorMessage("quoteflow.jql.clause.no.value.for.name.from.function: " + operand.Name + ", " +
                                         clause.Name);
            }
        }

        private void ValidateAssetSkusBatch(IMessageSet messages, ISet<string> issueKeys, User searcher,
            ITerminalClause clause, IOperand operand)
        {
            var missingIssueKeys = _assetSupport.GetSkusOfMissingAssets(issueKeys);
            foreach (string missingIssueKey in missingIssueKeys)
            {
                AddErrorAssetSkuNotFound(messages, missingIssueKey, searcher, clause, operand);
            }
            var validIssueKeys = new HashSet<string>(issueKeys);
            foreach (var missingIssueKey in missingIssueKeys)
            {
                validIssueKeys.Remove(missingIssueKey);
            }

            if (validIssueKeys.Any())
            {
                //messages.AddMessageSet(movedIssueValidator.validate(ApplicationUsers.from(searcher), validIssueKeys, clause));
            }
        }

        private void AddErrorAssetSkuNotFound(IMessageSet messages, string key, User searcher, ITerminalClause clause,
            IOperand operand)
        {
//            bool validIssueKey = _issueKeySupport.isValidIssueKey(key);
//            if (!operandResolver.isFunctionOperand(operand))
//            {
//                if (validIssueKey)
//                {
//                    messages.addErrorMessage(i18n.getText("jira.jql.clause.issuekey.noissue", key, clause.Name));
//                }
//                else
//                {
//                    messages.addErrorMessage(i18n.getText("jira.jql.clause.issuekey.invalidissuekey", key, clause.Name));
//                }
//            }
//            else
//            {
//                if (validIssueKey)
//                {
//                    messages.addErrorMessage(i18n.getText("jira.jql.clause.issuekey.noissue.from.func", operand.Name, clause.Name));
//                }
//                else
//                {
//                    messages.addErrorMessage(i18n.getText("jira.jql.clause.issuekey.invalidissuekey.from.func", operand.Name, clause.Name));
//                }
//            }
        }
    }
}