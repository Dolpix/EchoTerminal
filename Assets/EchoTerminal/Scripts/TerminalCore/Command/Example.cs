using System;
using System.Collections.Generic;

public static class Example
{
	public static void Main()
	{
		var parsers = new List<ITokenParser>
		{
			new IntParser(),
			new FloatParser(),
			new TargetParser(new[] { "@Player", "@Enemy1", "@Enemy2" }),
			new CommandNameParser(new[] { "Teleport", "Spawn", "Kill" }),
			new QuotedStringParser(),
			new Vec3Parser(),
			new StringParser()
		};

		var inputs = new[]
		{
			"Teleport @Player (10, 0, 5)",
			"Kill @Enemy1",
			"Spawn Goblin (1, 2, 3)",
			"Teleport @Player \"Hello World!\" (0, 0, 0)",
		};

		foreach (var input in inputs)
		{
			Console.WriteLine($"Input: {input}");
			Console.WriteLine(new string('-', 50));

			var tokens = Tokenizer.Tokenize(input, parsers);

			foreach (var token in tokens)
			{
				Console.WriteLine($"  \"{token.Raw}\"  type={token.Type?.Name ?? "unknown"}  state={token.State}");
			}

			Console.WriteLine();
		}
	}
}