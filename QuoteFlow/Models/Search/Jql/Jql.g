grammar Jql;

options { language=CSharp3; }

@parser::namespace { QuoteFlow.Models.Search.Jql.AntlrGen }
@lexer::namespace { QuoteFlow.Models.Search.Jql.AntlrGen }

@parser::header {
using System;
using System.Collections;
using System.Linq;
using Antlr.Runtime;
using QuoteFlow.Infrastructure.Exceptions;
using QuoteFlow.Infrastructure.Exceptions.Antlr;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.History;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Models.Search.Jql.Query.Order;
using QuoteFlow.Models.Search.Jql.Util;
}

/* START HACK
 * The following code will basically tell ANTLR to freak out immediately on error rather than trying to recover. 
 * This seems to be a hack as the ANTLR runtime seems to change quite frequently between releases. For instance, 
 * the ANTLRv3 book tells us to overwrite the wrong methods.
 */
@members {
	private int operandLevel = 0;
	private bool supportsHistoryPredicate = false;

	protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
    {
        throw new MismatchedTokenException(ttype, input);
    }

    public override object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
    {
        throw e;
    }

    public override void EmitErrorMessage(string msg)
    {
        base.EmitErrorMessage(msg);
    }

    /// <summary>
    /// Make sure that the passed token can be turned into a long. In ANTLR there
    /// does not appear to be an easy way to limit numbers to a valid Long range, so
    /// lets do so in Java.
    /// </summary>
    /// <param name="token"> the token to turn into a long. </param>
    /// <returns> the valid long. </returns>
    private long ParseLong(IToken token)
    {
        string text = token.Text;
        try
        {
            return Convert.ToInt64(text);
        }
        catch (Exception e)
        {
            var message = JqlParseErrorMessages.IllegalNumber(text, token.Line, token.CharPositionInLine);
            throw new RuntimeRecognitionException(message, e);
        }
    }

    private string CheckFieldName(IToken token)
    {
        string text = token.Text;
        if (text.IsNullOrEmpty())
        {
            ReportError(JqlParseErrorMessages.EmptyFieldName(token.Line, token.CharPositionInLine), null);
        }
        return text;
    }

    private string CheckFunctionName(IToken token)
    {
        string text = token.Text;
        if (text.IsNullOrEmpty())
        {
            ReportError(JqlParseErrorMessages.EmptyFunctionName(token.Line, token.CharPositionInLine), null);
        }
        return text;
    }

    private void ReportError(JqlParseErrorMessage message, Exception th)
    {
        throw new RuntimeRecognitionException(message, th);
    }

}

/*
 * END HACK
 */

@lexer::header {
using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using QuoteFlow.Infrastructure.Exceptions.Antlr;
using QuoteFlow.Infrastructure.Helpers;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Util;
}

@lexer::members {
	private LinkedList<AntlrPosition> Stack { get; set; }

    private void StripAndSet()
    {
        string text = Text;
        text = text.Substring(1, text.Length - 1 - 1);
        text = JqlStringSupport.Decode(text);
        Text = text;
    }

    private void CheckAndSet()
    {
        string text = JqlStringSupport.Decode(Text);
        if (JqlStringSupport.IsReservedString(text))
        {
            var message = JqlParseErrorMessages.ReservedWord(text, state.tokenStartLine, state.tokenStartCharPositionInLine);
            throw new RuntimeRecognitionException(message);
        }
        Text = text;
    }

    public override void EmitErrorMessage(string msg)
    {
        base.EmitErrorMessage(msg);
    }

    /// <summary>
    /// START HACK 
    /// We need to get the "lexer" to fail when it detects errors. At the moment ANTLR just drops the input
    /// up until the error and tries again. This can leave antlr actually parsing JQL strings that are not
    /// valid. For example, the string "priority = \kbjb" will actually be parsed as "priority = bjb". 
    /// To stop this we throw a RuntimeRecognitionRuntimeException which the DefaultJqlParser is careful to catch. 
    /// Throwing the RecognitionException  will not work as JqlLexer.nextToken catches this exception and tries 
    /// again (which will cause an infinite loop).
    /// 
    /// Antlr (check up to 3.1.3) does not seem to be able to handle the "catch" clause on lexer rules. It throws a RuntimeException
    /// when trying to process the grammar. To get around this we have hacked the error reporting using a "stack" to push
    /// on the rule we currently have an error in. We can use this information to produce a pretty good error message when
    /// the lexer tries to recover though is does make for some pretty strange logic.
    /// </summary>
    /// <param name="re"></param>
    public override void Recover(RecognitionException re)
    {
        var handler = new LexerErrorHelper(input, PeekPosition());
        handler.HandleError(re);
    }

    private void Recover()
    {
        var e = new MismatchedSetException(null, input);
        Recover(e);
    }

    private void PushPosition(int tokenType)
    {
        Stack.AddLast(new AntlrPosition(tokenType, input));
    }

    private void PopPosition()
    {
        if (Stack.Count != 0) Stack.RemoveFirst();
    }

    private AntlrPosition PeekPosition()
    {
        return Stack.Count == 0 ? null : Stack.ElementAt(0);
    }

    public override void mTokens()
    {
    }
}

