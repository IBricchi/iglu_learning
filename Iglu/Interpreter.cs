using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Interpreter : Expr.IVisitor<object>
	{
		public void Interpret(Expr expr)
		{
			try
			{
				object value = Evaluate(expr);
				Console.WriteLine(Stringify(value));
			}
			catch(RuntimeError error)
			{
				Program.RuntimeError(error);
			}

		}

		private string Stringify(object obj)
		{
			if (obj == null) return "null";

			if (obj is double) return obj.ToString();

			if (obj is bool) return IsTruthy(obj) ? "true" : "false";

			if (obj is string) return (string)obj;

			return "";
		}

		private object Evaluate(Expr expr)
		{
			return expr.Accept(this);
		}

		private bool IsTruthy(object obj)
		{
			if (obj == null) return false;
			if (obj is bool) return (bool)obj;
			return true;
		}

		private bool IsEqual(object a, object b)
		{
			if (a == null && b == null) return true;
			if (a == null) return false;

			return a.Equals(b);
		}

		private void CheckNumberOperand(Token oper, object operand)
		{
			if (operand is double) return;
			throw new RuntimeError(oper, "Operand must be a number.");
		}
		private void CheckNumberOperand(Token oper, object left, object right)
		{
			if (left is double && right is double) return;
			throw new RuntimeError(oper, "Operands must be numbers.");
		}

		public object visitBinaryExpr(Expr.Binary expr)
		{
			object left = Evaluate(expr.left);
			object right = Evaluate(expr.right);	

			switch(expr.oper.type)
			{
				// arithmatic
				case TokenType.MINUS:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left - (double)right;
				case TokenType.SLASH:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left / (double)right;
				case TokenType.STAR:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left * (double)right;
				case TokenType.PLUS:
					if (left is double && right is double)
					{
						return (double)left + (double)right;
					}
					if (left is string && right is string)
					{
						return (string)left + (string)right;
					}
					throw new RuntimeError(expr.oper, "Operand must be two numbers or two strings.");
				// comparisons
				case TokenType.GREATER:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left > (double)right;
				case TokenType.GREATER_EQUAL:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left >= (double)right;
				case TokenType.LESS:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left < (double)right;
				case TokenType.LESS_EQUAL:
					CheckNumberOperand(expr.oper, left, right);
					return (double)left <= (double)right;
				case TokenType.EQUAL_EQUAL:
					return IsEqual(left, right);
				case TokenType.BANG_EQUAL:
					return !IsEqual(left, right);
				// other operators
				case TokenType.COMMA:
					Evaluate(expr.left);
					return right;
				case TokenType.QUESTION:
					Expr.Binary ifs = (Expr.Binary)expr.right;
					if(IsTruthy(left))
					{
						return Evaluate(ifs.left);
					}
					else
					{
						return Evaluate(ifs.right);
					}
				default:
					break;
			}

			return null; // should be unreachable;
		}

		public object visitGroupingExpr(Expr.Grouping expr)
		{
			return Evaluate(expr.expression);
		}

		public object visitLiteralExpr(Expr.Literal expr)
		{
			return expr.value;
		}

		public object visitUnaryExpr(Expr.Unary expr)
		{
			object right = Evaluate(expr.right);

			switch(expr.oper.type)
			{
				case TokenType.MINUS:
					CheckNumberOperand(expr.oper, right);
					return -(double)right;
				case TokenType.BANG:
					return !IsTruthy(right);
			}

			return null; // should be unreachable
		}
	}
}
