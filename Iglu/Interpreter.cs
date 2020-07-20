using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<Void>
	{
		public readonly Env globals = new Env();
		private Env environment;

		private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

		private bool REPL;

		public void Interpret(List<Stmt> statements, bool REPL)
		{
			environment = globals;
			NativeFunctions.AddNativeFunctions.AddAll(globals);

			this.REPL = REPL;

			if (statements.Count != 1) this.REPL = false;

			try
			{
				foreach(Stmt statement in statements)
				{
					Execute(statement);
				}
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

			if (obj is string @string) return @string;

			return "";
		}

		public object Evaluate(Expr expr)
		{
			return expr.Accept(this);
		}

		public void Execute(Stmt statement)
		{
			statement.Accept(this);
		}

		public void ExecuteBlock(List<Stmt> statements, Env environment)
		{
			Env previous = this.environment;
			try
			{
				this.environment = environment;

				foreach(Stmt statement in statements)
				{
					Execute(statement);
				}
			}
			finally
			{
				this.environment = previous;
			}
		}

		public void Resolve(Expr expr, int depth)
		{
			locals[expr] = depth;
		}

		private object LookUpVariable(Token name, Expr expr)
		{
			int distance = -1;
			if (locals.ContainsKey(expr)) distance = locals[expr];

			if(distance != -1)
			{
				return environment.GetAt(distance, name.lexeme);
			}
			else
			{
				return globals.Get(name);
			}
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

		public object visitAssignExpr(Expr.Assign expr)
		{
			object value = Evaluate(expr.value);

			int distance = -1;
			if (locals.ContainsKey(expr)) distance = locals[expr];
			if(distance != -1)
			{
				environment.AssignAt(distance, expr.name, value);
			}
			else
			{
				environment.Assign(expr.name, value);
			}

			environment.Assign(expr.name, value);
			return value;
		}

		public object visitBinaryExpr(Expr.Binary expr)
		{
			object left = Evaluate(expr.left);
			object right;

			switch(expr.oper.type)
			{
				// arithmatic
				case TokenType.MINUS:
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left - (double)right;
				case TokenType.SLASH:
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left / (double)right;
				case TokenType.STAR:
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left * (double)right;
				case TokenType.PLUS:
					right = Evaluate(expr.right);
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
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left > (double)right;
				case TokenType.GREATER_EQUAL:
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left >= (double)right;
				case TokenType.LESS:
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left < (double)right;
				case TokenType.LESS_EQUAL:
					right = Evaluate(expr.right);
					CheckNumberOperand(expr.oper, left, right);
					return (double)left <= (double)right;
				case TokenType.EQUAL_EQUAL:
					right = Evaluate(expr.right);
					return IsEqual(left, right);
				case TokenType.BANG_EQUAL:
					right = Evaluate(expr.right);
					return !IsEqual(left, right);
				// other operators
				case TokenType.COMMA:
					right = Evaluate(expr.right);
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
				case TokenType.AND:
					if (!IsTruthy(left)) return left;
					return Evaluate(expr.right);
				case TokenType.OR:
					if (IsTruthy(left)) return left;
					return Evaluate(expr.right);
				default:
					break;
			}

			return null; // should be unreachable;
		}

		public object visitCallExpr(Expr.Call expr)
		{
			object callee = Evaluate(expr.callee);

			if(!(callee is ICallable))
			{
				throw new RuntimeError(expr.paren, "Can only call on functions and classes.");
			}

			List<object> args = new List<object>();
			foreach(Expr arg in expr.args)
			{
				args.Add(Evaluate(arg));
			}

			ICallable function = (ICallable)callee;
			
			if(args.Count != function.Arity())
			{
				throw new RuntimeError(expr.paren, "Expected " + function.Arity() + " arguments but got " + args.Count + ".");
			}
			
			return function.Call(this, args);
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

		public object visitVariableExpr(Expr.Variable expr)
		{
			return environment.Get(expr.name);
		}

		public Void visitBlockStmt(Stmt.Block stmt)
		{
			bool previous = REPL;
			REPL = false;
			ExecuteBlock(stmt.statements, new Env(environment));
			REPL = previous;
			return null;
		}

		public Void visitIfStmt(Stmt.If stmt)
		{
			bool cond = IsTruthy(Evaluate(stmt.condition));

			if(cond)
			{
				Execute(stmt.then);
			}else if(stmt.el != null)
			{
				Execute(stmt.el);
			}

			return null;
		}

		public Void visitExpressionStmt(Stmt.Expression stmt)
		{
			object value = Evaluate(stmt.expression);
			if (REPL) Console.Out.WriteLine(Stringify(value));
			return null;
		}

		public Void visitPrintStmt(Stmt.Print stmt)
		{
			object value = Evaluate(stmt.expression);
			Console.WriteLine(Stringify(value));
			return null;
		}

		public Void visitLetStmt(Stmt.Let stmt)
		{
			object value = null;
			if(stmt.initializer != null)
			{
				value = Evaluate(stmt.initializer);
			}

			environment.Define(stmt.name.lexeme, value);
			return null;
		}

		public Void visitWhileStmt(Stmt.While stmt)
		{
			while(IsTruthy(Evaluate(stmt.condition)))
			{
				Execute(stmt.body);
			}

			return null;
		}

		public Void visitFunctionStmt(Stmt.Function stmt)
		{
			Function fn = new Function(stmt, environment);
			if(stmt.name != null) environment.Define(stmt.name.lexeme, fn);

			return null;
		}

		public Void visitReturnStmt(Stmt.Return stmt)
		{
			object value =  null;

			if (stmt.value != null) value = Evaluate(stmt.value);

			throw new Return(value);
		}
	}
}