query returns [IClause clause, IOrderBy order]
	: (where = clause)?  (sort = orderBy)? EOF
	{
		$clause = $where.clause;
		$order = $sort.order;
	}
	;
	catch [MismatchedTokenException e]
	{
	    if (e.Expecting == EOF)
        {
            if (sort != null)
            {
                //If the sort has trailing tokens then "," must be the next token as this is the only way to
                //continue a sort.
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, ","), e);
            }
            else if (where == null)
            {
                //If there is no where clause, then we were not able to find a field name. If we found a field
                //name we would have found a clause and got an error there.
                ReportError(JqlParseErrorMessages.BadFieldName(e.Token), e);
            }
            else
            {
                //If we get a clause but have other stuff after, then the next token must be an "AND" or "OR" as this
                //is the only valid way to combine two clauses.
                ReportError(JqlParseErrorMessages.NeedLogicalOperator(e.Token), e);
            }
        }
	    ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
	}
	catch [RecognitionException e]
	{
	    ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
	}

/*
 * Represents a JQL clause.
 */
clause	returns [IClause clause]
	: orClause { $clause = $orClause.clause; }
	;

/*
 * Represents a set of 1..* JQL clauses combined by an 'OR'. 
 */
orClause returns [IClause clause]
	@init {
		var clauses = new List<IClause>();
	}
	// a clause is composed of 'AND' clauses since they have higher precedence than the 'OR' composition
	: cl = andClause { clauses.Add($cl.clause); } (OR cl = andClause { clauses.Add($cl.clause); })* {$clause = clauses.Count == 1? clauses.ElementAt(0) : new OrClause(clauses); }
	;

/*
 * Represents a set of 1..* JQL clauses combined by an 'AND'.
 */
andClause returns [IClause clause]
	@init {
		var clauses = new List<IClause>();
	}
	: cl = notClause { clauses.Add($cl.clause); } (AND cl = notClause { clauses.Add($cl.clause); })* {$clause = clauses.Count == 1? clauses.ElementAt(0) : new AndClause(clauses); }
	;

/*
 * Represents a JQL 'terminalClause', negated 'terminalClause' or bracketed expression.
 */
notClause returns [IClause clause]
	: (NOT | BANG) nc = notClause { $clause = new NotClause($nc.clause); }
	| subClause { $clause = $subClause.clause; }
                      | terminalClause { $clause = $terminalClause.clause; } 
	;
	catch [NoViableAltException e]
	{
	    //If there is no option here then we assume that the user meant a field expression.
	    ReportError(JqlParseErrorMessages.BadFieldName(e.Token), e);
	}


/*
 * Represents a bracketed JQL expression.
 */
subClause returns [IClause clause]
    : LPAREN orClause RPAREN { $clause = $orClause.clause; }
    ;
    catch [MismatchedTokenException e]
    {
        if (e.Expecting == RPAREN)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, ")"), e);
        }
        else
        {
            throw e;
        }
    }

/*
 * Represents a JQL termincalClause or matched brackets.
 */
