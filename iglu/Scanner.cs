using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iglu
{
	class Scanner
	{
		private readonly string source;

		private readonly List<Token> tokens = new List<Token>();

		private int start = 0;
		private int current = 0;
		private int length = 0;
		private int line = 1;

		private static readonly Dictionary<String, TokenType> keywords = new Dictionary<string, TokenType>()
		{
			{ "and", TokenType.AND },
			{ "class", TokenType.CLASS },
			{ "def", TokenType.DEF },
			{ "else", TokenType.ELSE },
			{ "false", TokenType.FALSE },
			{ "for", TokenType.FOR },
			{ "if", TokenType.IF },
			{ "let", TokenType.LET },
			{ "null", TokenType.NULL },
			{ "or", TokenType.OR },
			{ "print", TokenType.PRINT },
			{ "return", TokenType.RETURN },
			{ "parent", TokenType.PARENT },
			{ "this", TokenType.THIS },
			{ "while", TokenType.WHILE }
		};

		public Scanner(string source)
		{
			this.source = source;
		}

		internal List<Token> ScanTokens()
		{
			while(!IsAtEnd())
			{
				length = 0;
				start = current;
				ScanToken();
			}

			tokens.Add(new Token(TokenType.EOF, "", null, line));
			return tokens;
		}

		private bool Match(char expected)
		{
			if(IsAtEnd())
			{
				return false;
			}
			if(source[current] != expected)
			{
				return false;
			}

			length++;
			current++;
			return true;
		}

		private char Peek()
		{
			if(IsAtEnd())
			{
				return '\0';
			}
			else
			{
				return source[current];
			}
		}

		private char PeekNext()
		{
			if(current + 1 >= source.Length)
			{
				return '\0';
			}
			else
			{
				return source[current + 1];
			}
		}

		private bool IsDigit(char c)
		{
			return c >= '0' && c <= '9';
		}
		private bool IsAlpha(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
		}
		private bool IsAlphaNumeric(char c)
		{
			return IsAlpha(c) || IsDigit(c);
		}

		private bool IsAtEnd()
		{
			return current >= source.Length;
		}

		private char Advance()
		{
			length++;
			current++;
			return source[current - 1];
		}

		private void AddToken(TokenType type)
		{
			AddToken(type, null);
		}
		private void AddToken(TokenType type, Object literal)
		{
			string text = source.Substring(start, length);
			tokens.Add(new Token(type, text, literal, line));
		}

		private void ScanToken()
		{
			char c = Advance();
			switch (c)
			{
				// single independant characters
				case '(': AddToken(TokenType.LEFT_PAREN); break;
				case ')': AddToken(TokenType.RIGHT_PAREN); break;
				case '{': AddToken(TokenType.LEFT_BRACE); break;
				case '}': AddToken(TokenType.RIGHT_BRACE); break;
				case ',': AddToken(TokenType.COMMA); break;
				case '.': AddToken(TokenType.DOT); break;
				case '-': AddToken(TokenType.MINUS); break;
				case '+': AddToken(TokenType.PLUS); break;
				case ';': AddToken(TokenType.SEMICOLON); break;
				case '*': AddToken(TokenType.STAR); break;

				// characters which could be in pairs or single
				case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
				case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
				case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
				case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

				// longer lexemes
				// slash or comment?
				case '/':
					if(Match('/'))
					{
						// A comment goes until the end of the line
						while(Peek() != '\n' && !IsAtEnd())
						{
							Advance();
						}
					}
					else
					{
						AddToken(TokenType.SLASH);
					}
					break;
				// string
				case '"': String(); break;

				// Ignore whitespace.
				case ' ':
				case '\r':
				case '\t':
					break;
				case '\n':
					line++;
					break;

				// if nothing was found
				default:
					// if is number
					if(IsDigit(c))
					{
						Number();
						break;
					}
					// if is reserved words or identifiers
					else if(IsAlpha(c))
					{
						Identifier();
						break;
					}
					else
					{
						Program.Error(line, "Unexpected character: " + c + ".");
						break;
					}
			}
		}

		private void String()
		{
			while(Peek() != '"' && !IsAtEnd())
			{
				if(Peek() == '\n')
				{
					line++;
				}
				Advance();
			}

			// Unterminated string
			if(IsAtEnd())
			{
				Program.Error(line, "Unterminated string");
			}

			// Consume the closing ".
			Advance();

			// Trim the surrounding quotes.
			string value = source.Substring(start + 1, length - 2);
			AddToken(TokenType.STRING, value);
		}

		private void Number()
		{
			while(IsDigit(Peek()))
			{
				Advance();
			}

			if(Peek() == '.' && IsDigit(PeekNext()))
			{
				Advance();
				while (IsDigit(Peek()))
				{
					Advance();
				}
			}


			double value = Double.Parse(source.Substring(start, length));
			AddToken(TokenType.NUMBER, value);
		}

		private void Identifier()
		{
			while(IsAlphaNumeric(Peek()))
			{
				Advance();
			}

			string text = source.Substring(start, length);

			TokenType type;
			if(!keywords.TryGetValue(text, out type))
			{
				type = TokenType.IDENTIFIER;
			}
			AddToken(type);
		}

	}
}
