using System;
using System.Collections.Generic;
using System.Linq;

namespace Tool
{
	class Program
	{
		static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				Console.Error.WriteLine("Usage: Tool <tool name> <Tool Parameters>");
			}
			else
			{
				List<string> newArgs = args.ToList<string>();
				newArgs.RemoveAt(0);
				switch (args[0])
				{
					case "GenerateAst":
						GenerateAst.SubMain(newArgs);
						break;
					default:
						Console.Error.WriteLine("Error: Unknown tool name)");
						break;
				}

			}
		}
	}
}