terminalClause  returns [IClause clause]
	: f = field op = operator  ( (opand = operand pred= historyPredicate?) | chPred=historyPredicate?)
	{
        if (f != null && $f.field.IsEntityProperty())
        {
            if ($operand.operand == null)
            {
                var e = new RecognitionException(input);
                ReportError(JqlParseErrorMessages.BadOperand(e.Token), e);
            }
            $clause = new TerminalClause($f.field.Name, $operator.operator, $operand.operand, new List<Property>{$f.field.Property});
        }
	    else if ($operator.operator  == Operator.CHANGED)
	    {
            if ($operand.operand != null)
            {
                var e = new RecognitionException(input);
                ReportError(JqlParseErrorMessages.UnsupportedOperand($operator.operator.ToString(), $operand.operand.DisplayString),e);
            }
            $clause = new ChangedClause($field.field.Name, $operator.operator, $chPred.predicate);
	    }
	    else
	    {
	       if ($operator.operator == Operator.WAS || $operator.operator == Operator.WAS_NOT || $operator.operator == Operator.WAS_IN || $operator.operator == Operator.WAS_NOT_IN )
		   {
		        if ($operand.operand == null)
                {
                    var e = new RecognitionException(input);
                    ReportError(JqlParseErrorMessages.BadOperand(e.Token), e);
                }
                $clause = new WasClause($field.field.Name, $operator.operator, $operand.operand, $pred.predicate);
                supportsHistoryPredicate=true;
           }
           else
           {
		        if ($operand.operand == null)
                {
                    var e = new RecognitionException(input);
                    ReportError(JqlParseErrorMessages.BadOperand(e.Token), e);
                }
                $clause = new TerminalClause($field.field.Name, $operator.operator, $operand.operand);
                supportsHistoryPredicate=false;
                if ($pred.predicate != null)
                {
                    var errorMessage = JqlParseErrorMessages.UnsupportedPredicate($pred.predicate.DisplayString, $operator.operator.ToString());
                    JqlParseException exception = new JqlParseException(errorMessage);
                    ReportError(errorMessage, exception);
                }
           }
        }
	}
	;
	catch [RecognitionException e]
	{
	    // Because of the ANTLR lookahead, these ain't actually called ;-)
	    if (f == null)
	    {
	        ReportError(JqlParseErrorMessages.BadFieldName(e.Token), e);
	    }
	    else if (op == null)
	    {
	        ReportError(JqlParseErrorMessages.BadOperator(e.Token), e);
	    }
	    else if (opand == null)
	    {
	        ReportError(JqlParseErrorMessages.BadOperand(e.Token), e);
	    }
	    else
	    {
	        ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
	    }
	}


historyPredicate returns [IHistoryPredicate predicate]
	@init {
		var predicates = new List<IHistoryPredicate>();
	}
	: (p = terminalHistoryPredicate { predicates.Add($p.predicate);})+ {$predicate = predicates.Count == 1? predicates.ElementAt(0) : new AndHistoryPredicate(predicates); }
	;

/*
historyPredicate returns [HistoryPredicate predicate]
	@init {
		final List<HistoryClause> clauses = new List<HistoryClause>();
	}
	: orHistoryPredicate {$predicate = orHistoryPredicate.predicate;}
	;


orHistoryPredicate returns [HistoryPredicate predicate]
	@init {
		final List<HistoryPredicate> predicates = new List<HistoryPredicate>();
	}
	: p = andHistoryPredicate { predicates.Add($p.predicate);} ( OR p = andHistoryPredicate { predicates.Add($p.predicate);} )* 
	{
		$predicates.Count == 1 ? predicates.ElementAt(0) : new OrHistoryPredicate(predicates);
	}
	;
	
andHistoryPredicate returns [HistoryPredicate predicate]
	@init {
		final List<HistoryPredicate> predicates  = new List<HistoryPredicate>();
	}
	: p = notHistoryPredicate { predicates.Add($p.predicate);} (AND? p = notHistoryPredicate {predicates.Add($p.predicate);} )* 
	{
		$predicates.Count == 1 ? predicates.ElementAt(0) : new AndHistoryPredicate(predicates);
	}
	;
	
notHistoryPredicate returns [HistoryPredicate predicate]
	@init {
		final List<HistoryPredicate> predicates  = new List<HistoryPredicate>();
	}
	: (NOT|BANG) np = notHistoryPredicate { $predicate = new NotHistoryPredicate($np.predicate); }
	| terminalHistoryPredicate { $predicate = $terminalHistoryPredicate.predicate; }
	| subHistoryPredicate { $predicate = $subPredicate.predicate; }
	;
	catch [NoViableAltException e]
	{
	    // not sure what this case suggests at this stage
	    e.printStackTrace();
	}
	


subHistoryPredicate returns [HistoryPredicate predicate]
    : LPAREN orHistoryPredicate RPAREN { $predicate = $orHistoryPredicateClause.predicate; }
    ;
    catch [MismatchedTokenException e]
    {
        if (e.Expecting == RPAREN)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, ")"), e);
        }
        else@
        {
            throw e;
        }
    }
*/

