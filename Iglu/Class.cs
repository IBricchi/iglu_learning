using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	class Class : ICallable
	{
		public readonly string name;

		public Class(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "<class " + name + ">";
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
