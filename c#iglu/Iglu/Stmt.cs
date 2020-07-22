using System;
using System.Collections.Generic;

namespace Iglu
{
	abstract class Stmt
	{
		public interface IVisitor<R>
		{
			R visitBlockStmt(Block stmt);
			R visitIfStmt(If stmt);
			R visitClassStmt(Class stmt);
			R visitExpressionStmt(Expression stmt);
			R visitFunctionStmt(Function stmt);
			R visitPrintStmt(Print stmt);
			R visitReturnStmt(Return stmt);
			R visitLetStmt(Let stmt);
			R visitWhileStmt(While stmt);
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
		public class If : Stmt
		{
			public If(Expr condition, Stmt then, Stmt el)
			{
				this.condition = condition;
				this.then = then;
				this.el = el;
			}

			public readonly Expr condition;
			public readonly Stmt then;
			public readonly Stmt el;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitIfStmt(this);
			}
		}
		public class Class : Stmt
		{
			public Class(Token name, List<Stmt.Function> methods)
			{
				this.name = name;
				this.methods = methods;
			}

			public readonly Token name;
			public readonly List<Stmt.Function> methods;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitClassStmt(this);
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
		public class Function : Stmt
		{
			public Function(Token name, List<Token> parameters, List<Stmt> body)
			{
				this.name = name;
				this.parameters = parameters;
				this.body = body;
			}

			public readonly Token name;
			public readonly List<Token> parameters;
			public readonly List<Stmt> body;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitFunctionStmt(this);
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
		public class Return : Stmt
		{
			public Return(Token keyword, Expr value)
			{
				this.keyword = keyword;
				this.value = value;
			}

			public readonly Token keyword;
			public readonly Expr value;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitReturnStmt(this);
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
		public class While : Stmt
		{
			public While(Expr condition, Stmt body)
			{
				this.condition = condition;
				this.body = body;
			}

			public readonly Expr condition;
			public readonly Stmt body;

			public override R Accept<R>(IVisitor<R> visitor)
			{
				return visitor.visitWhileStmt(this);
			}
		}

		public abstract R Accept<R>(IVisitor<R> visitor);
	}
}