terminalHistoryPredicate returns [IHistoryPredicate predicate]
	: historyPredicateOperator operand
	{
		$predicate = new TerminalHistoryPredicate($historyPredicateOperator.operator, $operand.operand);
	}
	;


historyPredicateOperator returns [Operator operator]
	: FROM { $operator = Operator.FROM; }
	| TO {$operator = Operator.TO; }
	| BY {$operator = Operator.BY; }
	| BEFORE {$operator = Operator.BEFORE; }
	| AFTER {$operator = Operator.AFTER; }
	| ON {$operator = Operator.ON; } 
	| DURING {$operator = Operator.DURING;};
	
	

/*
 * Parse the current operator.
 */
operator returns [Operator operator]
	: EQUALS { $operator = Operator.EQUALS; }
	| NOT_EQUALS { $operator = Operator.NOT_EQUALS; }
	| LIKE { $operator = Operator.LIKE; }
	| NOT_LIKE { $operator = Operator.NOT_LIKE; }	
	| LT { $operator = Operator.LESS_THAN; }
	| GT { $operator = Operator.GREATER_THAN; }
	| LTEQ { $operator = Operator.LESS_THAN_EQUALS; }
	| GTEQ { $operator = Operator.GREATER_THAN_EQUALS; }
	| IN { $operator = Operator.IN; }
	| IS NOT { $operator = Operator.IS_NOT; }
	| IS { $operator = Operator.IS; }
	| NOT IN { $operator = Operator.NOT_IN; }
	| WAS { $operator = Operator.WAS; }
	| WAS NOT { $operator = Operator.WAS_NOT; }
    | WAS IN { $operator = Operator.WAS_IN; }
    | WAS NOT IN { $operator = Operator.WAS_NOT_IN; }
    | CHANGED { $operator = Operator.CHANGED; }
    ;
	catch [MismatchedTokenException e]
	{
	    //This will only get thrown when we read in "IS" or "NOT" not followed by its correct string.
        if (e.Expecting == NOT)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "NOT"), e);
        }
        else if (e.Expecting == IN)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "IN"), e);
        }
        else
        {
            // We will do this just in case.
            ReportError(JqlParseErrorMessages.BadOperator(e.Token), e);
        }
	}
	catch [RecognitionException e]
	{
        IToken currentToken = input.LT(1);
        if (currentToken.Type == IS)
        {
            // This happens when we get an IS is not followed by a NOT.
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "NOT"), e);
        }
        else if (currentToken.Type == NOT)
        {
            // This happens when we get a NOT is not followed by a IN.
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "IN"), e);
        }
        else if (currentToken.Type == WAS)
        {
            ReportError(JqlParseErrorMessages.BadOperand(e.Token),e);
        }
        else
        {
            ReportError(JqlParseErrorMessages.BadOperator(e.Token), e);
        }
    }

/*
 * Parse the JQL field name of a JQL clause.
 */
field returns [FieldReference field]
    @init {
        var names = new List<string>();
        var arrays = new List<string>();
        var propertyRefs = new List<string>();
    }
    @after {
        $field = new FieldReference(names, arrays, propertyRefs);
    }
	:
    num = numberString { names.Add($num.stringValue); }
    |
    (
        (
            str = string { names.Add(CheckFieldName($str.start)); }
            | cf = customField { names.Add($cf.field); }
        )
        (
	        (
	            LBRACKET
	            (
	                reff = argument {arrays.Add($reff.arg);}
	            )
	            RBRACKET
	        )
	        (
	            reff = propertyArgument {propertyRefs.Add($reff.arg);}
	        )*
        )*
	)
	;
	catch [MismatchedTokenException e]
	{
	    switch (e.Expecting)
	    {
	        case LBRACKET:
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "["), e);
                break;
            case RBRACKET:
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "]"), e);
                break;
	    }
	}
	catch [EarlyExitException e]
	{
        ReportError(JqlParseErrorMessages.BadPropertyArgument(e.Token), e);
	}
	catch [RecognitionException e]
	{
	    if (e.Token.Type == LBRACKET)
        {
            // We probably have some sort of custom field id that does not start with cf. Lets tell the user all about it.
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "cf"), e);
        }
        else
        {
            ReportError(JqlParseErrorMessages.BadFieldName(e.Token), e);
        }
	}

