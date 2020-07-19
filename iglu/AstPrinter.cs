using Iglu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iglu
{
	class AstPrinter : Expr.IVisitor<String>
	{
		public string Print(Expr expr)
		{
			return expr.Accept(this);
		}

		public string visitAssignExpr(Expr.Assign expr)
		{
			throw new NotImplementedException();
		}

		public string visitBinaryExpr(Expr.Binary expr)
		{
			return Parenthesize(expr.oper.lexeme, expr.left, expr.right);
		}

		public string visitCallExpr(Expr.Call expr)
		{
			throw new NotImplementedException();
		}

		public string visitGroupingExpr(Expr.Grouping expr)
		{
			return Parenthesize("group", expr.expression);
		}

		public string visitLiteralExpr(Expr.Literal expr)
		{
			if (expr.value == null) return "null";
			return expr.value.ToString();
		}

		public string visitUnaryExpr(Expr.Unary expr)
		{
			return Parenthesize(expr.oper.lexeme, expr.right);
		}

		public string visitVariableExpr(Expr.Variable expr)
		{
			throw new NotImplementedException();
		}

		private string Parenthesize(string name, params Expr[] exprs)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("(").Append(name);
			foreach(Expr expr in exprs)
			{
				builder.Append(" ");
				builder.Append(expr.Accept<string>(this));
			}
			builder.Append(")");

			return builder.ToString();
		}
	}
}
