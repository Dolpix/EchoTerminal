using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal;
using EchoTerminal.TerminalCore;

[SuggestorFor(typeof(Enum))]
public class EnumSuggester : ISuggester
{
	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (expectedType is not { IsEnum: true })
		{
			return Array.Empty<string>();
		}

		string[] names = Enum.GetNames(expectedType);

		if (string.IsNullOrEmpty(partial))
		{
			return names;
		}

		return names
			.Where(n => n.StartsWith(partial, StringComparison.OrdinalIgnoreCase) &&
			            !string.Equals(n, partial, StringComparison.OrdinalIgnoreCase))
			.ToList();
	}
}