customField returns [string field]
    : CUSTOMFIELD LBRACKET posnum = POSNUMBER RBRACKET { $field = JqlCustomFieldId.ToString(ParseLong($posnum)); }
    ;
    catch [MismatchedTokenException e]
    {
        switch(e.Expecting)
        {
            case CUSTOMFIELD:
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "cf"), e);
                break;
            case LBRACKET:
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "["), e);
                break;
            case POSNUMBER:
                ReportError(JqlParseErrorMessages.BadCustomFieldId(e.Token), e);
                break;
            case RBRACKET:
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "]"), e);
                break;
            default:
                throw e;
        }
    }

/*
 * Checks if a field name is actually valid.
 */
fieldCheck returns [FieldReference field]
	: f = field { $field = $f.field; } EOF
	;

/*
 * Parse the operand (RHS) of a JQL clause.
 */
operand	returns [IOperand operand]
	: EMPTY { $operand = new EmptyOperand(); }
	| str = string { $operand = new SingleValueOperand($str.stringValue); }
	| number = numberString { $operand = new SingleValueOperand(ParseLong($numberString.start)); }
	| fn = func {$operand = $fn.func;}
	| l = list {$operand = $l.list;}
	;
	catch [NoViableAltException e]
	{
        IToken currentToken = input.LT(1);
        IToken errorToken = e.Token;

        //HACKETY, HACKETY HACK. ANTLR uses a DFA to decide which type of operand we are going to handle. This DFA
        //will only find "string" if it is followed by one of {EOF, OR, AND, RPAREN, COMMA, ORDER}. This means that
        //a query like "a=b c=d" will actually fail here and appear to be an illegal operand. So we use a simple
        //heuristic here. If the currentToken != errorToken then it means we have a valid value its just that we
        //have not seen one of the expected trailing tokens.

        if (currentToken.TokenIndex < errorToken.TokenIndex)
        {
            if (operandLevel <= 0)
            {
                // If not in a MutliValueOperand, we probably mean "AND" and "OR"
                ReportError(JqlParseErrorMessages.NeedLogicalOperator(errorToken), e);
            }
            else
            {
                // If in a MutliValueOperand, we probably mean "," or ")".
                ReportError(JqlParseErrorMessages.ExpectedText(errorToken, ",", ")"), e);
            }
        }
        else
        {
            ReportError(JqlParseErrorMessages.BadOperand(errorToken), e);
        }
	}
	catch [RecognitionException e]
	{
	    ReportError(JqlParseErrorMessages.BadOperand(e.Token), e);
	}
 
/*
 * Parse a String in JQL.
 */
string returns [String stringValue]
	: str = STRING { $stringValue = $str.text; }
	| str = QUOTE_STRING { $stringValue = $str.text; }
	| str = SQUOTE_STRING { $stringValue = $str.text; }
	;

numberString returns [String stringValue]
	: num = (POSNUMBER | NEGNUMBER) { $stringValue = $num.text; }
	;

/*
 * 
 */
stringValueCheck returns [String stringValue]
	: str = string { $stringValue = $str.stringValue; } EOF
	;
	
/*
 * Parse the JQL list structure used for the 'IN' operator.
 */
list returns [IOperand list]
	@init {
		List<IOperand> args = new List<IOperand>();
		operandLevel++;
	}
	@after {
	    operandLevel--;
	}
	: LPAREN opnd = operand {args.Add($opnd.operand);} 
		(COMMA opnd = operand {args.Add($opnd.operand);})* RPAREN {$list = new MultiValueOperand(args);}
	;
	catch [MismatchedTokenException e]
    {
        if (e.Expecting == RPAREN)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, ")"), e);
        }
        else
        {
            throw e;
        }
    }
    
/*
 * Parse out the JQL function.
 */
