using System;
using System.Collections.Generic;

namespace Iglu
{
	abstract class Expr
	{
		public interface IVisitor<R>
		{
			R visitBinaryExpr(Binary expr);
			R visitGroupingExpr(Grouping expr);
			R visitLiteralExpr(Literal expr);
			R visitUnaryExpr(Unary expr);
		}
		public class Binary : Expr
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

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitBinaryExpr(this);
			}
		}
		public class Grouping : Expr
		{
			public Grouping(Expr expression)
			{
				this.expression = expression;
			}

			public readonly Expr expression;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitGroupingExpr(this);
			}
		}
		public class Literal : Expr
		{
			public Literal(object value)
			{
				this.value = value;
			}

			public readonly object value;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitLiteralExpr(this);
			}
		}
		public class Unary : Expr
		{
			public Unary(Token oper, Expr right)
			{
				this.oper = oper;
				this.right = right;
			}

			public readonly Token oper;
			public readonly Expr right;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitUnaryExpr(this);
			}
		}

		public abstract R Accept<R>(IVisitor<R> visitor);
	}
}
