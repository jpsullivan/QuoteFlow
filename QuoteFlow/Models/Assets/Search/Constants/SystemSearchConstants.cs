using System;
using System.Collections.Generic;
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

		public static SimpleFieldSearchConstants forProject()
		{
			return Catalog;
		}

		private static readonly SimpleFieldSearchConstants SUMMARY = new SimpleFieldSearchConstants(DocumentConstants.AssetName, AssetFieldConstants.Summary, "summary", AssetFieldConstants.Summary, AssetFieldConstants.Summary, OperatorClasses.TextOperators, QuoteFlowDataTypes.Text);

		public static SimpleFieldSearchConstants forSummary()
		{
			return SUMMARY;
		}

		private static readonly SimpleFieldSearchConstants DESCRIPTION = new SimpleFieldSearchConstants(DocumentConstants.AssetDesc, AssetFieldConstants.Description, "description", AssetFieldConstants.Description, AssetFieldConstants.Description, OperatorClasses.TextOperators, QuoteFlowDataTypes.Text);

		public static SimpleFieldSearchConstants forDescription()
		{
			return DESCRIPTION;
		}

		public static CommentsFieldSearchConstants forComments()
		{
			return CommentsFieldSearchConstants.Instance;
		}

		private static readonly SimpleFieldSearchConstants CREATED_DATE = new SimpleFieldSearchConstants(DocumentConstants.AssetCreated, new ClauseNames(AssetFieldConstants.Created, "createdDate"), AssetFieldConstants.Created, AssetFieldConstants.Created, AssetFieldConstants.Created, OperatorClasses.EqualityAndRelationalWithEmpty, QuoteFlowDataTypes.Date);

		public static SimpleFieldSearchConstants forCreatedDate()
		{
			return CREATED_DATE;
		}

		private static readonly SimpleFieldSearchConstants UPDATE_DATE = new SimpleFieldSearchConstants(DocumentConstants.AssetUpdated, new ClauseNames(AssetFieldConstants.Updated, "updatedDate"), AssetFieldConstants.Updated, AssetFieldConstants.Updated, AssetFieldConstants.Updated, OperatorClasses.EqualityAndRelationalWithEmpty, QuoteFlowDataTypes.Date);

		public static SimpleFieldSearchConstants forUpdatedDate()
		{
			return UPDATE_DATE;
		}

		public static UserFieldSearchConstantsWithEmpty forAssignee()
		{
			return ASSIGNEE;
		}

		private static readonly UserFieldSearchConstantsWithEmpty CREATOR = new UserFieldSearchConstantsWithEmpty(DocumentConstants.AssetCreator, AssetFieldConstants.Creator, "creator", "creatorSelect", AssetFieldConstants.Creator, DocumentConstants.AssetCreator, AssetFieldConstants.Creator, OperatorClasses.EqualityOperatorsWithEmpty);

		public static UserFieldSearchConstantsWithEmpty forCreator()
		{
			return CREATOR;
		}

		public static SavedFilterSearchConstants forSavedFilter()
		{
			return SavedFilterSearchConstants.Instance;
		}

		public static AllTextSearchConstants forAllText()
		{
			return AllTextSearchConstants.Instance;
		}

		public static IssueIdConstants forIssueId()
		{
			return IssueIdConstants.Instance;
		}

		public static IssueKeyConstants forIssueKey()
		{
			return IssueKeyConstants.Instance;
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
			var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			try
			{
				foreach (Method constantMethod in ConstantMethods)
				{
					names.addAll(getNames(constantMethod));
				}
			}
			catch (Exception e)
			{
				Logger.error("Unable to calculate system JQL names: Unexpected Error.", e);
				names = Collections.emptySet();
			}
			SYSTEM_NAMES = Collections.unmodifiableSet(names);
		}

