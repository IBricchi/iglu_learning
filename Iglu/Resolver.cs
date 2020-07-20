using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iglu
{
	class Resolver : Expr.IVisitor<Void>, Stmt.IVisitor<Void>
	{
		private readonly Interpreter interpreter;
		private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
		private FunctionType currentFunction = FunctionType.NONE;

		public Resolver(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}
		
		private enum FunctionType
		{
			NONE,
			FUNCTION
		}

		public void Resolve(List<Stmt> stmts)
		{
			foreach (Stmt stmt in stmts)
			{
				Resolve(stmt);
			}
		}

		private void Resolve(Stmt stmt)
		{
			stmt.Accept(this);
		}

		private void Resolve(Expr expr)
		{
			expr.Accept(this);
		}

		private void BeginScope()
		{
			scopes.Push(new Dictionary<string, bool>());
		}

		private void EndScope()
		{
			scopes.Pop();
		}

		private void Declare(Token name)
		{
			if (scopes.Count == 0) return;

			Dictionary<string, bool> scope = scopes.Peek();

			if (scope.ContainsKey(name.lexeme))
			{
				Program.Error(name, "Variable with this name already exists in this scope.");
			}

			scope[name.lexeme] = false;
		}

		private void Define(Token name)
		{
			if (scopes.Count == 0) return;

			Dictionary<string, bool> scope = scopes.Peek();
			scope[name.lexeme] = true;
		}

		private void ResolveLocal(Expr expr, Token name)
		{
			for(int i = scopes.Count - 1; i >= 0; i--)
			{
				if(scopes.ElementAt(i).ContainsKey(name.lexeme))
				{
					interpreter.Resolve(expr, scopes.Count - 1 - i);
				}
			}
		}

		private void ResolveFunction(Stmt.Function fn, FunctionType type)
		{
			FunctionType enclosingFunctionType = currentFunction;
			currentFunction = type;

			BeginScope();
			foreach(Token parameter in fn.parameters)
			{
				Declare(parameter);
				Define(parameter);
			}
			Resolve(fn.body);

			EndScope();

			currentFunction = enclosingFunctionType;
		}

		
		public Void visitAssignExpr(Expr.Assign expr)
		{
			Resolve(expr.value);
			ResolveLocal(expr, expr.name);

			return null;
		}

		public Void visitBinaryExpr(Expr.Binary expr)
		{
			Resolve(expr.left);
			Resolve(expr.right);

			return null;
		}

		public Void visitCallExpr(Expr.Call expr)
		{
			Resolve(expr.callee);

			foreach(Expr arg in expr.args)
			{
				Resolve(arg);
			}

			return null;
		}

		public Void visitGroupingExpr(Expr.Grouping expr)
		{
			Resolve(expr.expression);

			return null;
		}

		public Void visitLiteralExpr(Expr.Literal expr)
		{
			return null;
		}

		public Void visitUnaryExpr(Expr.Unary expr)
		{
			Resolve(expr.right);

			return null;
		}

		public Void visitVariableExpr(Expr.Variable expr)
		{
			if(scopes.Count != 0 && scopes.Peek()[expr.name.lexeme] == false)
			{
				Program.Error(expr.name, "Cannot read local variable in its own initializer.");
			}

			ResolveLocal(expr, expr.name);

			return null;
		}

		public Void visitGetExpr(Expr.Get expr)
		{
			Resolve(expr.obj);

			return null;
		}

		public Void visitSetExpr(Expr.Set expr)
		{
			Resolve(expr.value);
			Resolve(expr.obj);

			return null;
		}




		public Void visitBlockStmt(Stmt.Block stmt)
		{
			BeginScope();
			Resolve(stmt.statements);
			EndScope();

			return null;
		}

		public Void visitExpressionStmt(Stmt.Expression stmt)
		{
			Resolve(stmt.expression);

			return null;
		}

		public Void visitFunctionStmt(Stmt.Function stmt)
		{
			Declare(stmt.name);
			Define(stmt.name);

			ResolveFunction(stmt, FunctionType.FUNCTION);

			return null;
		}

		public Void visitIfStmt(Stmt.If stmt)
		{
			Resolve(stmt.condition);
			Resolve(stmt.then);
			if(stmt.el != null) Resolve(stmt.el);

			return null;
		}

		public Void visitLetStmt(Stmt.Let stmt)
		{
			Declare(stmt.name);
			if(stmt.initializer != null)
			{
				Resolve(stmt.initializer);
			}
			Define(stmt.name);

			return null;
		}

		public Void visitPrintStmt(Stmt.Print stmt)
		{
			Resolve(stmt.expression);

			return null;
		}

		public Void visitReturnStmt(Stmt.Return stmt)
		{
			if(currentFunction == FunctionType.NONE)
			{
				Program.Error(stmt.keyword, "Cannot return from top level code");
			}

			if(stmt.value != null) Resolve(stmt.value);

			return null;
		}

		public Void visitWhileStmt(Stmt.While stmt)
		{
			Resolve(stmt.condition);
			Resolve(stmt.body);

			return null;
		}

		public Void visitClassStmt(Stmt.Class stmt)
		{
			Declare(stmt.name);
			Define(stmt.name);

			return null;
		}
	}
}
