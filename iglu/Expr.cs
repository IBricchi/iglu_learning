using System;
using System.Collections.Generic;

namespace Iglu
{
	abstract class Expr
	{
		public interface IVisitor<R>
		{
			R visitAssignExpr(Assign expr);
			R visitBinaryExpr(Binary expr);
			R visitCallExpr(Call expr);
			R visitGetExpr(Get expr);
			R visitGroupingExpr(Grouping expr);
			R visitLiteralExpr(Literal expr);
			R visitUnaryExpr(Unary expr);
			R visitSetExpr(Set expr);
			R visitVariableExpr(Variable expr);
		}
		public class Assign : Expr
		{
			public Assign(Token name, Expr value)
			{
				this.name = name;
				this.value = value;
			}

			public readonly Token name;
			public readonly Expr value;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitAssignExpr(this);
			}
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
		public class Call : Expr
		{
			public Call(Expr callee, Token paren, List<Expr> args)
			{
				this.callee = callee;
				this.paren = paren;
				this.args = args;
			}

			public readonly Expr callee;
			public readonly Token paren;
			public readonly List<Expr> args;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitCallExpr(this);
			}
		}
		public class Get : Expr
		{
			public Get(Expr obj, Token name)
			{
				this.obj = obj;
				this.name = name;
			}

			public readonly Expr obj;
			public readonly Token name;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitGetExpr(this);
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
		public class Set : Expr
		{
			public Set(Expr obj, Token name, Expr value)
			{
				this.obj = obj;
				this.name = name;
				this.value = value;
			}

			public readonly Expr obj;
			public readonly Token name;
			public readonly Expr value;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitSetExpr(this);
			}
		}
		public class Variable : Expr
		{
			public Variable(Token name)
			{
				this.name = name;
			}

			public readonly Token name;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitVariableExpr(this);
			}
		}

		public abstract R Accept<R>(IVisitor<R> visitor);
	}
}