func returns [FunctionOperand func]
	: fname = funcName LPAREN arglist? RPAREN
	{
	    List<string> args = $arglist.args == null ? new List<string>() : $arglist.args;
        $func = new FunctionOperand($fname.name, args);
    }
	;
	catch [MismatchedTokenException e]
	{
	    // We should only get here when the query is trying to match ')'.
        if (e.Expecting == RPAREN)
        {
            if (e.Token.Type == EOF || e.Token.Type == COMMA)
            {
                ReportError(JqlParseErrorMessages.ExpectedText(e.Token, ")"), e);
            }
            else
            {
                // There is some argument that is not a string or number.
                ReportError(JqlParseErrorMessages.BadFunctionArgument(e.Token), e);
            }
        }
        else
        {
            ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
        }
    }
	catch [RecognitionException e]
	{
        ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
	}

/*
 * Rule to match function names.
 */ 
funcName returns [string name]
	: string { $name = CheckFunctionName($string.start); }
	| num = numberString { $name = $num.stringValue; }
	;

/*
 * Rule used to check the validity of a function name.
 */	
funcNameCheck returns [string name]
	: fname = funcName { $name = $fname.name; } EOF
	;

/*
 * Parse out a JQL function argument list.
 */
arglist	returns [List<string> args]
	@init {
		args = new List<string>();
	}
	@after {
	    // ANTLR will exit the arg list after the first token that is NOT a comma. If we exit and we are not
	    // at the end of the argument list, then we have an error.

	    IToken currentToken = input.LT(1);
	    if (currentToken.Type != EOF && currentToken.Type != RPAREN)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(currentToken, ")", ","), null);
        }
	}
	: str = argument { args.Add($str.arg); }
	(COMMA str = argument { args.Add($str.arg); } )*
	;

propertyArgument returns [string arg]
	: a = argument
	{
	    // property argument should at least contain one dot and property reference
	        if (a.Length<2 || a[0] != '.' || a[1] == '.')
            {

                IToken currentToken = input.LT(-1);
                if(input.Get(input.LT(-1).TokenIndex-1).Type == MATCHWS )
                {
                    ReportError(JqlParseErrorMessages.BadOperator(currentToken), null);
                }
                else
                {
                    ReportError(JqlParseErrorMessages.BadPropertyArgument(currentToken), null);
                }
            }
            // remove leading dot as this is unnecessary
            $arg = $a.arg.Substring(1);

    };

/*
 * Parse out a JQL function argument. Must be strings for the time being.
 */
argument returns [string arg]
	: str = string { $arg = $str.stringValue; } 
	| number = numberString { $arg = $number.stringValue; }
	;
	catch [RecognitionException e]
	{
	    switch (e.Token.Type)
	    {
	        case COMMA:
	        case RPAREN:
	            ReportError(JqlParseErrorMessages.EmptyFunctionArgument(e.Token), e);
                break;
            case RBRACKET:
                ReportError(JqlParseErrorMessages.BadPropertyArgument(e.Token), e);
                break;
            default:
                ReportError(JqlParseErrorMessages.BadFunctionArgument(e.Token), e);
                break;
	    }
	}

/*
 * Used to check that an argument value is actually valid.
 */
argumentCheck returns [string arg]
	: a = argument { $arg = $a.arg; } EOF
	;

/*
 * Represents the ORDER BY clause in JQL.
 */
orderBy returns [IOrderBy order]
	 @init {
		List<SearchSort> args = new List<SearchSort>();
	}
	: ORDER BY f = searchSort { args.Add(f); }
		(COMMA f = searchSort { args.Add(f); })* { $order = new OrderBy(args); }
	;
	catch [MismatchedTokenException e]
	{
        if (e.Expecting == BY)
        {
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "by"), e);
        }
        else if (e.Expecting == ORDER)
        {
            // This should not happen since the lookahead ensures that ORDER has been matched.
            ReportError(JqlParseErrorMessages.ExpectedText(e.Token, "order"), e);
        }
        else
        {
            ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
        }
	}
	catch [RecognitionException e]
	{
	    ReportError(JqlParseErrorMessages.GenericParseError(e.Token), e);
	}

/*
 * Represents a JQL field followed by a sort order.
 */
