using System;
using System.Collections.Generic;

namespace Iglu
{
	abstract class Stmt
	{
		public interface IVisitor<R>
		{
			R visitBlockStmt(Block stmt);
			R visitExpressionStmt(Expression stmt);
			R visitPrintStmt(Print stmt);
			R visitLetStmt(Let stmt);
		}
		public class Block : Stmt
		{
			public Block(List<Stmt> statements)
			{
				this.statements = statements;
			}

			public readonly List<Stmt> statements;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitBlockStmt(this);
			}
		}
		public class Expression : Stmt
		{
			public Expression(Expr expression)
			{
				this.expression = expression;
			}

			public readonly Expr expression;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitExpressionStmt(this);
			}
		}
		public class Print : Stmt
		{
			public Print(Expr expression)
			{
				this.expression = expression;
			}

			public readonly Expr expression;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitPrintStmt(this);
			}
		}
		public class Let : Stmt
		{
			public Let(Token name, Expr initializer)
			{
				this.name = name;
				this.initializer = initializer;
			}

			public readonly Token name;
			public readonly Expr initializer;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitLetStmt(this);
			}
		}

		public abstract R Accept<R>(IVisitor<R> visitor);
	}
}
