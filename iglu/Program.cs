using iglu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iglu
{
	static class Program
	{
		private static readonly Interpreter interpreter = new Interpreter();

		public static bool hadError = false;
		public static bool hadRuntimeError = false;

		static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				Console.Out.WriteLine("Usage: iglu [script]");
				Environment.Exit(64);
			}
			else if (args.Length == 1)
			{
				RunFile(args[0]);
			}
			else
			{
				RunPrompt();
			}
		}

		private static void RunFile(string path)
		{
			//byte[] bytes = Encoding.UTF8.GetBytes(File.ReadAllText(path));
			//run(Encoding.UTF8.GetString(bytes));

			Run(File.ReadAllText(path));

			// Indicate an error in the exit code.
			if (hadError) Environment.Exit(65);
			if (hadRuntimeError) Environment.Exit(70);
		}

		private static void RunPrompt()
		{
			TextReader reader = Console.In;

			bool lineIsNull = false;
			while(!lineIsNull)
			{
				Console.Out.Write("> ");
				string line = reader.ReadLine();
				if (line == null)
				{
					lineIsNull = true;
				}
				else
				{
					if (!line.EndsWith(';')) line += ';';
					Run(line);
					hadError = false;
				}
			}
		}

		private static void Run(string source)
		{
			Scanner scanner = new Scanner(source);
			List<Token> tokens = scanner.ScanTokens();

			// prints out each token
			// foreach(Token token in tokens)
			// {
			//	 Console.Out.WriteLine(token);
			// }

			Parser parser = new Parser(tokens);
			List<Stmt> statements = parser.Parse();

			// stop if syntax error
			if (hadError) return;

			// prints out parsed tree
			// Console.Out.WriteLine(new AstPrinter().Print(expression));

			// interpret
			interpreter.Interpret(statements);

		}

		public static void Error(int line, string message)
		{
			Report(line, "", message);
		}

		private static void Report(int line, string where, string message)
		{
			Console.Error.WriteLine(
				"[line " + line + " ] Error" + where + ": " + message
			);
			hadError = true;
		}

		public static void Error(Token token, string message)
		{
			if (token.type == TokenType.EOF)
			{
				Report(token.line, " at end", message);
			}
			else
			{
				Report(token.line, " at '" + token.lexeme + "'", message);
			}
		}

		public static void RuntimeError(RuntimeError error)
		{
			Console.Error.WriteLine(error.Message + "\n[line" + error.token.line + "]");
			hadRuntimeError = true;
		}
	}
		
}
