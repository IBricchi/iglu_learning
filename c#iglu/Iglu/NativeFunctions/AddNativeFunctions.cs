using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu.NativeFunctions
{
	static class AddNativeFunctions
	{
		public static void AddAll(Env globals)
		{
			globals.Define("clock", new Clock());
		}
	}
}
