using System;
using System.Collections.Generic;
using System.Text;

namespace Iglu
{
	sealed class Void
	{
		public static readonly Void Instance = null; // You don't even need this line
		private Void() { }
	}
}
