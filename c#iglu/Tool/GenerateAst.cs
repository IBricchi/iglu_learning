using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tool
{
	static class GenerateAst
	{
		public static void SubMain(List<string> args)
		{
			if (args.Count != 1)
			{
				Console.Error.WriteLine("Usage: Tool GenerateAst <output directory>");
				Environment.Exit(64);
			}
			else
			{
				String outputDir = args[0];
				DefineAst(outputDir, "Expr", new List<string>(){
					"Assign   : Token name, Expr value",
					"Binary   : Expr left, Token oper, Expr right",
					"Call     : Expr callee, Token paren, List<Expr> args",
					"Get      : Expr obj, Token name",
					"Grouping : Expr expression",
					"Literal  : object value",
					"Unary    : Token oper, Expr right",
					"This     : Token keyword",
					"Set      : Expr obj, Token name, Expr value",
					"Variable : Token name"
				});

				DefineAst(outputDir, "Stmt", new List<string>(){
					"Block      : List<Stmt> statements",
					"If         : Expr condition, Stmt then, Stmt el",
					"Class      : Token name, List<Stmt.Function> methods",
					"Expression : Expr expression",
					"Function   : Token name, List<Token> parameters, List<Stmt> body",
					"Print      : Expr expression",
					"Return     : Token keyword, Expr value",
					"Let        : Token name, Expr initializer",
					"While      : Expr condition, Stmt body"
				});
			}
		}

		private static void DefineAst(string outputDir, string baseName, List<string> types)
		{
			string path = outputDir + "\\" + baseName + ".cs";
			using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
			{
				writer.WriteLine("using System;");
				writer.WriteLine("using System.Collections.Generic;");
				writer.WriteLine("");
				writer.WriteLine("namespace Iglu");
				writer.WriteLine("{");
				writer.WriteLine("\tabstract class " + baseName);
				writer.WriteLine("\t{");

				DefineVisitor(writer, baseName, types);

				// The AST classes.
				foreach (string type in types)
				{
					string className = type.Split(":")[0].Trim();
					string fields = type.Split(":")[1].Trim();
					DefineType(writer, baseName, className, fields);
				}

				// the base accept method
				writer.WriteLine();
				writer.WriteLine("\t\tpublic abstract R Accept<R>(IVisitor<R> visitor);");

				writer.WriteLine("\t}");
				writer.WriteLine("}");
			}	
		}

		private static void DefineVisitor(
			StreamWriter writer,
			string baseName,
			List<string> types
		)
		{
			writer.WriteLine("\t\tpublic interface IVisitor<R>");
			writer.WriteLine("\t\t{");
			foreach(string type in types)
			{
				string typeName = type.Split(":")[0].Trim();
				writer.WriteLine("\t\t\tR visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
			}
			writer.WriteLine("\t\t}");
		}

		private static void DefineType(
			StreamWriter writer,
			string baseName,
			string className,
			string fieldList
		)
		{
			writer.WriteLine("\t\tpublic class " + className + " : " + baseName);
			writer.WriteLine("\t\t{");

			// constructor
			writer.WriteLine("\t\t\tpublic " + className + "(" + fieldList + ")");
			writer.WriteLine("\t\t\t{");

			// Store parameters in fields
			string[] fields = fieldList.Split(", ");
			foreach (string field in fields)
			{
				string name = field.Split(" ")[1];
				writer.WriteLine("\t\t\t\tthis." + name + " = " + name + ";");
			}
			writer.WriteLine("\t\t\t}");

			// fields
			writer.WriteLine();
			foreach(string field in fields)
			{
				writer.WriteLine("\t\t\tpublic readonly " + field + ";");
			}

			// visitor pattern
			writer.WriteLine();
			writer.WriteLine("\t\t\tpublic override R Accept<R>(IVisitor<R> visitor)");
			writer.WriteLine("\t\t\t{");
			writer.WriteLine("\t\t\t\treturn visitor.visit" + className + baseName + "(this);");
			writer.WriteLine("\t\t\t}");
			
			writer.WriteLine("\t\t}");
		}
	}
}
