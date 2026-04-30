using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Attributes;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
[SuggestorFor(typeof(bool))]
public class BoolSuggester : ISuggester
{
	private static readonly string[] Values = { "true", "false" };

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return Values;
		}

		return Values
			   .Where(v => v.StartsWith(partial, StringComparison.OrdinalIgnoreCase) &&
						   !string.Equals(v, partial, StringComparison.OrdinalIgnoreCase))
			   .ToList();
	}
}
}