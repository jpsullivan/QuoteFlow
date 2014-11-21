using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
{
    /// <summary>
    /// The default field flag operand registry that maps field name and
    /// navigator flag value pairs to their appropiate operand.
    /// </summary>
    public class FieldFlagOperandRegistry : IFieldFlagOperandRegistry
    {
        private static readonly IDictionary<string, IDictionary<string, IOperand>> REGISTRY;

        static FieldFlagOperandRegistry()
		{
            IDictionary<string, IOperand> issueTypeFlags = new Dictionary<string, IOperand>();
//            issueTypeFlags.Add(QuoteFlow.Constants.ALL_STANDARD_ASSET_TYPES, new FunctionOperand(AllStandardIssueTypesFunction.FUNCTION_STANDARD_ISSUE_TYPES));

            IDictionary<string, IOperand> versionFlags = new Dictionary<string, IOperand>();
//            versionFlags.Add(VersionManager.ALL_RELEASED_VERSIONS, new FunctionOperand(AllReleasedVersionsFunction.FUNCTION_RELEASED_VERSIONS));
//            versionFlags.Add(VersionManager.ALL_UNRELEASED_VERSIONS, new FunctionOperand(AllUnreleasedVersionsFunction.FUNCTION_UNRELEASED_VERSIONS));
//            versionFlags.Add(VersionManager.NO_VERSIONS, EmptyOperand.EMPTY);
                

            IDictionary<string, IOperand> componentFlags = new Dictionary<string, IOperand>();
//            componentFlags.Add(SystemSearchConstants.ForComponent().EmptySelectFlag, EmptyOperand.EMPTY);

            IDictionary<string, IOperand> resolutionFlags = new Dictionary<string, IOperand>();
//            resolutionFlags.Add(ResolutionSystemField.UNRESOLVED_VALUE.ToString(), new SingleValueOperand(ResolutionSystemField.UNRESOLVED_OPERAND));

			IDictionary<string, IDictionary<string, IOperand>> tmpRegistry = new Dictionary<string, IDictionary<string, IOperand>>();

			//tmpRegistry[SystemSearchConstants.forIssueType().JqlClauseNames.PrimaryName] = issueTypeFlags;
			//tmpRegistry[SystemSearchConstants.forComponent().JqlClauseNames.PrimaryName] = componentFlags;
			//tmpRegistry[SystemSearchConstants.forAffectedVersion().JqlClauseNames.PrimaryName] = versionFlags;
			//tmpRegistry[SystemSearchConstants.forFixForVersion().JqlClauseNames.PrimaryName] = versionFlags;
			//tmpRegistry[SystemSearchConstants.forResolution().JqlClauseNames.PrimaryName] = resolutionFlags;

			REGISTRY = new Dictionary<string, IDictionary<string, IOperand>>(tmpRegistry);
		}


        public IOperand GetOperandForFlag(string fieldName, string flagValue)
        {
            var fieldFlags = REGISTRY[fieldName];
            return fieldFlags != null ? fieldFlags[flagValue] : null;
        }

        public ISet<string> GetFlagForOperand(string fieldName, IOperand operand)
        {
            var fieldFlags = REGISTRY[fieldName];
            if (fieldFlags == null) return null;

            ISet<string> flags = new HashSet<string>();
            foreach (var entry in fieldFlags)
            {
                if (entry.Value.Equals(operand))
                {
                    flags.Add(entry.Key);
                }
            }

            return flags.Any() ? flags : null;
        }
    }
}