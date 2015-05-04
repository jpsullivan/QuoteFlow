using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// A builder that can create JQL clauses by combining other clauses with logical operators. This builder's interface is
    /// very low level and is designed to be built upon by other builders to present a better interface.
    /// 
    /// The builder is designed to be used in a fluenet style. For example the JQL <tt>clause1 and clause2 or clause3</tt>
    /// can be build using {@code new BasicJqlBuilder().add(clause1).and().add(clause2).or().add(clause3).build()}.
    /// 
    /// It is the caller's responsibility to ensure that they call the methods in an order that will generate valid JQL. The
    /// builder will throw an <see cref="IllegalStateException"/> if an attempt is made to generate invalid JQL. For example, a call to
    /// {@code builder.clause(clause1).clause(clause2)} will fail since there was no operator placed between the
    /// clauses.
    /// 
    /// The builder can be told to inject automatically either an "AND" (<see cref="#defaultAnd()"/>) or an "OR" (<see cref="#defaultOr()"/>)
    /// between clauses when no operator have been specified. For example, a call to {@code builder.defaultAnd().clause(clause1).clause(clause2).build()}
    /// will actually generate the JQL {@code clause1 AND clause2}. The affect of calling either {@code defaultAnd()} or {@code defaultOr()} will
    /// remain in place on the builder until one of <see cref="#defaultNone()"/>, {@code defaultAnd()}, {@code defaultOr()} or <see cref="#clear()"/> is called.
    /// 
    /// The builder may handle the precedence in JQL in different ways. For instance, {@code
    /// builder.clause(clause1).or().clause(clause2).and().clause(clause3).build()} could create a JQL expression
    /// <tt>(clause1 or clause2) and clause3</tt> if JQL precedence is ignored or <tt>clause1 or (clause2 and clause3)</tt>
    /// if it takes precedence into account. How precedence is handled is left as an implementation detail.
    /// </summary>
    public interface ISimpleClauseBuilder
    {
        /// <summary>
        /// Reset the builder to its empty initial state. 
        /// </summary>
        /// <returns> the reset builder. </returns>
        ISimpleClauseBuilder Clear();

        /// <summary>
        /// Add a new logical AND operator to the JQL expression being built.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder And();

        /// <summary>
        /// Add a new logical OR operator to the JQL expression being built.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder Or();

        /// <summary>
        /// Add a new logical NOT operator to JQL expression being built.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder Not();

        /// <summary>
        /// Add the passed clause to the JQL expression being built.
        /// </summary>
        /// <param name="clause"> the clause to add to the current JQL expression. </param>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder Clause(IClause clause);

        /// <summary>
        /// Start a new sub-expression in the JQL expression being built. This can be used to override any precendece rules
        /// implemented in the builder.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder Sub();

        /// <summary>
        /// End the current sub-expression in the JQL expression being built.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder Endsub();

        /// <summary>
        /// Create a new <see cref="QuoteFlow.Core.Jql.Clause"/> for the JQL the builder has been constructing. A <code>null</code> value may be
        /// returned to indicate that there is no condition is generate.
        /// <p/>
        /// A call to build is non destructive and the builder may continue to be used after it is called.
        /// </summary>
        /// <returns> the generated clause or <code>null</code> if there was not clause to generated. </returns>
        IClause Build();

        /// <summary>
        /// Create a copy of this builder.
        /// </summary>
        /// <returns> a new copy of this builder. </returns>
        ISimpleClauseBuilder Copy();

        /// <summary>
        /// Tell the builder to combine clauses using the "AND" JQL condition when none has been specified. Normally the
        /// caller must ensure that a call to either <see cref="#and()"/> or <see cref="#or()"/> is placed between calls to {@link
        /// #clause(com.atlassian.query.clause.Clause)} to ensure that valid JQL is built (and that no {@link
        /// IllegalStateException} is thrown). Calling this method on the builder tells it to automatically add a JQL "AND"
        /// between JQL clauses when no calls to either {@code and} or {@code or} have been made. This mode will remain
        /// active until one of <see cref="#defaultNone()"/>, {@code defaultOr()} or <see cref="#clear()"/> is called.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder DefaultAnd();

        /// <summary>
        /// Tell the builder to combine clauses using the "OR" JQL condition when none has been specified. Normally the
        /// caller must ensure that a call to either <see cref="#and()"/> or <see cref="#or()"/> is placed between calls to {@link
        /// #clause(com.atlassian.query.clause.Clause)} to ensure that valid JQL is built (and that no {@link
        /// IllegalStateException} is thrown). Calling this method on the builder tells it to automatically add a JQL "OR"
        /// between JQL clauses when no calls to either {@code and} or {@code or} have been made. This mode will remain
        /// active until one of <see cref="#defaultNone()"/>, {@code defaultAnd()} or <see cref="#clear()"/> is called.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder DefaultOr();

        /// <summary>
        /// Tell the builder to stop injecting JQL "AND" or "OR" operators automatically between calls to {@link
        /// #clause(com.atlassian.query.clause.Clause)}. This essentially turns off the behaviour started by calling either
        /// <see cref="#defaultAnd()"/> or <see cref="#defaultOr()"/>.
        /// </summary>
        /// <returns> a builder that can be used to further extends the current JQL expression. </returns>
        ISimpleClauseBuilder DefaultNone();
    }

}