using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// A builder used during the construction of JQL conditions for a particular JQL field in a fluent programming style. It is used
    /// to specify the value or values of a JQL condition whose operator has already been specified using a <seealso cref="ConditionBuilder"/>. For example,
    /// a call to <seealso cref="IConditionBuilder.eq()"/> will return a ValueBuilder that can be used to specify which value should be matched
    /// by the equals operator.
    /// <p> Each JQL condition is essentially structured as {@code name operator operand}. When this object is created, it has an
    /// implied "name" and "operator". This object can be used to complete the current JQL condition by creating the operand. For example,
    /// {@code JqlQueryBuilder.affectedVersion().eq()} creates a ValueBuilder whose implied name is "affectedVersion" and whose operator
    /// is "=" that can used to complete the JQL condition by filling in the operand.
    /// <p>Generally, it is not possible to passs nulls, empty collections, empty arrays, collections that contain nulls, or arrays
    /// that contain nulls to the methods on this interface. Any exceptions to these argument conditions are documented on the method concerned.
    /// Passing a method a bad argument will result in a <seealso cref="IllegalArgumentException"/>.
    /// <p/>
    /// JQL values are of two types <seealso cref="string"/> and <seealso cref="Long"/>. For fields that are resolvable by both Id's and Names (e.g.
    /// projects, versions, issue types, components, options etc), the order of resolution depends on the value type. If the JQL
    /// value type is long, QuoteFlow will first attempt to find the domain object by Id, if that fails, it will attempt to find
    /// the domain object by name with the string value of the long. If the JQL value type is a String, QuoteFlow will first try to find
    /// the domain object by name, if that fails AND the string can be parsed into a number, QuoteFlow attempts to find the domain object by
    /// id with that number.
    /// </summary>
    public interface IValueBuilder
    {
        /// <summary>
        /// Finish the current condition such that it matches the passed value. It essentially creates the condition
        /// {@code currentName currentOperator value}.
        /// </summary>
        /// <param name="value"> the value of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        IJqlClauseBuilder String(string value);

        /// <summary>
        /// Finish the current condition such that it matches the passed values. It essentially creates the condition
        /// {@code currentName currentOperator (values)}.
        /// </summary>
        /// <param name="values"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        IJqlClauseBuilder Strings(params string[] values);

        /// <summary>
        /// Finish the current condition such that it matches the passed values. It essentially creates the condition
        /// {@code currentName currentOperator (values)}.
        /// </summary>
        /// <param name="values"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        IJqlClauseBuilder Strings(ICollection<string> values);

        /// <summary>
        /// Finish the current condition such that it matches the passed value. It essentially creates the condition
        /// {@code current Namecurrent Operator value}.
        /// </summary>
        /// <param name="value"> the value of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        IJqlClauseBuilder Number(long? value);

        /// <summary>
        /// Finish the current condition such that it matches the passed values. It essentially creates the condition
        /// {@code currentName currentOperator (values)}.
        /// </summary>
        /// <param name="values"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        IJqlClauseBuilder Numbers(params long?[] values);

        /// <summary>
        /// Finish the current condition such that it matches the passed values. It essentially creates the condition
        /// {@code currentName currentOperator (values)}.
        /// </summary>
        /// <param name="values"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        IJqlClauseBuilder Numbers(ICollection<long?> values);

        /// <summary>
        /// Finish the current condition such that it matches the passed operand. It essentially creates the condition
        /// {@code currentName currentOperator operand}.
        /// </summary>
        /// <param name="operand"> the value of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Operand(IOperand operand);

        /// <summary>
        /// Finish the current condition such that it matches the passed operands. It essentially creates the condition
        /// {@code currentName currentOperator (operands)}.
        /// </summary>
        /// <param name="operands"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Operands(params IOperand[] operands);

        /// <summary>
        /// Finish the current condition such that it matches the passed operands. It essentially creates the condition
        /// {@code currentName currentOperator (operands)}.
        /// </summary>
        /// <param name="operands"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Operands<T>(ICollection<T> operands) where T : IOperand;

        /// <summary>
        /// Finish the current condition such that it looks for empty values. It essentially creates the condition
        /// {@code currentName currentOperator EMPTY}. </summary>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Empty();

        /// <summary>
        /// Finish the current condition such that it matches the value(s) returned from the passed function. It essentially creates the condition
        /// {@code currentName currentOperator funcName()}.
        /// </summary>
        /// <param name="funcName"> the name of the function whose value(s) must be matched. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Function(string funcName);

        /// <summary>
        /// Finish the current condition such that it matches the value(s) returned from the passed function. It essentially creates the condition
        /// {@code currentName currentOperator funcName(arg1, arg2, arg3, ..., arg4)}.
        /// </summary>
        /// <param name="funcName"> the name of the function whose value(s) must be matched. </param>
        /// <param name="args"> the arguments to be passed to the function. Cannot be null or contain null values. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Function(string funcName, params string[] args);

        /// <summary>
        /// Finish the current condition such that it matches the value(s) returned from the passed function. It essentially creates the condition
        /// {@code currentName currentOperator funcName(arg1, arg2, arg3, ..., arg4)}.
        /// </summary>
        /// <param name="funcName"> the name of the function whose value(s) must be matched. </param>
        /// <param name="args"> the arguments to be passed to the function. Cannot be null or contain null values. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Function(string funcName, ICollection<string> args);

        /// <summary>
        /// Finish the current condition such that it matches the values returned from the "membersOfFunction" function. This function returns all QuoteFlow
        /// users that are a members of the passed group. It essentially adds the condition
        /// {@code currentName currentOperator membersOf("groupName")} to the query.
        /// </summary>
        /// <param name="groupName"> the name of the group to search. Cannot be null. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionMembersOf(string groupName);

        /// <summary>
        /// Finish the current condition such that it matches the value returned from the "currentUser" function. This function returns the currently
        /// logged in QuoteFlow user. It essentially adds the condition {@code currentName currentOperator currentUser()} to the query.
        /// </summary>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionCurrentUser();

        /// <summary>
        /// Finish the current condition such that it matches the value returned from the "now" function. This function returns the time when
        /// it is run. It essentially adds the condition {@code currentName currentOperator now()} to the query.
        /// </summary>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionNow();

        /// <summary>
        /// Finish the current condition such that it matches the value(s) returned from the "cascadingOption" function.
        /// It essentially adds the condition {@code currentName currentOperator cascadingOption(parent).}
        /// </summary>
        /// <param name="parent"> the first argument to the "cascadingOption" function. Cannot be null. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionCascadingOption(string parent);

        /// <summary>
        /// Finish the current condition such that it matches the value(s) returned from the "cascadingOption" function.
        /// It essentially adds the condition {@code currentName currentOperator cascadingOption(parent, child).}
        /// </summary>
        /// <param name="parent"> the first argument to the "cascadingOption" function. Cannot be null. </param>
        /// <param name="child"> the second argument to the "cascadingOption" function. Cannot be null. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionCascadingOption(string parent, string child);

        /// <summary>
        /// Finish the current condition such that it matches the value(s) returned from the "cascadingOption" function.
        /// It essentially adds the condition {@code currentName currentOperator cascadingOption(parent, "none").}
        /// </summary>
        /// <param name="parent"> parent the first argument to the "cascadingOption" function. Cannot be null. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionCascadingOptionParentOnly(string parent);

        /// <summary>
        /// Finish the current condition such that it matches the value returned from the "lastLogin" function. This functions
        /// returns the time when the current user previously logged in.
        /// </summary>
        /// <returns> the builder of the overall JQL query </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionLastLogin();

        /// <summary>
        /// Finish the current condition such that it matches the value returned from the "currentLogin" function. This functions
        /// returns the time when the current user logged in.
        /// </summary>
        /// <returns> the builder of the overall JQL query </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder FunctionCurrentLogin();

        /// <summary>
        /// Specify the value that must me matched using the current operator. It essentially creates the condition
        /// {@code currentName currentOperator date}.
        /// </summary>
        /// <param name="date"> the value of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Date(DateTime date);

        /// <summary>
        /// Specify the values that must be matched using the current operator. It essentially creates the condition
        /// {@code currentName currentOperator (dates)}.
        /// </summary>
        /// <param name="dates"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Dates(params DateTime[] dates);

        /// <summary>
        /// Specify the values that must be matched using the current operator. It essentially creates the condition
        /// {@code currentName currentOperator (dates)}.
        /// </summary>
        /// <param name="dates"> the values of the JQL condition. </param>
        /// <returns> the bulder of the overall JQL query. </returns>
        /// <exception cref="IllegalStateException"> if it is not possible to add a JQL condition given the current state of the builder. </exception>
        IJqlClauseBuilder Dates(ICollection<DateTime> dates);
    }
}