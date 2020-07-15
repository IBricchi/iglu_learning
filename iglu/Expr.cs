using Iglu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iglu
{
	abstract class Expr
	{
		class Binary : Expr
		{
			public Binary(Expr left, Token oper, Expr right)
			: base(left, oper, right) { }
		}

		public Expr(Expr left, Token oper, Expr right)
		{
			this.left = left;
			this.oper = oper;
			this.right = right;
		}

		internal readonly Expr left;
		internal readonly Token oper;
		internal readonly Expr right;

	}
}
