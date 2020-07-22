using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Class : ICallable
	{
		public readonly string name;
		private readonly Dictionary<string, Function> methods;

		public Class(string name, Dictionary<string, Function> methods)
		{
			this.name = name;
			this.methods = methods;
		}

		public override string ToString()
		{
			return "<class " + name + ">";
		}

		public Function FindMethod(string name)
		{
			if(methods.ContainsKey(name))
			{
				return methods[name];
			}

			return null;
		}

		public object Call(Interpreter interpreter, List<object> arguments)
		{
			Instance instance = new Instance(this);
			return instance;
		}

		public int Arity()
		{
			return 0;
		}
	}
}
