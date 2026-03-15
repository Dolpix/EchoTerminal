using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal
{
public class CommandHints
{
	private readonly CommandParser _parser;

	public CommandHints(CommandParser parser)
	{
		_parser = parser;
	}

	public List<string> GetHints(string input)
	{
		var result = _parser.Parse(input);

		if (!result.IsKnownCommand || result.Args == null)
		{
			return null;
		}

		var hints = new List<string>();
		foreach (var overload in result.Overloads)
		{
			var parts = overload.Params.Select(p => p.Expected.ToString()).ToList();
			if (parts.Count > 0)
			{
				hints.Add(string.Join(" ", parts));
			}
		}

		return hints.Count > 0 ? hints : null;
	}
}
}