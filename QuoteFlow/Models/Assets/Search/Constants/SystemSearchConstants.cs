using System;
using System.Collections.Generic;
using System.Reflection;
using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Models.Search.Jql.Clauses;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Constants
{
    public sealed class SystemSearchConstants
	{
		/// <summary>
		/// The ID of the query searcher.
		/// </summary>
		public const string QUERY_SEARCHER_ID = "text";

		private static readonly SimpleFieldSearchConstants Catalog = new SimpleFieldSearchConstants(DocumentConstants.CatalogId, AssetFieldConstants.Catalog, "cid", AssetFieldConstants.Catalog, AssetFieldConstants.Catalog, OperatorClasses.EqualityOperatorsWithEmpty, QuoteFlowDataTypes.Catalog);

		public static SimpleFieldSearchConstants ForCatalog()
		{
			return Catalog;
		}

		private static readonly SimpleFieldSearchConstants Summary = new SimpleFieldSearchConstants(DocumentConstants.AssetName, AssetFieldConstants.Summary, "summary", AssetFieldConstants.Summary, AssetFieldConstants.Summary, OperatorClasses.TextOperators, QuoteFlowDataTypes.Text);

		public static SimpleFieldSearchConstants ForSummary()
		{
			return Summary;
		}

		private static readonly SimpleFieldSearchConstants Description = new SimpleFieldSearchConstants(DocumentConstants.AssetDesc, AssetFieldConstants.Description, "description", AssetFieldConstants.Description, AssetFieldConstants.Description, OperatorClasses.TextOperators, QuoteFlowDataTypes.Text);

		public static SimpleFieldSearchConstants ForDescription()
		{
			return Description;
		}

		public static CommentsFieldSearchConstants ForComments()
		{
			return CommentsFieldSearchConstants.Instance;
		}

		private static readonly SimpleFieldSearchConstants CreatedDate = new SimpleFieldSearchConstants(DocumentConstants.AssetCreated, new ClauseNames(AssetFieldConstants.Created, "createdDate"), AssetFieldConstants.Created, AssetFieldConstants.Created, AssetFieldConstants.Created, OperatorClasses.EqualityAndRelationalWithEmpty, QuoteFlowDataTypes.Date);

		public static SimpleFieldSearchConstants ForCreatedDate()
		{
			return CreatedDate;
		}

		private static readonly SimpleFieldSearchConstants UpdateDate = new SimpleFieldSearchConstants(DocumentConstants.AssetUpdated, new ClauseNames(AssetFieldConstants.Updated, "updatedDate"), AssetFieldConstants.Updated, AssetFieldConstants.Updated, AssetFieldConstants.Updated, OperatorClasses.EqualityAndRelationalWithEmpty, QuoteFlowDataTypes.Date);

		public static SimpleFieldSearchConstants ForUpdatedDate()
		{
			return UpdateDate;
		}

		private static readonly UserFieldSearchConstantsWithEmpty Creator = new UserFieldSearchConstantsWithEmpty(DocumentConstants.AssetCreator, AssetFieldConstants.Creator, "creator", "creatorSelect", AssetFieldConstants.Creator, DocumentConstants.AssetCreator, AssetFieldConstants.Creator, OperatorClasses.EqualityOperatorsWithEmpty);

		public static UserFieldSearchConstantsWithEmpty ForCreator()
		{
			return Creator;
		}

//		public static SavedFilterSearchConstants ForSavedFilter()
//		{
//			return SavedFilterSearchConstants.Instance;
//		}

		public static AllTextSearchConstants ForAllText()
		{
			return AllTextSearchConstants.Instance;
		}

		public static AssetIdConstants ForAssetId()
		{
            return AssetIdConstants.Instance;
		}

		private static readonly Set<string> SYSTEM_NAMES;

		public static Set<string> SystemNames
		{
			get { return SYSTEM_NAMES; }
		}

		public static bool IsSystemName(string name)
		{
			return SYSTEM_NAMES.Contains(name);
		}

		//NOTE: This code must be after all the static variable declarations that we need to access. Basically, make this
		//the last code in the file.
		static SystemSearchConstants()
		{
			var names = new Set<string>(StringComparer.OrdinalIgnoreCase);
			try
			{
                foreach (MethodInfo constantMethod in GetConstantMethods())
                {
                    names.AddMany(GetNames(constantMethod));
				}
			}
			catch (Exception e)
			{
				//Logger.error("Unable to calculate system JQL names: Unexpected Error.", e);
				names = new Set<string>();
			}

			SYSTEM_NAMES = new Set<string>(names);
		}

        private static readonly IDictionary<string, IClauseInformation> ClauseInformationMap = new Dictionary<string, IClauseInformation>()
        {
            { GetClauseFieldName(ForAllText()), ForAllText() },
            { GetClauseFieldName(ForComments()), ForComments() },
            { GetClauseFieldName(ForCreatedDate()), ForCreatedDate() },
            { GetClauseFieldName(ForCreator()), ForCreator() },
            { GetClauseFieldName(ForDescription()), ForDescription() },
            { GetClauseFieldName(ForAssetId()), ForAssetId() },
            { GetClauseFieldName(ForCatalog()), ForCatalog() },
            { GetClauseFieldName(ForSummary()), ForSummary() },
            { GetClauseFieldName(ForUpdatedDate()), ForUpdatedDate() }
        };

        private static string GetClauseFieldName(IClauseInformation clauseInfo)
        {
            return clauseInfo.FieldId ?? clauseInfo.JqlClauseNames.PrimaryName;
        }

		public static IClauseInformation GetClauseInformationById(string id)
		{
			return ClauseInformationMap[id];
		}

		private static IEnumerable<string> GetNames(MethodInfo constantMethod)
		{
			try
			{
				var information = (IClauseInformation) constantMethod.Invoke(null, null);
				if (information == null)
				{
					//logConstantError(constantMethod, "Clause information was not available.", null);
					return new HashSet<string>();
				}

				var names = information.JqlClauseNames;
				if (names == null)
				{
					//logConstantError(constantMethod, "The ClauseName was not available.", null);
					return new HashSet<string>();
				}

				var strings = names.JqlFieldNames;
				if (strings == null)
				{
					//logConstantError(constantMethod, "The ClauseName returned no values.", null);
					return new HashSet<string>();
				}

				return strings;
			}
			catch (Exception e)
			{
				//logConstantError(constantMethod, "Unexpected Error.", e);
			}
			return new HashSet<string>();
		}

		private static IEnumerable<MethodInfo> GetConstantMethods()
		{
            MethodInfo[] methods;
			try
			{
                methods = typeof(SystemSearchConstants).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			}
			catch (Exception e)
			{
				//Logger.error("Unable to calculate system JQL names: " + e.Message, e);
			    return new List<MethodInfo>();
			}

			IList<MethodInfo> returnMethods = new List<MethodInfo>(methods.Length);
			foreach (MethodInfo method in methods)
			{
				Type returnType = method.ReturnType;
				if (!returnType.IsSubclassOf(typeof(IClauseInformation)))
				{
					continue;
				}

				returnMethods.Add(method);
			}

			return returnMethods;
		}
	}

}