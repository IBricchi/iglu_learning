using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Env
	{
		private readonly Dictionary<string, object> values = new Dictionary<string, object>();

		public void Define(string name, object value)
		{
			values[name] = value;
		}

		public void Assign(Token name, object value)
		{
			if(values.ContainsKey(name.lexeme))
			{
				values[name.lexeme] = value;
				return;
			}

			throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "',");
		}

		public object Get(Token name)
		{
			if(values.ContainsKey(name.lexeme))
			{
				return values[name.lexeme];
			}

			throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
		}
	}
}
