using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// A builder used to construct the Where <see cref="IClause"/> portion of a JQL <see cref="IQuery"/> in a fluent programming
    /// structure. JQL queries can be defined as one or more terminal clauses, seperated by logical operators, where terminal clauses
    /// define value conditions on specific fields.
    /// <p/>
    /// This builder provided methods to build terminal clauses specific to system fields (e.g. <see cref="#reporter()"/>)
    /// and short cuts for common terminal clauses  (e.g. {@link #unresolved() which produce the terminal clause {@code resolution = Unresolved}.
    /// But also allows the programmer to define his terminal clause components manually, for example
    /// {@code builder.field("cf[100]").in().strings("jql", "rocks").buildQuery()}, this is useful for custom fields.
    /// <p/>
    /// To build Where Clauses with more than one terminal clause, the logical operators must be defined by the programmer between
    /// each call to a terminal clause method, or a default operator must be set. For example to produce the JQL {@code project = HSP and issuetype = bug}
    /// the builder would be used as such {@code builder.project("HSP").and().issueType("bug").buildQuery()} or
    /// {@code builder.defaultAnd().project("HSP").issueType("bug").buildQuery()}. Not defining the operator, such as
    /// {@code builder.project("HSP").issueType("bug").buildQuery()} will cause an illegal state exception.
    /// <p/>
    /// Different logical operators can be specified by the programmer by using the <see cref="IConditionBuilder"/> returned by the field
    /// level methods such as <see cref="#project()"/>. For instance to create the terminal clause {@code component != searching} the programmer would use
    /// the builder as such {@code builder.component().notEq().string("searching")}.
    /// <p/>
    /// By default the builder uses the standard order of precedence. However if the programmer wishes to define their own order,
    /// they can make use of the <see cref="#sub()"/> and <see cref="#endsub()"/> methods, which effectively add opening and closing parenthesis to
    /// the JQL respectively. For instance to create the JQL {@code (resolution is unresolved and assignee is empty) or resolution = fixed}
    /// the programmer would use the builder as such {@code builder.sub().field("resolution").and.assigneeIsEmpty().endsub().or().resolution().eq("fixed")}
    /// <p/>
    /// Generally, it is not possible to passs nulls, empty collections, empty arrays, collections that contain nulls, or arrays
    /// that contain nulls to the method on the interface. Any exceptions to these argument conditions are documented on the method concern.
    /// Passing a method a bad argument will result in a <see cref="IllegalArgumentException"/>.
    /// <p/>
    /// JQL values are of two types <see cref="string"/> and <see cref="Long"/>. For fields that are resolvable by both Id's and Names (e.g.
    /// projects, versions, issue types, components, options etc), the order of resolution depends on the value type. If the JQL
    /// value type is long, JIRA will first attempt to find the domain object by Id, if that fails, it will attempt to find
    /// the domain object by name with the string value of the long. If the JQL value type is a String, JIRA will first try to find
    /// the domain object by name, if that fails AND the string can be parsed into a number, JIRA attempts to find the domain object by
    /// id with that number.
    /// </summary>
    public interface IJqlClauseBuilder
    {
        /// <summary>
        /// Call this to get a handle on the associated <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <returns> the associated <see cref="JqlQueryBuilder"/>. Null may be returned to indicate
        /// there is no associated builder. </returns>
        JqlQueryBuilder EndWhere();

        /// <summary>
        /// Call this method to build a <see cref="QuoteFlow.Api.Jql.Query"/> using the current builder. When <see cref="#endWhere()"/> is not null, this
        /// equates to calling {@code endWhere().buildQuery()}. When {@code endWhere()} is null, this equates to calling
        /// {@code new QueryImpl(buildClause())}.
        /// </summary>
        /// <exception cref="IllegalStateException"> if it is not possible to build the current query given the state of the builder. </exception>
        /// <returns> the newly generated query query. </returns>
        IQuery BuildQuery();

        /// <summary>
        /// Reset the builder to its empty state.
        /// </summary>
        /// <returns>The reset builder.</returns>
        IJqlClauseBuilder Clear();

        /// <summary>
        /// Tell the builder to combine JQL conditions using the "AND" operator when none has been specified. Normally the
        /// caller must ensure that a call to either <see cref="#and()"/> or <see cref="#or()"/> is placed between calls to create JQL
        /// conditions. Calling this method on the builder tells it to automatically add a JQL "AND"
        /// between JQL conditions when no calls to either {@code and} or {@code or} have been made. This mode will remain
        /// active until one of <see cref="#defaultNone()"/>, {@code defaultOr()} or <see cref="#clear()"/> is called.
        /// <p/>
        /// While in this mode it is still possible to explicitly call {@code and} or {@code or} to overide the default
        /// operator for the current condition.
        /// <p/>
        /// For example {@code builder.where().assigneeIsEmpty().or().defaultAnd().reporterIsCurrentUser().affectedVersion("10.5").defaultOr().issueType("bug").buildQuery()}
        /// will build the JQL query "assignee is empty or reporter = currentUser() and affectedVersion = '10.5' or issuetype = bug".
        /// </summary>
        /// <returns>A builder that can be used to further extends the current JQL expression.</returns>
        IJqlClauseBuilder DefaultAnd();

        /// <summary>
        /// Tell the builder to combine JQL conditions using the "OR" operator when none has been specified. Normally the
        /// caller must ensure that a call to either <see cref="#and()"/> or <see cref="#or()"/> is placed between calls to create JQL
        /// conditions. Calling this method on the builder tells it to automatically add a JQL "OR"
        /// between JQL conditions when no calls to either {@code and} or {@code or} have been made. This mode will remain
        /// active until one of <see cref="#defaultNone()"/>, {@code defaultAnd()} or <see cref="#clear()"/> is called.
        /// <p/>
        /// While in this mode it is still possible to explicitly call {@code and} or {@code or} to overide the default
        /// operator for the current condition.
        /// <p/>
        /// For example {@code builder.where().assigneeIsEmpty().and().defaultOr().reporterIsCurrentUser().affectedVersion("10.5").defaultOr().issueType("bug").buildQuery()}
        /// will build the JQL query "assignee is empty and reporter = currentUser() or affectedVersion = '10.5' or issuetype = bug". 
        /// </summary>
        /// <returns>A builder that can be used to further extends the current JQL expression.</returns>
        IJqlClauseBuilder DefaultOr();

        /// <summary>
        /// Tell the builder to stop injecting JQL "AND" or "OR" operators automatically between the generated JQL
        /// conditions. This essentially turns off the behaviour started by calling either
        /// <see cref="#defaultAnd()"/> or <see cref="#defaultOr()"/>.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        IJqlClauseBuilder DefaultNone();

        /// <summary>
        /// Add the JQL "AND" operator to the JQL expression currently being built. The builder takes into account operator
        /// precendence when generating the JQL expression, and as such, the caller may need to group JQL conditions using
        /// the <see cref="#sub()"/> and <see cref="#endsub()"/> calls. For example, {@code builder.not().affectedVersion("11").and().effectedVersion("12")}
        /// produces the JQL {@code NOT (affectedVersion = "11") and affectedVersion = "12"} as the {@link #not() "NOT"
        /// operator} has a higher precedence than "AND". On the other hand, {@code builder.not().sub().affectedVersion("11").and().effectedVersion("12").endsub()}
        /// produces the JQL {@code NOT(affectedVersion = "11" andaffectedVersion = "12")}.
        /// </summary>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add the AND operator given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder And();

        /// <summary>
        /// Add the JQL "OR" operator to the JQL expression currently being built. The builder takes into account operator
        /// precendence when generating the JQL expression, and as such, the caller may need to group JQL conditions using
        /// the <see cref="#sub()"/> and <see cref="#endsub()"/> calls. For example, {@code builder.issueType("bug").and().affectedVersion("11").or().affectedVersion("12")}
        /// produces the JQL {@code (issueType = "bug" andaffectedVersion = "11") or affectedVersion = "12"} as the {@link
        /// #and() "AND" operator} has a higher precedence than "OR". On the other hand, {@code
        /// builder.issueType("bug").and().sub().affectedVersion("11").or().affectedVersion("12").endsub()} produces the JQL
        /// {@code issueType = "bug" and (affectedVersion = "11" or affectedVersion = "12")}.
        /// </summary>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Or();

        /// <summary>
        /// Add the JQL "NOT" operator to the JQL expression currently being built. The builder takes into account operator
        /// precendence when generating the JQL expression, and as such, the caller may need to group JQL conditions using
        /// the <see cref="#sub()"/> and <see cref="#endsub()"/> calls. For example, {@code builder.not().affectedVersion("11").and().effectedVersion("12")}
        /// produces the JQL {@code NOT (affectedVersion = "11") and affectedVersion = "12"} as the <see cref="#and()"/> "AND"
        /// operator} has a lower precedence than "NOT". On the other hand, {@code builder.not().sub().affectedVersion("11").and().effectedVersion("12").endsub()}
        /// produces the JQL {@code NOT(affectedVersion = "11" andaffectedVersion = "12")}.
        /// </summary>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder Not();

        /// <summary>
        /// Create a new sub expression in the current JQL. This essentialy opens a bracket in the JQL query such that all
        /// the JQL expressions from now until the next matching <see cref="#endsub() close bracket"/> are grouped together. This
        /// can be used to override JQL's precedence rules. For example, {@code builder.sub().affectedVersion("12").or().affectedVersion("11").endsub().and().issueType("bug")}
        /// will produce the JQL query {@code (affectedVersion = "12" oraffectedVersion = "12") and type = "bug"}.
        /// </summary>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Sub();

        /// <summary>
        /// End the current sub JQL expression. This essentially adds a close bracket to the JQL query which will close the
        /// last <see cref="#sub() open bracket"/>.
        /// </summary>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Endsub();

        /// <summary>
        /// Add a condition to the query that finds the issues of a particular type. This essentially adds the JQL condition
        /// {@code issuetype in (types)} to the query being built.
        /// </summary>
        /// <param name="types">The QuoteFlow manufacturers to search for. Each one can be specified either by its name (e.g. "Bug") or by
        /// its manufacturer ID as a string (e.g. "10000"). Must not be null, empty or contain any null values. </param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Manufacturer(params string[] types);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for
        /// issue types.
        /// </summary>
        /// <returns> a reference to a IConditionBuilder for issue types. </returns>
        IConditionBuilder Manufacturer();

        /// <summary>
        /// Add a condition to the query that finds the issues that match the passed description. This essentially adds the
        /// condition {@code description ~ "value"} to the query being built.
        /// 
        /// NOTE: The description field performs apporximate text matching not exact text matching.
        /// </summary>
        /// <param name="value">The value of the condition.</param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Description(string value);

        /// <summary>
        /// Add a condition to the query that finds the issues that have no description. This essentially adds the condition
        /// {@code description IS EMPTY} to the query being built.
        /// </summary>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder DescriptionIsEmpty();

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for
        /// issue descriptions.
        /// </summary>
        /// <returns> a reference to a IConditionBuilder for issue descriptions. </returns>
        IConditionBuilder Description();

        /// <summary>
        /// Add a condition to the query that finds the issues match the passed summary. This essentially adds the condition
        /// {@code summary ~ "value"} to the query being built.
        /// 
        /// NOTE: The summary field performs apporximate text matching not exact text matching.
        /// </summary>
        /// <param name="value"> the value of the condition. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder Summary(string value);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for asset summaries.
        /// </summary>
        /// <returns>A reference to a IConditionBuilder for issue summaries. </returns>
        IConditionBuilder Summary();

        /// <summary>
        /// Add a condition to the query that finds the issues that match the passed comment. This essentially adds the
        /// condition {@code comment ~ "value"} to the query being built.
        /// <p/>
        /// NOTE: The comment field performs apporximate text matching not exact text matching.
        /// </summary>
        /// <param name="value"> the value of the condition. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder Comment(string value);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for asset comments.
        /// </summary>
        /// <returns>A reference to a IConditionBuilder for issue comments.</returns>
        IConditionBuilder Comment();

        /// <summary>
        /// Add a condition to the query that finds the assetswithin a particular catalog. This essentially adds the JQL
        /// condition {@code project in (projects)} to the query being built.
        /// </summary>
        /// <param name="catalogs">The QuoteFlow catalogs to search for. Each one can be specified by its name (e.g. "JIRA"), its key
        /// (e.g. "JRA") or by its Quoteflow ID as a string (e.g. "10000"). Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder Catalog(params string[] catalogs);

        /// <summary>
        /// Add a condition to the query that finds the issues within a particular project. This essentially adds the JQL
        /// condition {@code project in (pids)} to the query being built.
        /// </summary>
        /// <param name="catalogIds">The QuoteFlow id's of the projects to search for. Cannot be null, empty or contain null values. </param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Catalog(params int?[] catalogIds);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL 
        /// condition for an asset's catalog.
        /// </summary>
        /// <returns>A reference to a IConditionBuilder for catalogs</returns>
        IConditionBuilder Catalog();

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL 
        /// condition for an asset's cost.
        /// </summary>
        /// <returns>A reference to a IConditionBuilder for asset costs</returns>
        IConditionBuilder Cost();

        /// <summary>
        /// Add a condition to the query that finds the issues that were created after the passed date. This essentially
        /// adds the query {@code created &gt;= startDate} to the query being built.
        /// </summary>
        /// <param name="startDate"> the date that issues must be created after. Cannot be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder CreatedAfter(DateTime startDate);

        /// <summary>
        /// Add a condition to the query that finds the issues that were created after the passed date. This essentially
        /// adds the query {@code created &gt;= startDate} to the query being built.
        /// </summary>
        /// <param name="startDate"> the date that issues must be created after. Can be a date (e.g. "2008-10-23") or a
        /// period (e.g. "-3w"). Cannot be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder CreatedAfter(string startDate);

        /// <summary>
        /// Add a condition to the query that finds the issues that where created between the passed dates. This essentially
        /// adds the query {@code created &gt;= startDate AND created &lt;= endDate} to the query being built. </p> 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code startDate} with a null {@code endDate} will add the condition {@code created &gt;= startDate}. Passing a
        /// non-null {@code endDate} with a null {@code startDate} will add the condition {@code created &lt;= endDate}.
        /// Passing a null {@code startDate} and null {@code endDate} is illegal.
        /// </summary>
        /// <param name="startDate"> the date that issues must be created on or after. May be null if {@code endDate} is not null. </param>
        /// <param name="endDate"> the date that issues must be created on or before. May be null if {@code startDate} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        /// <exception cref="IllegalArgumentException"> if both {@code startDate} and {@code endDate} are null. </exception>
        IJqlClauseBuilder CreatedBetween(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Add a condition to the query that finds the issues that where created between the passed dates. This essentially
        /// adds the query {@code created &gt;= startDateString AND created &lt;= endDateString} to the query being built.
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a
        /// non-null {@code startDateString} with a null {@code endDateString} will add the condition {@code created &gt;=
        /// startDateString}. Passing a non-null {@code endDateString} with a null {@code startDateString} will add the
        /// condition {@code created &lt;= endDateString}. Passing a null {@code startDateString} and null {@code
        /// endDateString} is illegal.
        /// </summary>
        /// <param name="startDateString"> the date that issues must be created on or after. Can be a date (e.g. "2008-10-23") or a
        /// period (e.g. "-3w"). May be null if {@code endDateString} is not null. </param>
        /// <param name="endDateString"> the date that issues must be created on or before. Can be a date (e.g. "2008-10-23") or a
        /// period (e.g. "-3w"). May be null if {@code startDateString} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder CreatedBetween(string startDateString, string endDateString);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for
        /// issue's creation date.
        /// </summary>
        /// <returns> a reference to a IConditionBuilder for created date. </returns>
        IConditionBuilder Created();

        /// <summary>
        /// Add a condition to the query that finds the issues that were updated after the passed date. This essentially
        /// adds the query {@code updated &gt;= startDate} to the query being built.
        /// </summary>
        /// <param name="startDate"> the date that issues must be updated after. Cannot be null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder UpdatedAfter(DateTime startDate);

        /// <summary>
        /// Add a condition to the query that finds the issues that were updated after the passed date. This essentially
        /// adds the query {@code updated &gt;= startDate} to the query being built.
        /// </summary>
        /// <param name="startDate"> the date that issues must be updated after. Can be a date (e.g. "2008-10-23") or a
        /// period (e.g. "-3w"). Cannot be null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder UpdatedAfter(string startDate);

        /// <summary>
        /// Add a condition to the query that finds the issues that where updated between the passed dates. This essentially
        /// adds the query {@code updated &gt;= startDate AND updated &lt;= endDate} to the query being built. </p> It is
        /// also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code startDate} with a null {@code endDate} will add the condition {@code updated &gt;= startDate}. Passing a
        /// non-null {@code endDate} with a null {@code startDate} will add the condition {@code updated &lt;= endDate}.
        /// Passing a null {@code startDate} and null {@code endDate} is illegal.
        /// </summary>
        /// <param name="startDate"> the date that issues must be updated on or after. May be null if {@code endDate} is not null. </param>
        /// <param name="endDate"> the date that issues must be updated on or before. May be null if {@code startDate} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder UpdatedBetween(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Add a condition to the query that finds the issues that where updated between the passed dates. This essentially
        /// adds the query {@code updated &gt;= startDateString AND updated &lt;= endDateString} to the query being built.
        /// </p> It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a
        /// non-null {@code startDateString} with a null {@code endDateString} will add the condition {@code updated &gt;=
        /// startDateString}. Passing a non-null {@code endDateString} with a null {@code startDateString} will add the
        /// condition {@code updated &lt;= endDateString}. Passing a null {@code startDateString} and null {@code
        /// endDateString} is illegal.
        /// </summary>
        /// <param name="startDateString"> the date that issues must be updated on or after. Can be a date (e.g. "2008-10-23") or a
        /// period (e.g. "-3w"). May be null if {@code endDateString} is not null. </param>
        /// <param name="endDateString"> the date that issues must be updated on or before. Can be a date (e.g. "2008-10-23") or a
        /// period (e.g. "-3w"). May be null if {@code startDateString} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder UpdatedBetween(string startDateString, string endDateString);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for
        /// issue's updated date.
        /// </summary>
        /// <returns> a reference to a IConditionBuilder for updated date. </returns>
        IConditionBuilder Updated();

        /// <summary>
        /// Add a condition to the query that will find all assets with the passed id. This essentially adds the JQL condition
        /// {@code key IN (keys)} to the query.
        /// </summary>
        /// <param name="assetIds">The asset keys to search for. Cannot be null, empty or contain any nulls.</param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder Asset(params string[] assetIds);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for the asset's id or key.
        /// </summary>
        /// <returns>A reference to a IConditionBuilder for issue id or key.</returns>
        IConditionBuilder Asset();

        /// <summary>
        /// Add a condition to the query that finds issues which contains/do not contain attachments.
        /// </summary>
        /// <param name="hasAttachment"> true if expecting issues with attachments. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AttachmentsExists(bool hasAttachment);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for the passed name.
        /// </summary>
        /// <param name="jqlName">The name of the JQL condition. Cannot be null.</param>
        /// <returns> a reference to a IConditionBuilder for the passed name. </returns>
        IConditionBuilder Field(string jqlName);

        /// <summary>
        /// Add the passed JQL condition to the query being built.
        /// </summary>
        /// <param name="clause"> the clause to add. Must not be null. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        IJqlClauseBuilder AddClause(IClause clause);

        /// <summary>
        /// Add the JQL condition {@code clausename operator date} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="date"> the date for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, DateTime date);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (dates)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="dates"> dates for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddDateCondition(string clauseName, params DateTime[] dates);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="dates"> date values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, params DateTime[] dates);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (dates)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="dates"> dates for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddDateCondition(string clauseName, ICollection<DateTime> dates);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="dates"> date values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, ICollection<DateTime> dates);

        /// <summary>
        /// Add a condition range condition to the current query for the passed dates. This essentially adds the query {@code
        /// clauseName &gt;= startDate AND clauseName &lt;= endDate} to the query being built. </p> It is also possible to
        /// create an open interval by passing one of the arguments as {@code null}. Passing a non-null {@code startDate}
        /// with a null {@code endDate} will add the condition {@code clauseName &gt;= startDate}. Passing a non-null {@code
        /// endDate} with a null {@code startDate} will add the condition {@code clauseName &lt;= endDate}. Passing a null
        /// {@code startDate} and null {@code endDate} is illegal.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="startDate"> the date for the start of the range. May be null if {@code endDate} is not null. </param>
        /// <param name="endDate"> the date for the end of the range. May be null if {@code startDate} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        /// <exception cref="IllegalArgumentException"> if both {@code startDate} and {@code endDate} are null. </exception>
        IJqlClauseBuilder AddDateRangeCondition(string clauseName, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Add the JQL condition {@code clauseName = functionName()} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="functionName"> name of the function to call. Must not be null. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName);

        /// <summary>
        /// Add the JQL condition {@code clauseName = functionName(arg1, arg2, arg3, ..., argN)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="functionName"> name of the function to call. Must not be null. </param>
        /// <param name="args"> the arguments to add to the function. Must not be null or contain any null values. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName, params string[] args);

        /// <summary>
        /// Add the JQL condition {@code clauseName = functionName(arg1, arg2, arg3, ..., argN)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="functionName"> name of the function to call. Must not be null. </param>
        /// <param name="args"> the arguments to add to the function. Must not be null or contain any null values. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName, ICollection<string> args);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator functionName()} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="functionName"> name of the function to call. Must not be null. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator functionName(arg1, arg2, arg3, ..., argN)} to the query being
        /// built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="functionName"> name of the function to call. Must not be null. </param>
        /// <param name="args"> the arguments to add to the function. Must not be null or contain any null values. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName, params string[] args);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator functionName(arg1, arg2, arg3, ..., argN)} to the query being
        /// built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="functionName"> name of the function to call. Must not be null. </param>
        /// <param name="args"> the arguments to add to the function. Must not be null or contain any null values. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName, ICollection<string> args);

        /// <summary>
        /// Add the JQL condition {@code clauseName = "clauseValue"} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValue"> string value for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. Never null. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddStringCondition(string clauseName, string clauseValue);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValues"> string values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddStringCondition(string clauseName, params string[] clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValues"> string values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddStringCondition(string clauseName, ICollection<string> clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator "clauseValue"} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValue"> string value for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, string clauseValue);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValues"> string values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, params string[] clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValues"> string values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, ICollection<string> clauseValues);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially adds the query {@code
        /// clauseName &gt;= start AND clauseName &lt;= end} to the query being built. </p> It is also
        /// possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null {@code
        /// start} with a null {@code end} will add the condition {@code clauseName &gt;=
        /// start}. Passing a non-null {@code end} with a null {@code start} will add the
        /// condition {@code clauseName &lt;= end}. Passing a null {@code start} and null {@code
        /// end} is illegal.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="start"> the start of the range. May be null if {@code end} is not null. </param>
        /// <param name="end"> the end of the range. May be null if {@code start} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        /// <exception cref="IllegalArgumentException"> if both {@code start} and {@code end} are null. </exception>
        IJqlClauseBuilder AddStringRangeCondition(string clauseName, string start, string end);

        /// <summary>
        /// Add the JQL condition {@code clauseName = clauseValue} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValue"> long value for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, int? clauseValue);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValues"> long values. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, params int?[] clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValues"> long values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, ICollection<int?> clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator clauseValue} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValue"> long value for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, int? clauseValue);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValues"> long values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, params int?[] clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValues"> long values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, ICollection<int?> clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName = clauseValue} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValue"> long value for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, decimal? clauseValue);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValues"> long values. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, params decimal?[] clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="clauseValues"> long values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, ICollection<decimal?> clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator clauseValue} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValue"> long value for the condition. Must not be null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, decimal? clauseValue);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValues"> long values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, params decimal?[] clauseValues);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (clauseValues)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="clauseValues"> long values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, ICollection<decimal?> clauseValues);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially adds the query {@code
        /// clauseName &gt;= start AND clauseName &lt;= end} to the query being built. 
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as null. 
        /// Passing a non-null {@code start} with a null {@code end} will add the condition 
        /// {@code clauseName &gt;=start}. Passing a non-null {@code end} with a null {@code start} 
        /// will add the condition {@code clauseName &lt;= end}. Passing a null {@code start} and 
        /// null {@code end} is illegal.
        /// </summary>
        /// <param name="clauseName">Name of the clause in the condition. Must not be null.</param>
        /// <param name="start">The start of the range. May be null if {@code end} is not null.</param>
        /// <param name="end">The end of the range. May be null if {@code start} is not null.</param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder AddNumberRangeCondition(string clauseName, int? start, int? end);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially adds the query {@code
        /// clauseName &gt;= start AND clauseName &lt;= end} to the query being built. 
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as null. 
        /// Passing a non-null {@code start} with a null {@code end} will add the condition 
        /// {@code clauseName &gt;=start}. Passing a non-null {@code end} with a null {@code start} 
        /// will add the condition {@code clauseName &lt;= end}. Passing a null {@code start} and 
        /// null {@code end} is illegal.
        /// </summary>
        /// <param name="clauseName">Name of the clause in the condition. Must not be null.</param>
        /// <param name="start">The start of the range. May be null if {@code end} is not null.</param>
        /// <param name="end">The end of the range. May be null if {@code start} is not null.</param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder AddNumberRangeCondition(string clauseName, decimal? start, decimal? end);

        /// <summary>
        /// Return a <see cref="IConditionBuilder"/> that can be used to build a JQL condition for
        /// the passed JQL name.
        /// </summary>
        /// <param name="clauseName"> the name of the JQL condition to add. </param>
        /// <returns> a reference to a condition builder for the passed condition. </returns>
        IConditionBuilder AddCondition(string clauseName);

        /// <summary>
        /// Add the JQL condition {@code clauseName = operand} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operand"> defines an operand that will serve as the clause value. Must not be null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddCondition(string clauseName, IOperand operand);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (operands)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operands"> operands values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddCondition(string clauseName, params IOperand[] operands);

        /// <summary>
        /// Add the JQL condition {@code clauseName in (operands)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operands"> operands values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddCondition<T>(string clauseName, ICollection<T> operands) where T : IOperand;

        /// <summary>
        /// Add the JQL condition {@code clauseName operator operand} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator">One of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="operand">Defines an operand that will serve as the clause value. Must not be null. </param>
        /// <returns>A reference to the current builder.</returns>
        IJqlClauseBuilder AddCondition(string clauseName, Operator @operator, IOperand operand);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (operands)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="operands"> values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddCondition(string clauseName, Operator @operator, params IOperand[] operands);

        /// <summary>
        /// Add the JQL condition {@code clauseName operator (operands)} to the query being built.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="operator"> one of the enumerated <see cref="Operator"/>s. Must not be null. </param>
        /// <param name="operands"> values for the condition. Must not be null, empty or contain any null values. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddCondition<T>(string clauseName, Operator @operator, ICollection<T> operands) where T : IOperand;

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially adds the query {@code
        /// clauseName &gt;= start AND clauseName &lt;= end} to the query being built. </p> It is also
        /// possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null {@code
        /// start} with a null {@code end} will add the condition {@code clauseName &gt;=
        /// start}. Passing a non-null {@code end} with a null {@code start} will add the
        /// condition {@code clauseName &lt;= end}. Passing a null {@code start} and null {@code
        /// end} is illegal.
        /// </summary>
        /// <param name="clauseName"> name of the clause in the condition. Must not be null. </param>
        /// <param name="start"> the start of the range. May be null if {@code end} is not null. </param>
        /// <param name="end"> the end of the range. May be null if {@code start} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the
        /// builder. </exception>
        /// <exception cref="IllegalArgumentException"> if both {@code start} and {@code end} are null. </exception>
        IJqlClauseBuilder AddRangeCondition(string clauseName, IOperand start, IOperand end);

        /// <summary>
        /// Add an "IS EMPTY" condition to the current query for the passed JQL clause. This essentially adds the query
        /// {@code clauseName IS EMPTY} to the query being built.
        /// </summary>
        /// <param name="clauseName"> the clause name for the new condition. Cannot be null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder AddEmptyCondition(string clauseName);

        /// <summary>
        /// Create the JQL clause the builder has currently constructed. The builder can still be used after this method is
        /// called.
        /// </summary>
        /// <returns> the clause generated by the builder. Can be null if there is no clause to generate. </returns>
        IClause BuildClause();
    }

}