using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Function : ICallable
	{
		private readonly Stmt.Function declaration;
		private readonly Env closure;

		public Function(Stmt.Function declaration, Env closure)
		{
			this.declaration = declaration;
			this.closure = closure;
		}

		public int Arity()
		{
			return declaration.parameters.Count;
		}

		public object Call(Interpreter interpreter, List<object> args)
		{
			Env env = new Env(closure);
			for(int i = 0; i < declaration.parameters.Count; i++)
			{
				env.Define(declaration.parameters[i].lexeme, args[i]);
			}

			try
			{
				interpreter.ExecuteBlock(declaration.body, env);
			}
			catch(Return returnValue)
			{
				return returnValue.value;
			}

			return null;
		}

		public override string ToString()
		{
			return "<fn " + declaration.name.lexeme + " >";
		}
	}
}
