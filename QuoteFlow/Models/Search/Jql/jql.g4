grammar Jql;

options { language=CSharp2; }

@parser::header {
	
}

@parser::namespace { Demo.Antlr }


@lexer::namespace { Demo.Antlr }

parse
  :  exp EOF -> ^(ROOT exp)
  ;

exp
  :  addExp
  ;

addExp
  :  mulExp (('+' | '-')^ mulExp)*
  ;

mulExp
  :  unaryExp (('*' | '/')^ unaryExp)*
  ;

unaryExp
  :  '-' atom -> ^(UNARY_MIN atom)
  |  atom
  ;

atom
  :  Number
  |  '(' exp ')' -> exp
  ;

Number
  :  ('0'..'9')+ ('.' ('0'..'9')+)?
  ;

Space 
  :  (' ' | '\t' | '\r' | '\n'){Skip();}
  ;