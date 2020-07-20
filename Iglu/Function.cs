using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Function : ICallable
	{
		private readonly Stmt.Function declaration;

		public Function(Stmt.Function declaration)
		{
			this.declaration = declaration;
		}

		public int Arity()
		{
			return declaration.parameters.Count;
		}

		public object Call(Interpreter interpreter, List<object> args)
		{
			Env env = new Env(interpreter.globals);
			for(int i = 0; i < declaration.parameters.Count; i++)
			{
				env.Define(declaration.parameters[i].lexeme, args[i]);
			}

			interpreter.ExecuteBlock(declaration.body, env);

			return null;
		}

		public override string ToString()
		{
			return "<fn " + declaration.name.lexeme + " >";
		}
	}
}
