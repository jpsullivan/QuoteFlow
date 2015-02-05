using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// An implementation of <seealso cref="SimpleClauseBuilder"/> that takes JQL prededence into account when building its associated
    /// JQL <seealso cref="com.atlassian.query.clause.Clause"/>. For exmaple, the expression {@code
    /// builder.clause(clause1).or().clause(clause2).and().clause(clause3).build()} will return the Clause representation of
    /// <code>clause1 OR (clause2 AND clause3)</code>.
    /// </summary>
    public class PrecedenceSimpleClauseBuilder : ISimpleClauseBuilder
    {
        private readonly Stacks _stacks;
        private IBuilderState _builderState;
        private BuilderOperator _defaultOperator;

        public PrecedenceSimpleClauseBuilder()
        {
            this._stacks = new Stacks();
            this._builderState = StartState
        }

        public ISimpleClauseBuilder Clear()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder And()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder Or()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder Not()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder Clause(IClause clause)
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder Sub()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder Endsub()
        {
            throw new NotImplementedException();
        }

        public IClause Build()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder Copy()
        {
            return new PrecedenceSimpleClauseBuilder(this);
        }

        public ISimpleClauseBuilder DefaultAnd()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder DefaultOr()
        {
            throw new NotImplementedException();
        }

        public ISimpleClauseBuilder DefaultNone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stack used to help with building the JQL clause using operator precedence. 
        /// There are two stacks here, an operator stack and an operand stack.
        /// 
        /// It implements the 
        /// <a href="http://en.wikipedia.org/wiki/Shunting_yard_algorithm">Shunting Yard Algorithm</a> 
        /// to implement operator precedence.
        /// </summary>
        private sealed class Stacks
        {
            private LinkedList<BuilderOperator> Operators { get; set; }
            private LinkedList<IMutableClause> Operands { get; set; }

            internal int Level { get; set; }

            public Stacks()
            {
                Operators = new LinkedList<BuilderOperator>();
                Operands = new LinkedList<IMutableClause>();
            }

            /// <summary>
            /// Make a safe deep copy of the passed state. The new state should be independent of the passed state.
            /// </summary>
            /// <param name="state"> the state to copy. </param>
            internal Stacks(Stacks state)
            {
                Operators = new LinkedList<BuilderOperator>(state.Operators);
                Operands = new LinkedList<IMutableClause>();
                Level = state.Level;

                foreach (IMutableClause operand in state.Operands)
                {
                    Operands.AddLast(operand.Copy());
                }
            }

            public void Clear()
            {
                Operands.Clear();
                Operators.Clear();
                Level = 0;
            }

            private BuilderOperator? PopOperator()
            {
                BuilderOperator? builderOperator = Operators.First();
                Operators.Remove(0);
                if (builderOperator == BuilderOperator.LPAREN)
                {
                    Level = Level - 1;
                }
                return builderOperator;
            }

            private BuilderOperator? PeekOperator()
            {
                return Operators.ElementAt(0);
            }

            private bool HasOperator()
            {
                return Operators.Count > 0;
            }

            private void PushOperand(IMutableClause operand)
            {
                Operands.AddFirst(operand);
            }

            private IMutableClause PopClause()
            {
                return Operands.Remove(0);
            }

            private IMutableClause PeekClause()
            {
                return Operands[0];
            }

            /// <summary>
            /// Process the current operators on the operator stack using the Shunting Yard Algorithm and then push the
            /// passed operator onto the operator stack. Passing null to this method will leave the complete MutableClause as
            /// the only argument on the operand stack.
            /// </summary>
            /// <param name="op">The operator to add to the stack. A <code>null</code> argument means that the operator stack should be emptied.</param>
            private void ProcessAndPush(BuilderOperator? op)
            {
                if (op != BuilderOperator.LPAREN && HasOperator())
                {
                    BuilderOperator? currentTop = PeekOperator();

                    // The NOT operator is special since it a right-associative unary operator. For right-associative
                    // operators only process when something of higher but not equal precedence appears on the stack.
                    int compare = op == BuilderOperator.NOT ? -1 : 0;

                    while (currentTop != null && (op == null || op.CompareTo(currentTop) <= compare))
                    {
                        PopOperator();

                        //Get the operands for the operator we are about to process.
                        IMutableClause leftOperand;
                        IMutableClause rightOperand;

                        if (currentTop == BuilderOperator.NOT)
                        {
                            leftOperand = PopClause();
                            rightOperand = null;
                        }
                        else
                        {
                            rightOperand = PopClause();
                            leftOperand = PopClause();
                        }

                        //Execute the operator and add the result to the top of the stack as the argument for the next
                        //operator.
                        PushOperand(leftOperand.Combine(currentTop, rightOperand));

                        if (HasOperator())
                        {
                            currentTop = PeekOperator();
                        }
                        else
                        {
                            currentTop = null;
                        }
                    }
                }

                if (op != null)
                {
                    if (op == BuilderOperator.RPAREN)
                    {
                        if (HasOperator() && PeekOperator() == BuilderOperator.LPAREN)
                        {
                            PopOperator();
                        }
                        else
                        {
                            throw new InvalidOperationException("The ')' does not have a matching '('.");
                        }
                    }
                    else
                    {
                        if (op == BuilderOperator.LPAREN)
                        {
                            Level = Level + 1;
                        }

                        Operators.AddFirst(op);
                    }
                }
            }

//            /// <summary>
//            /// Get the, possibly partial, JQL expression for the current stack state. Used mainly for error messages.
//            /// </summary>
//            /// <returns>The current JQL expression given the state of the stacks.</returns>
//            internal string DisplayString
//            {
//                get
//                {
//                    IEnumerator<BuilderOperator> operatorIterator = new InfiniteReversedIterator<BuilderOperator>(Operators.listIterator(Operators.Count));
//                    IEnumerator<IMutableClause> clauseIterator = new InfiniteReversedIterator<IMutableClause>(Operands.ListIterator(Operands.Count));
//                    StringBuilder stringBuilder = new StringBuilder();
//
//                    bool start = true;
//                    //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                    BuilderOperator op = operatorIterator.next();
//                    while (op != null)
//                    {
//                        switch (op)
//                        {
//                            case LPAREN:
//                                AddString(stringBuilder, "(");
//                                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                                op = operatorIterator.next();
//                                start = true;
//                                break;
//                            case RPAREN:
//                                //just append these where seen.
//                                stringBuilder.Append(")");
//                                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                                op = operatorIterator.next();
//                                start = false;
//                                break;
//                            case AND:
//                            case OR:
//                                //do we need to add the starting operand for this operator. We only do this for new expressions.
//                                if (start)
//                                {
//                                    //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                                    AddString(stringBuilder, ClauseToString(clauseIterator.next(), op));
//                                }
//
//                                //Fall through here on purpose.
//                                goto case NOT;
//                            case NOT:
//                                AddString(stringBuilder, op.ToString());
//
//                                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                                BuilderOperator nextOperator = operatorIterator.next();
//                                //We don't want to print the next operand yet for these two operators.
//                                if (nextOperator != BuilderOperator.LPAREN && nextOperator != BuilderOperator.NOT)
//                                {
//                                    //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                                    AddString(stringBuilder, ClauseToString(clauseIterator.next(), op));
//                                }
//                                start = false;
//                                op = nextOperator;
//                                break;
//                        }
//                    }
//
//                    //Loop through any remaining operands and add them. There should only ever be one.
//                    IMutableClause clause = clauseIterator.next();
//                    while (clause != null)
//                    {
//                        AddString(stringBuilder, ClauseToString(clause, null));
//                        //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//                        clause = clauseIterator.next();
//                    }
//
//                    return stringBuilder.ToString();
//                }
//            }

            private static void AddString(StringBuilder builder, string append)
            {
                if (append.Length == 0)
                {
                    return;
                }

                int length = builder.Length;
                if (length != 0 && builder[length - 1] != '(')
                {
                    builder.Append(" ");
                }

                builder.Append(append);
            }

            /// <summary>
            /// Return a string representation of the passed clause.
            /// </summary>
            /// <param name="clause"> the clause to stringify. </param>
            /// <param name="op"> the operator that this clause belongs to, that is, this clause is an operand of this
            /// operator. </param>
            /// <returns> the string version of the passed clause. </returns>
            private static string ClauseToString(IMutableClause clause, BuilderOperator op)
            {
                if (clause == null)
                {
                    return "";
                }
                IClause jqlClause = clause.AsClause();
                if (jqlClause == null)
                {
                    return "";
                }

                //We need to bracket the clause if its primary operator has lower precedence than the passed
                //operator.
                BuilderOperator clauseOperator = OperatorVisitor.findOperator(jqlClause);
                if (op != null && clauseOperator != null && op.CompareTo(clauseOperator) > 0)
                {
                    return "(" + jqlClause.ToString() + ")";
                }
                else
                {
                    return jqlClause.ToString();
                }
            }

            public IClause AsClause()
            {
                return AsMutableClause().AsClause();
            }

            private IMutableClause AsMutableClause()
            {
                ProcessAndPush();
                return PeekClause();
            }

            public override string ToString()
            {
                return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
            }
        }

        /// <summary>
        /// Represents the state of the builder. The PrecedenceBasicBuilder will delegate its operations to its current state
        /// to perform.
        /// </summary>
        private interface IBuilderState
        {
            IBuilderState Enter(Stacks stacks);

            IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator);

            IBuilderState And(Stacks stacks);

            IBuilderState Or(Stacks stacks);

            IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator);

            IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator);

            IBuilderState Endgroup(Stacks stacks);

            IClause Build(Stacks stacks);

            IBuilderState Copy(Stacks stacks);
        }

        /// <summary>
        /// This is the initial state for the builder. In this state the builder expects a clause, sub-clause or a NOT clause
        /// to be added. It is also possible to build in this state, however, the builder will return <code>null</code> as no
        /// condition has can be generated.
        /// 
        /// @since v4.0.
        /// </summary>
        private class StartState : BuilderState
        {
            internal static readonly StartState INSTANCE = new StartState();

            internal StartState()
            {
            }

            public virtual BuilderState enter(Stacks stacks)
            {
                return this;
            }

            /// <summary>
            /// When NOT is called we transition to the <seealso cref="PrecedenceSimpleClauseBuilder.NotState"/> expecting a NOT clause
            /// to be added.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return NotState.INSTANCE;
            }

            public virtual BuilderState and(Stacks stacks)
            {
                return this;
            }

            public virtual BuilderState or(Stacks stacks)
            {
                return this;
            }

            /// <summary>
            /// When a clause is added an AND or OR operator is expected next so we transition into the {@link
            /// OperatorState}.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="clause"> the clause to add to tbe builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState add(Stacks stacks, MutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.pushOperand(clause);
                return OperatorState.INSTANCE;
            }

            /// <summary>
            /// We now have a sub-expression so lets transition into that state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return StartGroup.INSTANCE;
            }

            public virtual BuilderState endgroup(Stacks stacks)
            {
                throw new IllegalStateException("Tying to start JQL expression with ')'.");
            }

            public virtual Clause build(Stacks stacks)
            {
                return null;
            }

            public virtual BuilderState copy(Stacks stacks)
            {
                return this;
            }

            public override string ToString()
            {
                return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
            }
        }

        /// <summary>
        /// This is the state of the builder when the user is trying to enter in a negated clause. In this state it is only
        /// possible to add a clause, start a sub-expression or negate again.
        /// 
        /// @since v4.0.
        /// </summary>
        private class NotState : BuilderState
        {
            internal static readonly NotState INSTANCE = new NotState();

            internal NotState()
            {
            }

            /// <summary>
            /// Upon entry into this state, we always add a not operator.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState enter(Stacks stacks)
            {
                stacks.processAndPush(BuilderOperator.NOT);
                return this;
            }

            /// <summary>
            /// We are negating the clause again, so re-enter our state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return this;
            }

            public virtual BuilderState and(Stacks stacks)
            {
                throw new IllegalStateException("Trying to create the illegal JQL expression 'NOT AND'. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual BuilderState or(Stacks stacks)
            {
                throw new IllegalStateException("Trying to create the illegal JQL expression 'NOT OR'. The current JQL is '" + stacks.DisplayString + "'.");
            }

            /// <summary>
            /// Add the clause to the operand stack so that it may be negated.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="clause"> the clause to add to the query. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState add(Stacks stacks, MutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.pushOperand(clause);
                return OperatorState.INSTANCE;
            }

            public virtual BuilderState group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return StartGroup.INSTANCE;
            }

            public virtual BuilderState endgroup(Stacks stacks)
            {
                throw new IllegalStateException("Tying to end JQL sub-expression without completing 'NOT' operator. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual Clause build(Stacks stacks)
            {
                throw new IllegalStateException("Trying to end JQL expression with the 'NOT' operator. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual BuilderState copy(Stacks stacks)
            {
                return this;
            }

            public override string ToString()
            {
                return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
            }
        }

        /// <summary>
        /// This is the state of the builder once the user has enetered in a valid JQL clause. It is possible to:
        /// <ul>
        /// <li>Add a 'AND' operator.</li>
        /// <li>Add a 'OR' operator.</li>
        /// <li>Start a sub-expression.</li>
        /// <li>End a sub-expression.</li>
        /// <li>Build the JQL</li>
        /// <li>Add a clause if there is a default operator</li>
        /// </ul>
        /// 
        /// </summary>
        private class OperatorState : BuilderState
        {
            internal static readonly OperatorState INSTANCE = new OperatorState();

            internal OperatorState()
            {

            }

            public virtual BuilderState enter(Stacks stacks)
            {
                return this;
            }

            public virtual BuilderState not(Stacks stacks, BuilderOperator defaultOperator)
            {
                if (defaultOperator == null)
                {
                    throw new IllegalStateException("Trying to combine JQL expressions using the 'NOT' operator. The current JQL is '" + stacks.DisplayString + "'.");
                }
                else
                {
                    stacks.processAndPush(defaultOperator);
                    return NotState.INSTANCE;
                }
            }

            /// <summary>
            /// We now know that the next logical operator is going to be an AND. After this call completes, the user must
            /// enter either a clause, sub-clause or NOT clause so we transition into the {@link
            /// PrecedenceSimpleClauseBuilder.ClauseState}.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState and(Stacks stacks)
            {
                stacks.processAndPush(BuilderOperator.AND);
                return new ClauseState(BuilderOperator.AND);
            }

            /// <summary>
            /// We now know that the next logical operator is going to be an OR. After this call completes, the user must
            /// enter either a clause, sub-clause or NOT clause so we transition into the {@link
            /// PrecedenceSimpleClauseBuilder.ClauseState}.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState or(Stacks stacks)
            {
                stacks.processAndPush(BuilderOperator.OR);
                return new ClauseState(BuilderOperator.OR);
            }

            /// <summary>
            /// In this state we are able to add a clause only if a default operator has been specified.
            /// </summary>
            /// <param name="clause"> the clause to add to the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next state for the builder. </returns>
            public virtual BuilderState add(Stacks stacks, MutableClause clause, BuilderOperator defaultOperator)
            {
                if (defaultOperator == null)
                {
                    throw new IllegalStateException("Trying to combine JQL expressions without logical operator. The current JQL is '" + stacks.DisplayString + "'.");
                }

                stacks.processAndPush(defaultOperator);
                stacks.pushOperand(clause);
                return this;
            }

            public virtual BuilderState group(Stacks stacks, BuilderOperator defaultOperator)
            {
                if (defaultOperator == null)
                {
                    throw new IllegalStateException("Trying to combine JQL expressions without logical operator. The current JQL is '" + stacks.DisplayString + "'.");
                }
                else
                {
                    stacks.processAndPush(defaultOperator);
                    return StartGroup.INSTANCE;
                }
            }

            /// <summary>
            /// We are now going to try to end the current sub-expression. After this we transition back into this state
            /// awaing the next clause.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState endgroup(Stacks stacks)
            {
                //If this is true, then there is no current sub-expression.
                if (stacks.Level == 0)
                {
                    throw new IllegalStateException("Tyring end JQL sub-expression that does not exist. The current JQL is '" + stacks.DisplayString + "'.");
                }
                stacks.processAndPush(BuilderOperator.RPAREN);

                return this;
            }

            /// <summary>
            /// Try to build the JQL given the current state of the builder.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual Clause build(Stacks stacks)
            {
                //If this is true, then there are unfinished sub-expressions.
                if (stacks.Level > 0)
                {
                    throw new IllegalStateException("Tyring to build JQL expression that has an incomplete sub-expression. The current JQL is '" + stacks.DisplayString + "'.");
                }

                //we must take a copy of the stack to ensure that build does not destruct the builder.
                Stacks localStacks = new Stacks(stacks);
                return localStacks.asClause();
            }

            public virtual BuilderState copy(Stacks stacks)
            {
                return this;
            }

            public override string ToString()
            {
                return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
            }
        }

        /// <summary>
        /// This is the state of the builder when it is expecting a clause, sub-clause or a NOT clause. This is slightly
        /// different from the <seealso cref="PrecedenceSimpleClauseBuilder.StartState"/> as building is illegal and that this must
        /// happen after the bulider was in the <seealso cref="PrecedenceSimpleClauseBuilder.OperatorState"/>.
        /// </summary>
        private class ClauseState : BuilderState
        {
            internal readonly BuilderOperator lastOperator;

            public ClauseState(BuilderOperator lastOperator)
            {
                this.lastOperator = lastOperator;
            }

            public virtual BuilderState enter(Stacks stacks)
            {
                return this;
            }

            /// <summary>
            /// When NOT is called we transition to the <seealso cref="PrecedenceSimpleClauseBuilder.NotState"/> expecting a NOT clause
            /// to be added.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return NotState.INSTANCE;
            }

            public virtual BuilderState and(Stacks stacks)
            {
                throw new IllegalStateException(string.Format("Trying to create illegal JQL expression '{0} {1}'. Current JQL is '{2}'.", lastOperator, BuilderOperator.AND, stacks.DisplayString));
            }

            public virtual BuilderState or(Stacks stacks)
            {
                throw new IllegalStateException(string.Format("Trying to create illegal JQL expression '{0} {1}'. Current JQL is '{2}'.", lastOperator, BuilderOperator.OR, stacks.DisplayString));
            }

            /// <summary>
            /// When a clause is added an AND or OR operator is expected next so we transition into the {@link
            /// OperatorState}.
            /// </summary>
            /// <param name="clause"> the clause to add to the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next state for the builder. </returns>
            public virtual BuilderState add(Stacks stacks, MutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.pushOperand(clause);
                return OperatorState.INSTANCE;
            }

            /// <summary>
            /// We now expect a sub-clause, so lets transitiion into that state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return StartGroup.INSTANCE;
            }

            public virtual BuilderState endgroup(Stacks stacks)
            {
                throw new IllegalStateException(string.Format("Trying to create illegal JQL expression '{0} {1}'. Current JQL is '{2}'.", lastOperator, BuilderOperator.RPAREN, stacks.DisplayString));
            }

            public virtual Clause build(Stacks stacks)
            {
                throw new IllegalStateException(string.Format("Trying end the JQL expression with operator '{0}'. Current JQL is '{1}'.", lastOperator, stacks.DisplayString));
            }

            public virtual BuilderState copy(Stacks stacks)
            {
                return this;
            }

            public override string ToString()
            {
                return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
            }
        }

        /// <summary>
        /// This is the state for the builder when the user is expected to enter a sub-expression. From this state it is
        /// possible to:
        /// <p/>
        /// <ul> <li> Start a new sub-expression. <li> Add a new clause to the sub-expression. <li> Add a new NOT to the
        /// sub-expression. <ul>
        /// </summary>
        private class StartGroup : BuilderState
        {
            internal static readonly StartGroup INSTANCE = new StartGroup();

            internal StartGroup()
            {
            }

            /// <summary>
            /// Upon entry into this state we always start a new sub-expression.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState enter(Stacks stacks)
            {
                stacks.processAndPush(BuilderOperator.LPAREN);
                return this;
            }

            /// <summary>
            /// Starting a NOT clause will put us in the <seealso cref="PrecedenceSimpleClauseBuilder.NotState"/>.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return NotState.INSTANCE;
            }

            public virtual BuilderState and(Stacks stacks)
            {
                throw new IllegalStateException("Trying to start sub-expression with 'AND'. Current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual BuilderState or(Stacks stacks)
            {
                throw new IllegalStateException("Trying to start sub-expression with 'OR'. Current JQL is '" + stacks.DisplayString + "'.");
            }

            /// <summary>
            /// When a clause is added an AND or OR operator is expected next so we transition into the {@link
            /// OperatorState}.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="clause"> the clause to add to tbe builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState add(Stacks stacks, MutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.pushOperand(clause);
                return OperatorState.INSTANCE;
            }

            /// <summary>
            /// Starting a new group will renter this state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual BuilderState group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return this;
            }

            public virtual BuilderState endgroup(Stacks stacks)
            {
                throw new IllegalStateException("Trying to create empty sub-expression. Current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual Clause build(Stacks stacks)
            {
                throw new IllegalStateException("Tyring to build JQL expression that has an incomplete sub-expression. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual BuilderState copy(Stacks stacks)
            {
                return this;
            }

            public override string ToString()
            {
                return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
            }
        }

        ///CLOVER:OFF

        /// <summary>
        /// This is a guard state that can be used to stop calls to the builder when it is being initialised.
        /// 
        /// @since v4.0
        /// </summary>
        private class IllegalState : BuilderState
        {
            internal static readonly IllegalState INSTANCE = new IllegalState();

            public virtual BuilderState enter(Stacks stacks)
            {
                return this;
            }

            public virtual BuilderState not(Stacks stacks, BuilderOperator defaultOperator)
            {
                throw new IllegalStateException("Trying to access builder in illegal state.");
            }

            public virtual BuilderState and(Stacks stacks)
            {
                throw new IllegalStateException("Trying to add 'AND' to builder before it has been initialised.");
            }

            public virtual BuilderState or(Stacks stacks)
            {
                throw new IllegalStateException("Trying to add 'OR' to builder before it has been initialised.");
            }

            public virtual BuilderState add(Stacks stacks, MutableClause clause, BuilderOperator defaultOperator)
            {
                throw new IllegalStateException("Trying to add clause to builder before it has been initialised.");
            }

            public virtual BuilderState group(Stacks stacks, BuilderOperator defaultOperator)
            {
                throw new IllegalStateException("Trying to start sub-clause in a builder that has not been initialised.");
            }

            public virtual BuilderState endgroup(Stacks stacks)
            {
                throw new IllegalStateException("Trying to end sub-clause in a builder that has not been initialised.");
            }

            public virtual Clause build(Stacks stacks)
            {
                throw new IllegalStateException("Trying to call build before the builder is initialised.");
            }

            public virtual BuilderState copy(Stacks stacks)
            {
                throw new IllegalStateException("Trying to copy a builder that has not been initialised.");
            }

            public override string ToString()
            {
                return "Illegal State";
            }
        }

        ///CLOVER:ON

        /// <summary>
        /// An iterator that does decorates a <seealso cref="java.util.ListIterator"/> such that it appears reversed and infinite. The
        /// iterator will return all the values of the wrapped iterator up util it ends. Once it ends this iterator will
        /// continue to return null.
        /// </summary>
        private class InfiniteReversedIterator<T> : IEnumerator<T>
        {
            internal IEnumerator<T> @delegate;

            public InfiniteReversedIterator(IEnumerator<T> @delegate)
            {
                this.@delegate = @delegate;
            }

            public virtual bool hasNext()
            {
                return true;
            }

            public virtual T next()
            {
                if (@delegate.hasPrevious())
                {
                    return @delegate.previous();
                }
                return null;
            }

            public virtual void remove()
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public T Current
            {
                get { throw new NotImplementedException(); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        /// <summary>
        /// Simple visitor that will returns the operator associated with the root node of the passed clause. 
        /// May return null when the root operator has no operator.
        /// </summary>
        private class OperatorVisitor : IClauseVisitor<BuilderOperator>
        {
            public static BuilderOperator FindOperator(IClause clause)
            {
                var visitor = new OperatorVisitor();
                return clause.Accept(visitor);
            }

            public BuilderOperator Visit(AndClause andClause)
            {
                return BuilderOperator.AND;
            }

            public BuilderOperator Visit(NotClause notClause)
            {
                return BuilderOperator.NOT;
            }

            public BuilderOperator Visit(OrClause orClause)
            {
                return BuilderOperator.OR;
            }

            public BuilderOperator Visit(ITerminalClause clause)
            {
                //return null;
                throw new NotImplementedException();
            }

            public BuilderOperator Visit(IWasClause clause)
            {
                //return null;
                throw new NotImplementedException();
            }

            public BuilderOperator Visit(IChangedClause clause)
            {
                //return null;
                throw new NotImplementedException();
            }
        }
    }
}
