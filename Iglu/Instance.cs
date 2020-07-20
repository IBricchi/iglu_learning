using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Instance
	{
		private Class klass;
		private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

		public Instance(Class klass)
		{
			this.klass = klass;
		}

		public override string ToString()
		{
			return "<instance " + klass.name + ">";
		}

		public object Get(Token name)
		{
			if(fields.ContainsKey(name.lexeme))
			{
				return fields[name.lexeme];
			}

			throw new RuntimeError(name, "Undefined property '" + name.lexeme + "'.");
		}

		public void Set(Token name, object value)
		{
			fields[name.lexeme] = value;
		}
	}
}
