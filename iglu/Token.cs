using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iglu
{
	class Token
	{
		readonly TokenType type;
		readonly string lexeme;
		readonly Object literal;
		readonly int line;

		public Token(TokenType type, String lexeme, Object literal, int line)
		{
			this.type = type;
			this.lexeme = lexeme;
			this.literal = literal;
			this.line = line;
		}

		public override string ToString()
		{
			return type + " " + lexeme + " " + literal;
		}
	}
}
