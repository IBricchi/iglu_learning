using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu.NativeFunctions
{
	class Clock : ICallable
	{
		public int Arity() { return 0; }

		public object Call(Interpreter interpreter, List<object> args)
		{
			return (double)Environment.TickCount / 1000f;
		}

		public override string ToString()
		{
			return "<native fn>";
		}
	}
}
