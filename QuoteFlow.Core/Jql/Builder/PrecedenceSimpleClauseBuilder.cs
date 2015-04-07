using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// An implementation of <see cref="ISimpleClauseBuilder"/> that takes JQL prededence into account when building its associated
    /// JQL <see cref="IClause"/>. For exmaple, the expression {@code
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
            this._builderState = StartState.INSTANCE.Enter(_stacks);
            this._defaultOperator = BuilderOperator.None;
        }

        private PrecedenceSimpleClauseBuilder(PrecedenceSimpleClauseBuilder copy)
        {
            this._builderState = IllegalState.INSTANCE;
            this._stacks = new Stacks(copy._stacks);
            this._builderState = copy._builderState.Copy(this._stacks);
            this._defaultOperator = copy._defaultOperator;
        }

        public ISimpleClauseBuilder Clear()
        {
            _defaultOperator = BuilderOperator.None;
            _stacks.Clear();
            _builderState = StartState.INSTANCE.Enter(_stacks);
            
            return this;
        }

        public ISimpleClauseBuilder And()
        {
            _builderState = _builderState.And(_stacks).Enter(_stacks);
            return this;
        }

        public ISimpleClauseBuilder Or()
        {
            _builderState = _builderState.Or(_stacks).Enter(_stacks);
            return this;
        }

        public ISimpleClauseBuilder Not()
        {
            _builderState = _builderState.Not(_stacks, _defaultOperator).Enter(_stacks);
            return this;
        }

        public ISimpleClauseBuilder Clause(IClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException("clause");
            }

            _builderState = _builderState.Add(_stacks, new SingleMutableClause(clause), _defaultOperator).Enter(_stacks);
            return this;
        }

        public ISimpleClauseBuilder Sub()
        {
            _builderState = _builderState.Group(_stacks, _defaultOperator).Enter(_stacks);
            return this;
        }

        public ISimpleClauseBuilder Endsub()
        {
            _builderState = _builderState.Endgroup(_stacks).Enter(_stacks);
            return this;
        }

        public IClause Build()
        {
            return _builderState.Build(_stacks);
        }

        public ISimpleClauseBuilder Copy()
        {
            return new PrecedenceSimpleClauseBuilder(this);
        }

        public ISimpleClauseBuilder DefaultAnd()
        {
            _defaultOperator = BuilderOperator.AND;
            return this;
        }

        public ISimpleClauseBuilder DefaultOr()
        {
            _defaultOperator = BuilderOperator.OR;
            return this;
        }

        public ISimpleClauseBuilder DefaultNone()
        {
            _defaultOperator = BuilderOperator.None;
            return this;
        }

        public override string ToString()
        {
            return _stacks.DisplayString;
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

            private BuilderOperator PopOperator()
            {
                BuilderOperator builderOperator = Operators.First();
                Operators.RemoveFirst();
                if (builderOperator == BuilderOperator.LPAREN)
                {
                    Level = Level - 1;
                }
                return builderOperator;
            }

            private BuilderOperator PeekOperator()
            {
                return Operators.ElementAt(0);
            }

            private bool HasOperator()
            {
                return Operators.Any();
            }

            internal void PushOperand(IMutableClause operand)
            {
                Operands.AddFirst(operand);
            }

            private IMutableClause PopClause()
            {
                if (!Operands.Any()) return null;

                var op = Operands.First();
                Operands.Remove(op);
                return op;
            }

            private IMutableClause PeekClause()
            {
                return Operands.ElementAt(0);
            }

            /// <summary>
            /// Process the current operators on the operator stack using the Shunting Yard Algorithm and then push the
            /// passed operator onto the operator stack. Passing null to this method will leave the complete MutableClause as
            /// the only argument on the operand stack.
            /// </summary>
            /// <param name="op">The operator to add to the stack. A <code>null</code> argument means that the operator stack should be emptied.</param>
            internal void ProcessAndPush(BuilderOperator op)
            {
                if (op != BuilderOperator.LPAREN && HasOperator())
                {
                    BuilderOperator currentTop = PeekOperator();

                    // The NOT operator is special since it a right-associative unary operator. For right-associative
                    // operators only process when something of higher but not equal precedence appears on the stack.
                    int compare = op == BuilderOperator.NOT ? -1 : 0;

                    while (currentTop != BuilderOperator.None && (op == BuilderOperator.None || op.CompareTo(currentTop) <= compare))
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

                        currentTop = HasOperator() ? PeekOperator() : BuilderOperator.None;
                    }
                }

                if (op == BuilderOperator.None) return;

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

            /// <summary>
            /// Get the, possibly partial, JQL expression for the current stack state. Used mainly for error messages.
            /// </summary>
            /// <returns>The current JQL expression given the state of the stacks.</returns>
            internal string DisplayString
            {
                get
                {
                    var operatorIterator = Operators.Reverse().GetEnumerator();
                    var clauseIterator = Operands.Reverse().GetEnumerator();
//                    IEnumerator<BuilderOperator> operatorIterator = new InfiniteReversedIterator<BuilderOperator>(Operators.GetEnumerator());
//                    IEnumerator<IMutableClause> clauseIterator = new InfiniteReversedIterator<IMutableClause>(Operands.GetEnumerator());
                    StringBuilder stringBuilder = new StringBuilder();

                    bool start = true;

                    BuilderOperator op = BuilderOperator.None;
                    if (operatorIterator.MoveNext())
                    {
                         op = operatorIterator.Current;
                    }

                    while (op != BuilderOperator.None)
                    {
                        switch (op)
                        {
                            case BuilderOperator.LPAREN:
                                AddString(stringBuilder, "(");
                                op = operatorIterator.MoveNext() ? operatorIterator.Current : BuilderOperator.None;
                                start = true;
                                break;
                            case BuilderOperator.RPAREN:
                                // just append these where seen.
                                stringBuilder.Append(")");
                                op = operatorIterator.MoveNext() ? operatorIterator.Current : BuilderOperator.None;
                                start = false;
                                break;
                            case BuilderOperator.AND:
                            case BuilderOperator.OR:
                                // do we need to add the starting operand for this operator. We only do this for new expressions.
                                if (start)
                                {
                                    if (clauseIterator.MoveNext())
                                    {
                                        AddString(stringBuilder, ClauseToString(clauseIterator.Current, op));
                                    }
                                }

                                // Fall through here on purpose.
                                goto case BuilderOperator.NOT;
                            case BuilderOperator.NOT:
                                AddString(stringBuilder, op.ToString());

                                var nextOperator = BuilderOperator.None;
                                if (operatorIterator.MoveNext())
                                {
                                    nextOperator = operatorIterator.Current;

                                    // We don't want to print the next operand yet for these two operators.
                                    if (nextOperator != BuilderOperator.LPAREN && nextOperator != BuilderOperator.NOT)
                                    {
                                        if (clauseIterator.MoveNext())
                                        {
                                            AddString(stringBuilder, ClauseToString(clauseIterator.Current, op));
                                        }
                                    }
                                }

                                start = false;
                                op = nextOperator;

                                break;
                        }
                    }

                    // Loop through any remaining operands and add them. There should only ever be one.
                    if (clauseIterator.MoveNext())
                    {
                        IMutableClause clause = clauseIterator.Current;
                        while (clause != null)
                        {
                            AddString(stringBuilder, ClauseToString(clause, BuilderOperator.None));

                            clause = operatorIterator.MoveNext() ? clauseIterator.Current : null;
                        }
                    }

                    return stringBuilder.ToString();
                }
            }

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

                // We need to bracket the clause if its primary operator has lower precedence than the passed operator.
                BuilderOperator clauseOperator = OperatorVisitor.FindOperator(jqlClause);
                if (op != BuilderOperator.None && clauseOperator != BuilderOperator.None && op.CompareTo(clauseOperator) > 0)
                {
                    return "(" + jqlClause.ToString() + ")";
                }

                return jqlClause.ToString();
            }

            public IClause AsClause()
            {
                return AsMutableClause().AsClause();
            }

            private IMutableClause AsMutableClause()
            {
                ProcessAndPush(BuilderOperator.None);
                return PeekClause();
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
        /// </summary>
        private class StartState : IBuilderState
        {
            internal static readonly StartState INSTANCE = new StartState();

            private StartState()
            {
            }

            public virtual IBuilderState Enter(Stacks stacks)
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
            public virtual IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return NotState.Instance;
            }

            public virtual IBuilderState And(Stacks stacks)
            {
                return this;
            }

            public virtual IBuilderState Or(Stacks stacks)
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
            public virtual IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.PushOperand(clause);
                return OperatorState.INSTANCE;
            }

            /// <summary>
            /// We now have a sub-expression so lets transition into that state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return StartGroup.Instance;
            }

            public virtual IBuilderState Endgroup(Stacks stacks)
            {
                throw new InvalidOperationException("Tying to start JQL expression with ')'.");
            }

            public virtual IClause Build(Stacks stacks)
            {
                return null;
            }

            public virtual IBuilderState Copy(Stacks stacks)
            {
                return this;
            }
        }

        /// <summary>
        /// This is the state of the builder when the user is trying to enter in a negated clause. 
        /// In this state it is only possible to add a clause, start a sub-expression or negate again.
        /// </summary>
        private class NotState : IBuilderState
        {
            internal static readonly NotState Instance = new NotState();

            private NotState()
            {
            }

            /// <summary>
            /// Upon entry into this state, we always add a not operator.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState Enter(Stacks stacks)
            {
                stacks.ProcessAndPush(BuilderOperator.NOT);
                return this;
            }

            /// <summary>
            /// We are negating the clause again, so re-enter our state.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <param name="defaultOperator">The default combining operator.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return this;
            }

            public virtual IBuilderState And(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to create the illegal JQL expression 'NOT AND'. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual IBuilderState Or(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to create the illegal JQL expression 'NOT OR'. The current JQL is '" + stacks.DisplayString + "'.");
            }

            /// <summary>
            /// Add the clause to the operand stack so that it may be negated.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <param name="clause">The clause to add to the query.</param>
            /// <param name="defaultOperator">The default combining operator.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.PushOperand(clause);
                return OperatorState.INSTANCE;
            }

            public virtual IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return StartGroup.Instance;
            }

            public virtual IBuilderState Endgroup(Stacks stacks)
            {
                throw new InvalidOperationException("Tying to end JQL sub-expression without completing 'NOT' operator. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual IClause Build(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to end JQL expression with the 'NOT' operator. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual IBuilderState Copy(Stacks stacks)
            {
                return this;
            }
        }

        /// <summary>
        /// This is the state of the builder once the user has enetered in a valid JQL clause. 
        /// It is possible to:
        /// 
        /// - Add a 'AND' operator.
        /// - Add a 'OR' operator.
        /// - Start a sub-expression.
        /// - End a sub-expression.
        /// - Build the JQL
        /// - Add a clause if there is a default operator
        /// </summary>
        private class OperatorState : IBuilderState
        {
            internal static readonly OperatorState INSTANCE = new OperatorState();

            private OperatorState()
            {
            }

            public virtual IBuilderState Enter(Stacks stacks)
            {
                return this;
            }

            public virtual IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator)
            {
                if (defaultOperator == BuilderOperator.None)
                {
                    throw new InvalidOperationException("Trying to combine JQL expressions using the 'NOT' operator. The current JQL is '" + stacks.DisplayString + "'.");
                }

                stacks.ProcessAndPush(defaultOperator);
                return NotState.Instance;
            }

            /// <summary>
            /// We now know that the next logical operator is going to be an AND. After this call completes, the user must
            /// enter either a clause, sub-clause or NOT clause so we transition into the {@link
            /// PrecedenceSimpleClauseBuilder.ClauseState}.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState And(Stacks stacks)
            {
                stacks.ProcessAndPush(BuilderOperator.AND);
                return new ClauseState(BuilderOperator.AND);
            }

            /// <summary>
            /// We now know that the next logical operator is going to be an OR. After this call completes, the user must
            /// enter either a clause, sub-clause or NOT clause so we transition into the {@link
            /// PrecedenceSimpleClauseBuilder.ClauseState}.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState Or(Stacks stacks)
            {
                stacks.ProcessAndPush(BuilderOperator.OR);
                return new ClauseState(BuilderOperator.OR);
            }

            /// <summary>
            /// In this state we are able to add a clause only if a default operator has been specified.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <param name="clause">The clause to add to the builder.</param>
            /// <param name="defaultOperator">The default combining operator.</param>
            /// <returns>The next state for the builder.</returns>
            public virtual IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator)
            {
                if (defaultOperator == BuilderOperator.None)
                {
                    throw new InvalidOperationException("Trying to combine JQL expressions without logical operator. The current JQL is '" + stacks.DisplayString + "'.");
                }

                stacks.ProcessAndPush(defaultOperator);
                stacks.PushOperand(clause);
                return this;
            }

            public virtual IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator)
            {
                if (defaultOperator == BuilderOperator.None)
                {
                    throw new InvalidOperationException("Trying to combine JQL expressions without logical operator. The current JQL is '" + stacks.DisplayString + "'.");
                }
                
                stacks.ProcessAndPush(defaultOperator);
                return StartGroup.Instance;
            }

            /// <summary>
            /// We are now going to try to end the current sub-expression. After this we transition back into this state
            /// awaing the next clause.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState Endgroup(Stacks stacks)
            {
                //If this is true, then there is no current sub-expression.
                if (stacks.Level == 0)
                {
                    throw new InvalidOperationException("Tyring end JQL sub-expression that does not exist. The current JQL is '" + stacks.DisplayString + "'.");
                }

                stacks.ProcessAndPush(BuilderOperator.RPAREN);
                return this;
            }

            /// <summary>
            /// Try to build the JQL given the current state of the builder.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <returns>The next builder state.</returns>
            public virtual IClause Build(Stacks stacks)
            {
                //If this is true, then there are unfinished sub-expressions.
                if (stacks.Level > 0)
                {
                    throw new InvalidOperationException("Tyring to build JQL expression that has an incomplete sub-expression. The current JQL is '" + stacks.DisplayString + "'.");
                }

                //we must take a copy of the stack to ensure that build does not destruct the builder.
                Stacks localStacks = new Stacks(stacks);
                return localStacks.AsClause();
            }

            public virtual IBuilderState Copy(Stacks stacks)
            {
                return this;
            }
        }

        /// <summary>
        /// This is the state of the builder when it is expecting a clause, sub-clause or a NOT clause. This is slightly
        /// different from the <seealso cref="PrecedenceSimpleClauseBuilder.StartState"/> as building is illegal and that this must
        /// happen after the bulider was in the <seealso cref="PrecedenceSimpleClauseBuilder.OperatorState"/>.
        /// </summary>
        private class ClauseState : IBuilderState
        {
            private readonly BuilderOperator _lastOperator;

            public ClauseState(BuilderOperator lastOperator)
            {
                _lastOperator = lastOperator;
            }

            public virtual IBuilderState Enter(Stacks stacks)
            {
                return this;
            }

            /// <summary>
            /// When NOT is called we transition to the <see cref="PrecedenceSimpleClauseBuilder.NotState"/> expecting 
            /// a NOT clause to be added.
            /// </summary>
            /// <param name="stacks">Current stacks for the builder.</param>
            /// <param name="defaultOperator">The default combining operator.</param>
            /// <returns>The next builder state.</returns>
            public virtual IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return NotState.Instance;
            }

            public virtual IBuilderState And(Stacks stacks)
            {
                throw new InvalidOperationException(string.Format("Trying to create illegal JQL expression '{0} {1}'. Current JQL is '{2}'.", _lastOperator, BuilderOperator.AND, stacks.DisplayString));
            }

            public virtual IBuilderState Or(Stacks stacks)
            {
                throw new InvalidOperationException(string.Format("Trying to create illegal JQL expression '{0} {1}'. Current JQL is '{2}'.", _lastOperator, BuilderOperator.OR, stacks.DisplayString));
            }

            /// <summary>
            /// When a clause is added an AND or OR operator is expected next so we transition into the {@link
            /// OperatorState}.
            /// </summary>
            /// <param name="stacks">The current stacks for the builder.</param>
            /// <param name="clause">The clause to add to the builder.</param>
            /// <param name="defaultOperator">The default combining operator.</param>
            /// <returns>The next state for the builder.</returns>
            public virtual IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.PushOperand(clause);
                return OperatorState.INSTANCE;
            }

            /// <summary>
            /// We now expect a sub-clause, so lets transitiion into that state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return StartGroup.Instance;
            }

            public virtual IBuilderState Endgroup(Stacks stacks)
            {
                throw new InvalidOperationException(string.Format("Trying to create illegal JQL expression '{0} {1}'. Current JQL is '{2}'.", _lastOperator, BuilderOperator.RPAREN, stacks.DisplayString));
            }

            public virtual IClause Build(Stacks stacks)
            {
                throw new InvalidOperationException(string.Format("Trying end the JQL expression with operator '{0}'. Current JQL is '{1}'.", _lastOperator, stacks.DisplayString));
            }

            public virtual IBuilderState Copy(Stacks stacks)
            {
                return this;
            }
        }

        /// <summary>
        /// This is the state for the builder when the user is expected to enter a sub-expression. 
        /// From this state it is possible to:
        /// 
        /// - Start a new sub-expression. 
        /// - Add a new clause to the sub-expression. 
        /// - Add a new NOT to the sub-expression.
        /// </summary>
        private class StartGroup : IBuilderState
        {
            internal static readonly StartGroup Instance = new StartGroup();

            internal StartGroup()
            {
            }

            /// <summary>
            /// Upon entry into this state we always start a new sub-expression.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <returns> the next builder state. </returns>
            public virtual IBuilderState Enter(Stacks stacks)
            {
                stacks.ProcessAndPush(BuilderOperator.LPAREN);
                return this;
            }

            /// <summary>
            /// Starting a NOT clause will put us in the <seealso cref="PrecedenceSimpleClauseBuilder.NotState"/>.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator)
            {
                return NotState.Instance;
            }

            public virtual IBuilderState And(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to start sub-expression with 'AND'. Current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual IBuilderState Or(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to start sub-expression with 'OR'. Current JQL is '" + stacks.DisplayString + "'.");
            }

            /// <summary>
            /// When a clause is added an AND or OR operator is expected next so we transition into the {@link
            /// OperatorState}.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="clause"> the clause to add to tbe builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator)
            {
                stacks.PushOperand(clause);
                return OperatorState.INSTANCE;
            }

            /// <summary>
            /// Starting a new group will renter this state.
            /// </summary>
            /// <param name="stacks"> current stacks for the builder. </param>
            /// <param name="defaultOperator"> the default combining operator. </param>
            /// <returns> the next builder state. </returns>
            public virtual IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator)
            {
                return this;
            }

            public virtual IBuilderState Endgroup(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to create empty sub-expression. Current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual IClause Build(Stacks stacks)
            {
                throw new InvalidOperationException("Tyring to build JQL expression that has an incomplete sub-expression. The current JQL is '" + stacks.DisplayString + "'.");
            }

            public virtual IBuilderState Copy(Stacks stacks)
            {
                return this;
            }
        }

        /// <summary>
        /// This is a guard state that can be used to stop calls to the builder when it is being initialised.
        /// </summary>
        private class IllegalState : IBuilderState
        {
            internal static readonly IllegalState INSTANCE = new IllegalState();

            public virtual IBuilderState Enter(Stacks stacks)
            {
                return this;
            }

            public virtual IBuilderState Not(Stacks stacks, BuilderOperator defaultOperator)
            {
                throw new InvalidOperationException("Trying to access builder in illegal state.");
            }

            public virtual IBuilderState And(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to add 'AND' to builder before it has been initialised.");
            }

            public virtual IBuilderState Or(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to add 'OR' to builder before it has been initialised.");
            }

            public virtual IBuilderState Add(Stacks stacks, IMutableClause clause, BuilderOperator defaultOperator)
            {
                throw new InvalidOperationException("Trying to add clause to builder before it has been initialised.");
            }

            public virtual IBuilderState Group(Stacks stacks, BuilderOperator defaultOperator)
            {
                throw new InvalidOperationException("Trying to start sub-clause in a builder that has not been initialised.");
            }

            public virtual IBuilderState Endgroup(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to end sub-clause in a builder that has not been initialised.");
            }

            public virtual IClause Build(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to call build before the builder is initialised.");
            }

            public virtual IBuilderState Copy(Stacks stacks)
            {
                throw new InvalidOperationException("Trying to copy a builder that has not been initialised.");
            }

            public override string ToString()
            {
                return "Illegal State";
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
                return BuilderOperator.None;
            }

            public BuilderOperator Visit(IWasClause clause)
            {
                return BuilderOperator.None;
            }

            public BuilderOperator Visit(IChangedClause clause)
            {
                return BuilderOperator.None;
            }
        }
    }
}
