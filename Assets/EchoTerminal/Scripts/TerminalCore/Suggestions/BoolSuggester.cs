using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal;
using EchoTerminal.TerminalCore;

[SuggestorFor(typeof(bool))]
public class BoolSuggester : ISuggester
{
	private static readonly string[] _values = { "true", "false" };

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return _values;
		}

		return _values
			.Where(v => v.StartsWith(partial, StringComparison.OrdinalIgnoreCase) &&
			            !string.Equals(v, partial, StringComparison.OrdinalIgnoreCase))
			.ToList();
	}
}