//		private static final IDictionary<string, IClauseInformation> CLAUSE_INFORMATION_MAP = Maps.uniqueIndex(ImmutableSet.of(forAffectedVersion(), forAllText(), forAssignee(), forComments(), forComponent(), forCreatedDate(), forCreator(), forCurrentEstimate(), forDescription(), forDueDate(), forEnvironment(), forFixForVersion(), forIssueId(), forIssueKey(), forIssueParent(), forIssueType(), forLabels(), forLastViewedDate(), forOriginalEstimate(), forPriority(), forProgress(), forProject(), forProjectCategory(), forReporter(), forResolution(), forResolutionDate(), forSavedFilter(), forSecurityLevel(), forStatus(), forStatusCategory(), forSummary(), forTimeSpent(), forUpdatedDate(), forVoters(), forVotes(), forWatchers(), forWatches(), forWorkRatio(), forAttachments(), forIssueProperty()), new FunctionAnonymousInnerClassHelper()
//	   );
//
//		public static IClauseInformation forIssueProperty()
//		{
//			return new PropertyClauseInformation(new ClauseNames(ISSUE_PROPERTY));
//		}
//
//		public static IClauseInformation getClauseInformationById(string id)
//		{
//			return CLAUSE_INFORMATION_MAP.get(id);
//		}
//
//		private static ICollection<string> getNames(final Method constantMethod)
//		{
//			try
//			{
//				ClauseInformation information = (ClauseInformation) constantMethod.invoke(null);
//				if (information == null)
//				{
//					logConstantError(constantMethod, "Clause information was not available.", null);
//					return Collections.emptySet();
//				}
//
//				ClauseNames names = information.JqlClauseNames;
//				if (names == null)
//				{
//					logConstantError(constantMethod, "The ClauseName was not available.", null);
//					return Collections.emptySet();
//				}
//
//				Set<string> strings = names.JqlFieldNames;
//				if (strings == null)
//				{
//					logConstantError(constantMethod, "The ClauseName returned no values.", null);
//					return Collections.emptySet();
//				}
//
//				return strings;
//			}
//			catch (InvocationTargetException e)
//			{
//				Exception exception;
//				if (e.TargetException != null)
//				{
//					exception = e.TargetException;
//				}
//				else
//				{
//					exception = e;
//				}
//				logConstantError(constantMethod, null, exception);
//			}
//			catch (IllegalAccessException e)
//			{
//				logConstantError(constantMethod, null, e);
//			}
//			catch (SecurityException e)
//			{
//				logConstantError(constantMethod, "Security Error.", e);
//			}
//			catch (Exception e)
//			{
//				logConstantError(constantMethod, "Unexpected Error.", e);
//			}
//			return Collections.emptySet();
//		}
//
//		private static ICollection<Method> ConstantMethods
//		{
//			Method[] methods;
//			try
//			{
//				methods = typeof(SystemSearchConstants).Methods;
//			}
//			catch (SecurityException e)
//			{
//				Logger.error("Unable to calculate system JQL names: " + e.Message, e);
//				return Collections.emptySet();
//			}
//
//			IList<Method> returnMethods = new List<Method>(methods.Length);
//			foreach (Method method in methods)
//			{
//				int modifiers = method.Modifiers;
//				if (!Modifier.isStatic(modifiers) || !Modifier.isPublic(modifiers))
//				{
//					continue;
//				}
//
//				if (method.ParameterTypes.length != 0)
//				{
//					continue;
//				}
//
//				Type returnType = method.ReturnType;
//				if (!returnType.IsSubclassOf(typeof(ClauseInformation)))
//				{
//					continue;
//				}
//
//				returnMethods.Add(method);
//			}
//
//			return returnMethods;
//		}
//
//		private static void logConstantError(final Method constantMethod, final string msg, final Exception th)
//		{
//			string actualMessage = msg;
//			if ((msg == null) && (th != null))
//			{
//				actualMessage = th.Message;
//			}
//
//			Logger.error("Unable to calculate system JQL names for '" + constantMethod.Name + "': " + actualMessage, th);
//		}
//
//		private static Logger Logger
//		{
//			return Logger.getLogger(typeof(SystemSearchConstants));
//		}
	}

}