searchSort returns [SearchSort sort]
	: f = field (o = DESC | o = ASC)?
	{
	    if (o == null)
	    {
	        IToken token = input.LT(1);
	        // ANTLR is not very strict here. If ANTLR sees an illegal sort order, then it will simply leave
	        // the order by clause. We want to be a little stricter and find illegal SORT ORDERS.
	        if (token.Type != EOF && token.Type != COMMA)
	        {
	            ReportError(JqlParseErrorMessages.BadSortOrder(token), null);
	        }
	    }

	    SortOrder order = (o == null) ? SortOrder.ASC : SortOrderHelpers.ParseString($o.text);
	    if (f != null && $f.field.IsEntityProperty())
	    {
	        $sort = new SearchSort($f.field.Name, new List<Property>{$f.field.Property}, order);
	    }
	    else
	    {
		    $sort = new SearchSort($f.text, order);
		}
	}
	;
	catch [RecognitionException e]
	{
	    if (f == null)
	    {
            // We could not find a field.
            ReportError(JqlParseErrorMessages.BadFieldName(e.Token), e);
	    }
	    else
	    {
	        // We could not find a correct order.
	        ReportError(JqlParseErrorMessages.BadSortOrder(e.Token), e);
	    }
	}

/**
 * Some significant characters that need to be matched.
 */
LPAREN      : 	'(';
RPAREN		:	')';
COMMA		: 	',';
LBRACKET	:	'[';
RBRACKET 	: 	']';

fragment MINUS:  '-';

/**
 * JQL Operators
 */

BANG		:	'!';
LT		:	'<';
GT		:	'>';
GTEQ		:	'>=';
LTEQ 		:	'<=';
EQUALS		:	'=' ;
NOT_EQUALS	:	'!=';
LIKE		:	'~';
NOT_LIKE	:	'!~';		
IN		:	('I'|'i')('N'|'n');
IS		:	('I'|'i')('S'|'s');
AND 		:	('A'|'a')('N'|'n')('D'|'d') | AMPER | AMPER_AMPER;
OR		:	('O'|'o')('R'|'r') | PIPE | PIPE_PIPE;	
NOT		:	('N'|'n')('O'|'o')('T'|'t');
EMPTY		:	('E'|'e')('M'|'m')('P'|'p')('T'|'t')('Y'|'y') | ('N'|'n')('U'|'u')('L'|'l')('L'|'l');

WAS		:	('W'|'w')('A'|'a')('S'|'s');
CHANGED		:	('C'|'c')('H'|'h')('A'|'a')('N'|'n')('G'|'g')('E'|'e')('D'|'d');

BEFORE		:	('B'|'b')('E'|'e')('F'|'f')('O'|'o')('R'|'r')('E'|'e');
AFTER		:	('A'|'a')('F'|'f')('T'|'t')('E'|'e')('R'|'r');
FROM		:	('F'|'f')('R'|'r')('O'|'o')('M'|'m');
TO		:	('T'|'t')('O'|'o');


ON		:	('O'|'o')('N'|'n');
DURING		:	('D'|'d')('U'|'u')('R'|'r')('I'|'i')('N'|'n')('G'|'g');


/**
 * Order by
 */
ORDER	: ('o'|'O')('r'|'R')('d'|'D')('e'|'E')('r'|'R');
BY	: ('b'|'B')('y'|'Y');	
ASC	:	('a'|'A')('s'|'S')('c'|'C');
DESC:	('d'|'D')('e'|'E')('s'|'S')('c'|'C');

/*
 * Numbers
 */
POSNUMBER
	: DIGIT+;

NEGNUMBER
	: MINUS DIGIT+;

/**
 * The custom field prefix.
 */
CUSTOMFIELD
	: ('c'|'C')('f'|'F');

/**
 * String handling in JQL.
 */
 
STRING
    @init { pushPosition(STRING); }
    @after { popPosition(); }
	: (ESCAPE | ~(BSLASH | WS | STRINGSTOP))+
	{
		// Once this method is called, the text of the current token is fixed. This means that this Lexical rule
		// should not be called from other lexical rules.
		checkAndSet();
	}
	;

QUOTE_STRING
    @init { pushPosition(QUOTE_STRING); }
    @after { popPosition(); }
	: (QUOTE (ESCAPE | ~(BSLASH | QUOTE | CONTROLCHARS))* QUOTE)
	{
		//Once this method is called, the text of the current token is fixed. This means that this Lexical rule
		//should not be called from other lexical rules.
		stripAndSet();
	};
		
