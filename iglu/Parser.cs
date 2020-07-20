using Iglu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iglu
{
	class Parser
	{
		private class ParseError : Exception { }

		private readonly List<Token> tokens;
		private int current = 0;

		public Parser(List<Token> tokens)
		{
			this.tokens = tokens;
		}

		public List<Stmt> Parse()
		{
			List<Stmt> statements = new List<Stmt>();
			while(!IsAtEnd())
			{
				statements.Add(Declaration());
			}

			return statements;
		}

		private bool Match(params TokenType[] types)
		{
			foreach(TokenType type in types)
			{
				if(Check(type))
				{
					Advance();
					return true;
				}
			}

			return false;
		}

		private bool Check(TokenType type)
		{
			if(IsAtEnd())
			{
				return false;
			}
			else
			{
				return Peek().type == type;
			}
		}

		private Token Advance()
		{
			if(!IsAtEnd())
			{
				current++;
			}
			return Previous();
		}

		private bool IsAtEnd()
		{
			return Peek().type == TokenType.EOF;
		}

		private Token Peek()
		{
			return tokens[current];
		}

		private Token Previous()
		{
			return tokens[current - 1];
		}

		private Token Consume(TokenType type, string message)
		{
			if(Check(type))
			{
				return Advance();
			}
			else
			{
				throw Error(Peek(), message);
			}
		}

		private ParseError Error(Token token, String message)
		{
			Program.Error(token, message);
			return new ParseError();
		}

		private void Synchronize()
		{
			Advance();

			while(!IsAtEnd())
			{
				if (Previous().type == TokenType.SEMICOLON)
				{
					return;
				}
				else
				{
					switch (Peek().type)
					{
						case TokenType.CLASS:
						case TokenType.COMMA:
						case TokenType.FUN:
						case TokenType.FOR:
						case TokenType.IF:
						case TokenType.LET:
						case TokenType.PRINT:
						case TokenType.RETURN:
						case TokenType.WHILE:
							return;
					}

					Advance();
				}
			}
		}

		private Expr Expression()
		{
			return Assignment();
		}

		private Stmt Declaration()
		{
			try
			{
				if (Match(TokenType.FUN)) return FunDeclaration("function");
				if (Match(TokenType.LET)) return LetDeclaration();

				return Statement();
			}
			catch(ParseError)
			{
				Synchronize();
				return null;
			}
		}

		private Stmt LetDeclaration()
		{
			Token name = Consume(TokenType.IDENTIFIER, "Expected a variable name.");

			Expr initializer = null;
			if(Match(TokenType.EQUAL))
			{
				initializer = Expression();
			}

			Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
			return new Stmt.Let(name, initializer);
		}

		private Stmt FunDeclaration(string kind)
		{
			Token name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");

			Consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");

			List<Token> parameters = new List<Token>();
			if(!Check(TokenType.RIGHT_PAREN))
			{
				do
				{
					if (parameters.Count >= 255)
					{
						Error(Peek(), "Cannot have more than 255 parameters");
					}
					parameters.Add(Consume(TokenType.IDENTIFIER, "Expected parameter name."));
				}
				while (Match(TokenType.COMMA));
			}
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

			Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
			List<Stmt> body = Block();

			return new Stmt.Function(name, parameters, body);
		}

		private Stmt Statement()
		{
			if (Match(TokenType.FOR)) return ForStatement();
			if (Match(TokenType.IF)) return IfStatement();
			if (Match(TokenType.PRINT)) return PrintStatement();
			if (Match(TokenType.WHILE)) return WhileStatement();
			if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

			return ExpressionStatement();
		}

		private Stmt ForStatement()
		{
			Consume(TokenType.LEFT_PAREN, "Expect '(' after for statement.");
			
			Stmt initializer;
			if(Match(TokenType.SEMICOLON))
			{
				initializer = null;
			}
			else if(Match(TokenType.LET))
			{
				initializer = LetDeclaration();
			}else
			{
				initializer = ExpressionStatement();
			}
			
			Expr cond = null;
			if(!Check(TokenType.SEMICOLON))
			{
				cond = Expression();
			}
			Consume(TokenType.SEMICOLON, "Expect ';' after condition in for statement");

			Expr increment = null;
			if(!Check(TokenType.RIGHT_PAREN))
			{
				increment = Expression();
			}

			Consume(TokenType.RIGHT_PAREN, "Expect ')' after for expressions.");

			Stmt body = Statement();

			if(increment != null)
			{
				body = new Stmt.Block(new List<Stmt>()
				{
					body,
					new Stmt.Expression(increment)
				});
			}

			if (cond == null) cond = new Expr.Literal(true);
			body = new Stmt.While(cond, body);

			if (initializer != null)
			{
				body = new Stmt.Block(new List<Stmt>()
				{
					initializer,
					body
				});
			}

			return body;
		}

		private Stmt IfStatement()
		{
			Consume(TokenType.LEFT_PAREN, "Expect '(' after if statement.");
			Expr condition = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect '(' after if condition");
			Stmt then = Statement();
			Stmt el = Match(TokenType.ELSE) ? Statement() : null;

			return new Stmt.If(condition, then, el);
		}

		private Stmt PrintStatement()
		{
			Expr value = Expression();
			Consume(TokenType.SEMICOLON, "Expect ';' after value.");
			return new Stmt.Print(value);
		}

		private List<Stmt> Block()
		{
			List<Stmt> statements = new List<Stmt>();

			while(!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
			{
				statements.Add(Declaration());
			}

			Consume(TokenType.RIGHT_BRACE, "Expect '}', after block.");

			return statements;
		}

		private Stmt WhileStatement()
		{
			Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
			Expr condition = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after while condition.");
			Stmt body = Statement();

			return new Stmt.While(condition, body);
		}

		private Stmt ExpressionStatement()
		{
			Expr value = Expression();
			Consume(TokenType.SEMICOLON, "Expect ';' after value.");
			return new Stmt.Expression(value);
		}

		private Expr Assignment()
		{
			Expr expr = Ternary();

			if(Match(TokenType.EQUAL))
			{
				Token equals = Previous();
				Expr value = Assignment();

				if(expr is Expr.Variable)
				{
					Token name = ((Expr.Variable)expr).name;
					return new Expr.Assign(name, value);
				}
				Error(equals, "Invalid assignment target.");
			}

			return expr;
		}

		//private Expr Commation() // terrible name I know
		//{
		//	Expr expr = Ternary();

		//	while(Match(TokenType.COMMA))
		//	{
		//		Token oper = Previous();
		//		Expr right = Ternary();
		//		expr = new Expr.Binary(expr, oper, right);
		//	}

		//	return expr;
		//}

		private Expr Ternary()
		{
			Expr expr = LogicOr();

			while (Match(TokenType.QUESTION))
			{
				Token oper1 = Previous();
				Expr ifTrue = Expression();
				Consume(TokenType.COLON, "Expecting ':' as part of ternary operator.");
				Token oper2 = Previous();
				Expr ifFalse = Equality();

				Expr ifs = new Expr.Binary(ifTrue, oper2, ifFalse);
				expr = new Expr.Binary(expr, oper1, ifs);
			}

			return expr;
		}

		private Expr LogicOr()
		{
			Expr expr = LogicAnd();

			while (Match(TokenType.OR))
			{
				Token oper = Previous();
				Expr right = LogicOr();
				expr = new Expr.Binary(expr, oper, right);
			}

			return expr;
		}

		private Expr LogicAnd()
		{
			Expr expr = Equality();

			while (Match(TokenType.AND))
			{
				Token oper = Previous();
				Expr right = LogicAnd();
				expr = new Expr.Binary(expr, oper, right);
			}

			return expr;
		}

		private Expr Equality()
		{
			Expr expr = Comparison();

			while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
			{
				Token oper = Previous();
				Expr right = Comparison();
				expr = new Expr.Binary(expr, oper, right);
			}

			return expr;
		}

		private Expr Comparison()
		{
			Expr expr = Addition();

			while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
			{
				Token oper = Previous();
				Expr right = Addition();
				expr = new Expr.Binary(expr, oper, right);
			}

			return expr;
		}

		private Expr Addition()
		{
			Expr expr = Multiplication();

			while(Match(TokenType.MINUS, TokenType.PLUS))
			{
				Token oper = Previous();
				Expr right = Addition();
				expr = new Expr.Binary(expr, oper, right);
			}

			return expr;
		}

		private Expr Multiplication()
		{
			Expr expr = Unary();

			while(Match(TokenType.SLASH, TokenType.STAR))
			{
				Token oper = Previous();
				Expr right = Unary();
				expr = new Expr.Binary(expr, oper, right);
			}

			return expr;
		}

		private Expr Unary()
		{
			if(Match(TokenType.BANG, TokenType.MINUS))
			{
				Token oper = Previous();
				Expr right = Unary();
				return new Expr.Unary(oper, right);
			}
			else
			{
				return Call();
			}
		}

		private Expr FinishCall(Expr expr)
		{
			List<Expr> args = new List<Expr>();

			if(!Check(TokenType.RIGHT_PAREN))
			{
				do
				{
					if(args.Count >= 255)
					{
						Error(Peek(), "Cannot have more than 255 arguments.");
					}
					args.Add(Expression());
				}
				while (Match(TokenType.COMMA));
			}

			Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments");

			return new Expr.Call(expr, paren, args);
		}

		private Expr Call()
		{
			Expr expr = Primary();

			while (true)
			{
				if (Match(TokenType.LEFT_PAREN))
				{
					expr = FinishCall(expr);
				}else
				{
					break;
				}
			}

			return expr;
		}

		private Expr Primary()
		{
			if (Match(TokenType.FALSE))
			{
				return new Expr.Literal(false);
			}
			else if (Match(TokenType.TRUE))
			{
				return new Expr.Literal(true);
			}
			else if (Match(TokenType.NULL))
			{
				return new Expr.Literal(null);
			}
			else if (Match(TokenType.NUMBER, TokenType.STRING))
			{
				return new Expr.Literal(Previous().literal);
			}
			else if (Match(TokenType.IDENTIFIER))
			{
				return new Expr.Variable(Previous());
			}
			else if (Match(TokenType.LEFT_PAREN))
			{
				Expr expr = Expression();
				Consume(TokenType.RIGHT_PAREN, "Expecting ')' after expression.");
				return new Expr.Grouping(expr);
			}
			else
			{
				throw Error(Peek(), "Expected an expression.");
			}
		}
	}
}
