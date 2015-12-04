using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// An interface that is used to build JQL conditions for a particular JQL field in a fluent programming style. It is
    /// created and returned from the factory methods on the <see cref="IJqlClauseBuilder"/>. For
    /// example, the <see cref="IJqlClauseBuilder#affectedVersion()"/> method creates a new ConditionBuilder for the affectes version
    /// system field in QuoteFlow.
    /// 
    /// The object's main job is to specify the operator for the JQL current condition being generated. The value of the JQL
    /// condition can be specified when the operator is specified, or later using the {@link
    /// IValueBuilder}. For example, the JQL clause {@code affectedVersion = "1.2"} can be
    /// generated using {@code JqlQueryBuilder.affectedVersion().eq("1.2").build()} or {@code
    /// JqlQueryBuilder.affectedVersion().eq().string("1.2").build()}.
    /// 
    /// Generally, it is not possible to passs nulls, empty collections, empty arrays, collections that contain nulls, or arrays
    /// that contain nulls to the method on the interface. Any exceptions to these argument conditions are documented on the method concern.
    /// Passing a method a bad argument will result in a <see cref="IllegalArgumentException"/>.
    /// 
    /// JQL values are of two types <see cref="string"/> and <see cref="Long"/>. For fields that are resolvable by both Id's and Names (e.g.
    /// projects, versions, issue types, components, options etc), the order of resolution depends on the value type. If the JQL
    /// value type is long, QuoteFlow will first attempt to find the domain object by Id, if that fails, it will attempt to find
    /// the domain object by name with the string value of the long. If the JQL value type is a String, QuoteFlow will first try to find
    /// the domain object by name, if that fails AND the string can be parsed into a number, QuoteFlow attempts to find the domain object by
    /// id with that number.
    /// </summary>
    public interface IConditionBuilder
    {
        /// <summary>
        /// Make the operator for the JQL condition <see cref="Operator.EQUALS"/>. The value of
        /// the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns>A builder that can be used to specify the value of the condition.</returns>
        IValueBuilder Eq();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// passed value. It essentially creates the JQL condition {@code name = "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Eq(string value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Operator.EQUALS"/> operator and the
        /// passed value. It essentially creates the JQL condition {@code name = value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder Eq(int? value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name = date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Eq(DateTime date);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// passed value. It essentially creates the JQL condition {@code name = operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Eq(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// EMPTY value. It essentially creates the JQL condition {@code name = EMPTY}.
        /// </summary>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder EqEmpty();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// passed function. It essentially creates the JQL condition {@code name = funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder EqFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// passed function. It essentially creates the JQL condition {@code name = funcName(arg1, arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder EqFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#EQUALS equals operator"/> and the
        /// passed function. It essentially creates the JQL condition {@code name = funcName(arg1, arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder EqFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#NOT_EQUALS not equals"/>. The
        /// value of the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder NotEq();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the passed value. It essentially creates the JQL condition {@code name != "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEq(string value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the passed value. It essentially creates the JQL condition {@code name != value}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEq(int? value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the passed value. It essentially creates the JQL condition {@code name != operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEq(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name != date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEq(DateTime date);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the EMPTY value. It essentially creates the JQL condition {@code name != EMPTY}.
        /// </summary>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEqEmpty();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the passed function. It essentially creates the JQL condition {@code name != funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEqFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the passed function. It essentially creates the JQL condition {@code name != funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEqFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_EQUALS not equals operator"/>
        /// and the passed function. It essentially creates the JQL condition {@code name != funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEqFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#LIKE like"/>. The value of the
        /// condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder Like();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the passed
        /// value. It essentially creates the JQL condition {@code name ~ "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder Like(string value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the passed
        /// value. It essentially creates the JQL condition {@code name ~ value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder Like(int? value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the passed
        /// value. It essentially creates the JQL condition {@code name ~ operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Like(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name ~ date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Like(DateTime date);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the passed
        /// function. It essentially creates the JQL condition {@code name ~ funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LikeFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the passed
        /// function. It essentially creates the JQL condition {@code name ~ funcName(arg1, arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LikeFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LIKE like operator"/> and the passed
        /// function. It essentially creates the JQL condition {@code name != funcName(arg1, arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LikeFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#NOT_LIKE not like"/>. The
        /// value of the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder NotLike();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and
        /// the passed value. It essentially creates the JQL condition {@code name !~ "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotLike(string value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and
        /// the passed value. It essentially creates the JQL condition {@code name !~ value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null. </param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder NotLike(int? value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and
        /// the passed value. It essentially creates the JQL condition {@code name !~ operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotLike(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name !~ date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotLike(DateTime date);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name !~ funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotLikeFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name !~ funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotLikeFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_LIKE not like operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name !~ funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotLikeFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#IS is"/>. The value of the
        /// condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder Is();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IS is operator"/> and the EMPTY
        /// value. It essentially creates the JQL condition {@code name IS EMPTY}.
        /// </summary>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Empty { get; }

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#IS_NOT is not"/>. The value of
        /// the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder IsNot();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IS_NOT is not operator"/> and the
        /// EMPTY value. It essentially creates the JQL condition {@code name IS NOT EMPTY}.
        /// </summary>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotEmpty { get; }

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#LESS_THAN less than"/>. The
        /// value of the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder Lt();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN less than operator"/> and
        /// the passed value. It essentially creates the JQL condition {@code name &lt; "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Lt(string value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN less than operator"/> and
        /// the passed value. It essentially creates the JQL condition {@code name &lt; value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder Lt(int? value);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN less than operator"/> and
        /// the passed value. It essentially creates the JQL condition {@code name &lt; operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Lt(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN less than operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name &lt; date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Lt(DateTime date);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN "&lt;" operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name < funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN "&lt;" operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name < funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN `&lt;` operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name < funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition {@link Operator#LESS_THAN_EQUALS less than
        /// equals}. The value of the condition can be specified using the returned {@link
        /// IValueBuilder}.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder LtEq();

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS less than equals
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &lt;= "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtEq(string value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS less than equals
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &lt;= value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder LtEq(int? value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS less than equals
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &lt;= value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder LtEq(decimal? value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS less than equals
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &lt;= operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtEq(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#LESS_THAN_EQUALS less than or equals operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name &lt;= date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtEq(DateTime date);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS "&lt;="
        /// operator} and the passed function. It essentially creates the JQL condition {@code name &lt;= funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtEqFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS "&lt;="
        /// operator} and the passed function. It essentially creates the JQL condition {@code name &lt;= funcName(arg1,
        /// arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtEqFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#LESS_THAN_EQUALS "&lt;="
        /// operator} and the passed function. It essentially creates the JQL condition {@code name &lt;= funcName(arg1,
        /// arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder LtEqFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#GREATER_THAN greater than"/>.
        /// The value of the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder Gt();

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN greater than
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &gt; "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Gt(string value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN greater than
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &gt; value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder Gt(int? value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN greater than
        /// operator} and the passed value. It essentially creates the JQL condition {@code name &gt; operand}.
        /// </summary>
        /// <param name="operand">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder Gt(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#GREATER_THAN greater than operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name &gt; date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder Gt(DateTime date);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#GREATER_THAN `&gt;` operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name &gt; funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#GREATER_THAN `&gt;` operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name &gt; funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#GREATER_THAN `&gt;` operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name &gt; funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition {@link Operator#GREATER_THAN_EQUALS greater
        /// than equals}. The value of the condition can be specified using the returned {@link
        /// IValueBuilder}.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder GtEq();

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed value. It essentially creates the JQL condition {@code name &gt;= "str"}.
        /// </summary>
        /// <param name="value"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtEq(string value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed value. It essentially creates the JQL condition {@code name &gt;= value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder GtEq(int? value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed value. It essentially creates the JQL condition {@code name &gt;= value}.
        /// </summary>
        /// <param name="value">The value of the condition. Cannot be null.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder GtEq(decimal? value);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed value. It essentially creates the JQL condition {@code name &gt;= operand}.
        /// </summary>
        /// <param name="operand"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtEq(IOperand operand);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#GREATER_THAN_EQUALS greater than or equals operator"/> and the
        /// passed date. It essentially creates the JQL condition {@code name &gt;= date}.
        /// </summary>
        /// <param name="date"> the value of the condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtEq(DateTime date);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed function. It essentially creates the JQL condition {@code name &gt;= funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtEqFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed function. It essentially creates the JQL condition {@code name &gt;= funcName(arg1,
        /// arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtEqFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the {@link Operator#GREATER_THAN_EQUALS greater than
        /// equals operator} and the passed function. It essentially creates the JQL condition {@code name &gt;= funcName(arg1,
        /// arg2, arg3, ... argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder GtEqFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Operator.IN"/>. The values of the
        /// condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder In();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name IN (values)}.
        /// </summary>
        /// <param name="values"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder In(params string[] values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name IN (values)}.
        /// </summary>
        /// <param name="values"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder InStrings(ICollection<string> values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Operator.IN "/> in operator and the passed
        /// values. It essentially creates the JQL condition {@code name IN (values)}.
        /// </summary>
        /// <param name="values">The values of the condition. Cannot be null, empty or contain any null value.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder In(params int?[] values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Operator.IN"/> in operator and the passed
        /// values. It essentially creates the JQL condition {@code name IN (values)}.
        /// </summary>
        /// <param name="values">The values of the condition. Cannot be null, empty or contain any null value.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder InNumbers(ICollection<int?> values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name IN (operands)}.
        /// </summary>
        /// <param name="operands"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder In(params IOperand[] operands);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name IN (operands)}.
        /// </summary>
        /// <param name="operands"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder InOperands(ICollection<IOperand> operands);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name IN (dates)}.
        /// </summary>
        /// <param name="dates"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder In(params DateTime[] dates);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name IN (dates)}.
        /// </summary>
        /// <param name="dates"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder InDates(ICollection<DateTime> dates);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN"/> in operator and
        /// the passed function. It essentially creates the JQL condition {@code name in funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder InFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name in funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder InFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN in operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name in funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder InFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Make the operator for the JQL condition <see cref="Api.Jql.Operator#NOT_IN not in"/>. The values
        /// of the condition can be specified using the returned <see cref="IValueBuilder"/>.
        /// </summary>
        /// <returns> a builder that can be used to specify the value of the condition. </returns>
        IValueBuilder NotIn();

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN not in operator"/> and the
        /// passed values. It essentially creates the JQL condition {@code name NOT IN (values)}.
        /// </summary>
        /// <param name="values"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotIn(params string[] values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN not in operator"/> and the
        /// passed values. It essentially creates the JQL condition {@code name NOT IN (values)}.
        /// </summary>
        /// <param name="values"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotInStrings(ICollection<string> values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN not in operator"/> and the
        /// passed values. It essentially creates the JQL condition {@code name NOT IN (values)}.
        /// </summary>
        /// <param name="values">The values of the condition. Cannot be null, empty or contain any null value.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder NotIn(params int?[] values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN not in operator"/> and the
        /// passed values. It essentially creates the JQL condition {@code name NOT IN (values)}.
        /// </summary>
        /// <param name="values">The values of the condition. Cannot be null, empty or contain any null value.</param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder NotInNumbers(ICollection<int?> values);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN not in operator"/> and the
        /// passed values. It essentially creates the JQL condition {@code name NOT IN (operands)}.
        /// </summary>
        /// <param name="operands"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotIn(params IOperand[] operands);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN not in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name NOT IN (dates)}.
        /// </summary>
        /// <param name="dates"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotIn(params DateTime[] dates);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name NOT IN (dates)}.
        /// </summary>
        /// <param name="dates"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotInDates(ICollection<DateTime> dates);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#IN not in operator"/> and the passed
        /// values. It essentially creates the JQL condition {@code name NOT IN (operands)}.
        /// </summary>
        /// <param name="operands"> the values of the condition. Cannot be null, empty or contain any null value. </param>
        /// <returns>The <see cref="IJqlClauseBuilder"/> that created the condition.</returns>
        IJqlClauseBuilder NotInOperands(ICollection<IOperand> operands);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN "not in" operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name not in funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. Cannot be null. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotInFunc(string funcName);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN "not in" operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name not in funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotInFunc(string funcName, params string[] args);

        /// <summary>
        /// Create the JQL condition with the <see cref="Api.Jql.Operator#NOT_IN "not in" operator"/> and
        /// the passed function. It essentially creates the JQL condition {@code name not in funcName(arg1, arg2, arg3, ...
        /// argN)}.
        /// </summary>
        /// <param name="funcName"> the name of the function in the new condition. </param>
        /// <param name="args"> the arguments for the function. Cannot be null or contain any null values. </param>
        /// <returns> the <see cref="IJqlClauseBuilder"/> that created the condition. </returns>
        IJqlClauseBuilder NotInFunc(string funcName, ICollection<string> args);

        /// <summary>
        /// Add a condition range condition to the current query for the passed dates. This essentially
        /// adds the query {@code clauseName &gt;= start AND clauseName &lt;= end} to the query being built.
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code start} with a null {@code end} will add the condition {@code clauseName &gt;= start}. Passing
        /// a non-null {@code end} with a null {@code start} will add the condition {@code clauseName &lt;= end}.
        /// Passing a null {@code start} and null {@code end} is illegal.
        /// </summary>
        /// <param name="start"> the date for the start of the range. May be null if {@code end} is not null. </param>
        /// <param name="end"> the date for the end of the range. May be null if {@code start} is not null. </param>
        IJqlClauseBuilder Range(DateTime start, DateTime end);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. 
        /// This essentially adds the query {@code clauseName &gt;= start AND clauseName &lt;= end} 
        /// to the query being built.
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code start} with a null {@code end} will add the condition {@code clauseName &gt;= start}. Passing
        /// a non-null {@code end} with a null {@code start} will add the condition {@code clauseName &lt;= end}.
        /// Passing a null {@code start} and null {@code end} is illegal.
        /// </summary>
        /// <param name="start"> the value for the start of the range. May be null if {@code end} is not null. </param>
        /// <param name="end"> the value for the end of the range. May be null if {@code start} is not null. </param>
        IJqlClauseBuilder Range(string start, string end);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially
        /// adds the query {@code clauseName &gt;= start AND clauseName &lt;= end} to the query being built.
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code start} with a null {@code end} will add the condition {@code clauseName &gt;= start}. Passing
        /// a non-null {@code end} with a null {@code start} will add the condition {@code clauseName &lt;= end}.
        /// Passing a null {@code start} and null {@code end} is illegal.
        /// </summary>
        /// <param name="start">The value for the start of the range. May be null if {@code end} is not null.</param>
        /// <param name="end">The value for the end of the range. May be null if {@code start} is not null.</param>
        IJqlClauseBuilder Range(int? start, int? end);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially
        /// adds the query {@code clauseName &gt;= start AND clauseName &lt;= end} to the query being built.
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code start} with a null {@code end} will add the condition {@code clauseName &gt;= start}. Passing
        /// a non-null {@code end} with a null {@code start} will add the condition {@code clauseName &lt;= end}.
        /// Passing a null {@code start} and null {@code end} is illegal.
        /// </summary>
        /// <param name="start">The value for the start of the range. May be null if {@code end} is not null.</param>
        /// <param name="end">The value for the end of the range. May be null if {@code start} is not null.</param>
        IJqlClauseBuilder Range(decimal? start, decimal? end);

        /// <summary>
        /// Add a condition range condition to the current query for the passed values. This essentially
        /// adds the query {@code clauseName &gt;= start AND clauseName &lt;= end} to the query being built.
        /// 
        /// It is also possible to create an open interval by passing one of the arguments as {@code null}. Passing a non-null
        /// {@code start} with a null {@code end} will add the condition {@code clauseName &gt;= start}. Passing
        /// a non-null {@code end} with a null {@code start} will add the condition {@code clauseName &lt;= end}.
        /// Passing a null {@code start} and null {@code end} is illegal.
        /// </summary>
        /// <param name="start"> the value for the start of the range. May be null if {@code end} is not null. </param>
        /// <param name="end"> the value for the end of the range. May be null if {@code start} is not null. </param>
        /// <returns> a reference to the current builder. </returns>
        IJqlClauseBuilder Range(IOperand start, IOperand end);
    }

}