﻿namespace Iglu
{
	enum TokenType{
		// Single-character tokens.
		LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
		COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR, COLON,

		// One or two character tokens.
		BANG, BANG_EQUAL,
		EQUAL, EQUAL_EQUAL,
		GREATER, GREATER_EQUAL,
		LESS, LESS_EQUAL,

		// Literals.
		IDENTIFIER, STRING, NUMBER,

		// Keywords.
		AND, CLASS, ELSE, FALSE, DEF, FOR, IF, NULL, OR,
		PRINT, RETURN, PARENT, THIS, TRUE, LET, WHILE, CHILD_OF,

		EOF
	}
}
