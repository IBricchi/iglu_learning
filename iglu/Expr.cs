using System;
using System.Collections.Generic;

namespace Iglu
{
	abstract class Expr
	{
		class Binary : Expr
		{
			public Binary(Expr left, Token oper, Expr right)
			{
				this.left = left;
				this.oper = oper;
				this.right = right;
			}

			public readonly Expr left;
			public readonly Token oper;
			public readonly Expr right;
		}
		class Grouping : Expr
		{
			public Grouping(Expr expression)
			{
				this.expression = expression;
			}

			public readonly Expr expression;
		}
		class Literal : Expr
		{
			public Literal(Object value)
			{
				this.value = value;
			}

			public readonly Object value;
		}
		class Unary : Expr
		{
			public Unary(Token oper, Expr right)
			{
				this.oper = oper;
				this.right = right;
			}

			public readonly Token oper;
			public readonly Expr right;
		}
	}
}
