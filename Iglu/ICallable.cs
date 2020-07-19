using System;
using System.Collections.Generic;

namespace Iglu
{
	interface ICallable
	{
		object Call(Interpreter interpreter, List<object> args);

		int Arity();
	}
}