SQUOTE_STRING
    @init { pushPosition(SQUOTE_STRING); }
    @after { popPosition(); }
	: (SQUOTE (ESCAPE | ~(BSLASH | SQUOTE | CONTROLCHARS))* SQUOTE)
	{
		//Once this method is called, the text of the current token is fixed. This means that this Lexical rule
		//should not be called from other lexical rules.
		stripAndSet();
	};

/**
 * Match any whitespace and then ignore it.
 */	
MATCHWS  		:  	WS+ { $channel = HIDDEN; };

/**
 * These are some characters that we do not use now but we want to reserve. We have not reserved MINUS because we
 * really really really don't want to force people into quoting issues keys and dates.
 */
fragment RESERVED_CHARS
	: '{' | '}'
	| '*' | '/' | '%' | '+' | '^'
	| '$' | '#' | '@'
	| '?' | ';'
	;

/**
 * This is yet another large hack. We want this to be a different error message for reserved characters so we just match
 * them and try to recover.
 */
ERROR_RESERVED
    @init
    {
        pushPosition(ERROR_RESERVED);
        recover();
    }
    @after { popPosition(); }
    : RESERVED_CHARS
    ;


/**
 * This is a large HACK. Match any character that we did not match using one of the previous rules. It must be an error
 * so lets try and do something.
 */
ERRORCHAR
    @init
    {
        pushPosition(ERRORCHAR);
        recover();
    }
    @after { popPosition(); }
    : .
    ;

fragment QUOTE		:	'"' ;
fragment SQUOTE 	:	'\'';
fragment BSLASH		:	'\\';
fragment NL		:	'\r';
fragment CR		:	'\n';
fragment SPACE		:	' ';	
fragment AMPER	:	'&';
fragment AMPER_AMPER:	 '&&';
fragment PIPE	:	'|';	
fragment PIPE_PIPE	:	'||';	


/*
 * I would like to use the @afer rule but it does not appear to work for fragment rulez.
 */
fragment ESCAPE
    @init { pushPosition(ESCAPE); }
	:   BSLASH
	(
             	't'
             |  'n'
             |  'r' 
             |  QUOTE 
             |  SQUOTE
             |  BSLASH 
             |  SPACE
             |	'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT
	) { popPosition(); }
	;

/**
 * These are the Tokens that should not be included as a part of an unquoted string.
 */
fragment STRINGSTOP
	: CONTROLCHARS  
	| QUOTE | SQUOTE
	| EQUALS 
	| BANG 
	| LT | GT
	| LPAREN | RPAREN 
	| LIKE 
	| COMMA 
	| LBRACKET | RBRACKET
	| PIPE
	| AMPER
	| RESERVED_CHARS
	| NEWLINE;

/*
 * These are control characters minus whitespace. We use the negation of this set as the set of 
 * characters that we allow in a string.
 *
 * NOTE: This list needs to be synchronised with JqlStringSupport.IsJqlControlCharacter.
 */
fragment CONTROLCHARS
	:	'\u0000'..'\u0009'  //Exclude '\n' (\u000a)
	|   '\u000b'..'\u000c'  //Exclude '\r' (\u000d)
	|   '\u000e'..'\u001f'
	|	'\u007f'..'\u009f'
	//The following are Unicode non-characters. We don't want to parse them. Importantly, we wish 
	//to ignore U+FFFF since ANTLR evilly uses this internally to represent EOF which can cause very
	//strange behaviour. For example, the Lexer will incorrectly tokenise the POSNUMBER 1234 as a STRING
	//when U+FFFF is not excluded from STRING.
	//
	//http://en.wikipedia.org/wiki/Unicode
	| 	'\ufdd0'..'\ufdef'
	|	'\ufffe'..'\uffff' 
	;

fragment NEWLINE
    :   NL | CR;

fragment HEXDIGIT
	:	DIGIT | ('A'|'a') | ('B'|'b') | ('C'|'c') | ('D'|'d') | ('E'|'e') | ('F'|'f')
	;
	
fragment DIGIT
	:	'0'..'9'
	;

fragment WS 
	: 	(SPACE|'\t'|NEWLINE)
	;
